#include <stdio.h>
#include <esp_log.h>
#include <esp_system.h>
#include <nvs_flash.h>
#include <sys/param.h>
#include <string.h>
#include "esp_event.h"
#include "esp_netif.h"
#include <esp_wifi.h>
#include "esp_timer.h"
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "freertos/event_groups.h"
#include "esp_netif_sntp.h"
#include "lwip/ip_addr.h"
#include "esp_sntp.h"
#include "driver/gpio.h"
#include "driver/ledc.h"
#include <rom/ets_sys.h>
#include <math.h>
#include "cJSON.h"
#include "lwip/err.h"
#include "lwip/sockets.h"
#include "lwip/sys.h"
#include <lwip/netdb.h>

// Logging
const char* TAGIO = "PNP-Main";

#define C_STEP_PIN 27
#define C_DIR_PIN 32

#define VACPUMP_PIN 21
#define LIGHT_PIN 22

#define WIFI_SSID "ArtemChip-H24"
#define WIFI_PASSWORD "--------"

#define PORT 7008

int cur_position_z = -1;
static EventGroupHandle_t s_wifi_event_group;
static esp_netif_t *s_example_sta_netif = NULL;
int retries_cnt = 0;
#define WIFI_CONNECTED_BIT BIT0
#define WIFI_FAIL_BIT BIT1

int keepAlive = 1;
int keepIdle = 5;
int keepInterval = 1;
int keepCount = 1;
int noDelay = 1;

void init_gpio(void) {
    // Var
    cur_position_z = -1;
    
    // GPIO
    gpio_set_direction(C_STEP_PIN, GPIO_MODE_OUTPUT);
    gpio_set_direction(C_DIR_PIN, GPIO_MODE_OUTPUT);
    gpio_set_direction(VACPUMP_PIN, GPIO_MODE_OUTPUT);
    gpio_set_direction(LIGHT_PIN, GPIO_MODE_OUTPUT);
    gpio_set_level(C_STEP_PIN, 0);
    gpio_set_level(C_DIR_PIN, 0);
    gpio_set_level(VACPUMP_PIN, 0);
    gpio_set_level(LIGHT_PIN, 0);
}

static void wifi_event_handler(void *arg, esp_event_base_t event_base, int32_t event_id, void *event_data)
{
    if (event_base == WIFI_EVENT && event_id == WIFI_EVENT_STA_START) {
        esp_wifi_connect();
    } else if (event_base == WIFI_EVENT && event_id == WIFI_EVENT_STA_DISCONNECTED) {
        esp_wifi_connect();
        retries_cnt++;
        if (retries_cnt >= 10) {
            xEventGroupSetBits(s_wifi_event_group, WIFI_FAIL_BIT);
            ESP_LOGI(TAGIO, "Connect to Wi-Fi fail");
        }
    } else if (event_base == IP_EVENT && event_id == IP_EVENT_STA_GOT_IP) {
        ip_event_got_ip_t* event = (ip_event_got_ip_t*) event_data;
        ESP_LOGI(TAGIO, "Got IP:" IPSTR, IP2STR(&event->ip_info.ip));
        retries_cnt = 0;
        xEventGroupSetBits(s_wifi_event_group, WIFI_CONNECTED_BIT);
    }
}

esp_err_t init_wifi(void) {
    wifi_config_t wifi_config = {
        .sta = {
            .ssid = WIFI_SSID,
            .password = WIFI_PASSWORD,
            .pmf_cfg = {
                .capable = true,
                .required = false
            },
            .failure_retry_cnt = 10,
        },
    };
    retries_cnt = 0;
    s_wifi_event_group = xEventGroupCreate();
    ESP_ERROR_CHECK(esp_netif_init());
    ESP_ERROR_CHECK(esp_event_loop_create_default());
    wifi_init_config_t cfg = WIFI_INIT_CONFIG_DEFAULT();
    ESP_ERROR_CHECK(esp_wifi_init(&cfg));
    esp_netif_inherent_config_t esp_netif_config = ESP_NETIF_INHERENT_DEFAULT_WIFI_STA();
    esp_netif_config.route_prio = 128;
    s_example_sta_netif = esp_netif_create_wifi(WIFI_IF_STA, &esp_netif_config);
    esp_wifi_set_default_wifi_sta_handlers();
    ESP_ERROR_CHECK(esp_wifi_set_storage(WIFI_STORAGE_RAM));
    ESP_ERROR_CHECK(esp_wifi_set_ps(WIFI_PS_MIN_MODEM));
    ESP_ERROR_CHECK(esp_wifi_set_mode(WIFI_MODE_STA));
    ESP_ERROR_CHECK(esp_wifi_set_config(WIFI_IF_STA, &wifi_config));
    ESP_ERROR_CHECK(esp_event_handler_register(WIFI_EVENT, ESP_EVENT_ANY_ID, &wifi_event_handler, NULL));
    ESP_ERROR_CHECK(esp_event_handler_register(IP_EVENT, IP_EVENT_STA_GOT_IP, &wifi_event_handler, NULL));
    ESP_ERROR_CHECK(esp_wifi_start());
    ESP_LOGI(TAGIO, "Connecting to %s...", wifi_config.sta.ssid);
    EventBits_t bits = xEventGroupWaitBits(s_wifi_event_group,
            WIFI_CONNECTED_BIT | WIFI_FAIL_BIT,
            pdFALSE,
            pdFALSE,
            portMAX_DELAY);
    if (bits & WIFI_CONNECTED_BIT) {
        ESP_LOGI(TAGIO, "Connected to Wi-Fi!");
        return ESP_OK;
    } else if (bits & WIFI_FAIL_BIT) {
        ESP_LOGI(TAGIO, "Failed to connect to Wi-Fi!");
        return ESP_ERR_WIFI_NOT_STARTED;
    } else {
        ESP_LOGE(TAGIO, "Unexpected Wi-Fi event!");
        return ESP_ERR_WIFI_NOT_STARTED;
    }
}

bool prefix(const char *pre, const char *str)
{
    return strncmp(pre, str, strlen(pre)) == 0;
}

int gpio_perform_single_command(cJSON *cmd) {
    if (cmd == NULL) return -1;
    if (cJSON_GetObjectItem(cmd, "command")) {
        char *command = cJSON_GetObjectItem(cmd, "command")->valuestring;
        if (prefix("lighton", command)) {
            gpio_set_level(LIGHT_PIN, 1);
            return 0;
        } else if (prefix("lightoff", command)) {
            gpio_set_level(LIGHT_PIN, 0);
            return 0;
        } else if (prefix("vacon", command)) {
            gpio_set_level(VACPUMP_PIN, 1);
            return 0;
        } else if (prefix("vacoff", command)) {
            gpio_set_level(VACPUMP_PIN, 0);
            return 0;
        } else if (prefix("crotate", command)) {
            cJSON *arg = cJSON_GetObjectItem(cmd, "argument");
            if (arg) {
                if (!cJSON_IsNumber(arg)) {
                    return -1;
                }
                int cnt_steps = arg->valueint;
                gpio_set_level(C_DIR_PIN, (cnt_steps > 0) ? 1 : 0);
                for (int i = 0; i < abs(cnt_steps); i++) {
                    gpio_set_level(C_STEP_PIN, 1);
                    ets_delay_us(500);
                    gpio_set_level(C_STEP_PIN, 0);
                    ets_delay_us(500);
                }
                return 0;
            } else {
                return -1;
            }
        } else {
            return -1;
        }
    } else {
        return -1;
    }
}

int gpio_perform_commands(char* command_json) {
    cJSON *root = cJSON_Parse(command_json);
    int resval = -1;
    if (root == NULL) {
        return -1;
    } else if (cJSON_IsArray(root)) {
        cJSON *ritem = NULL;
        cJSON_ArrayForEach(ritem, root) {
            int subresval = gpio_perform_single_command(ritem);
            if (subresval != resval) resval = subresval;
        }
    } else {
        resval = gpio_perform_single_command(root);
    }
    cJSON_Delete(root);
    return resval;
}

static void communicate_with_client(const int sock) {
    int len;
    char rx_buffer[255];
    char tx_buffer[16];

    do {
        len = recv(sock, rx_buffer, sizeof(rx_buffer) - 1, 0);
        if (len > 0) {
            rx_buffer[len] = 0;
            int cmd_res = gpio_perform_commands(rx_buffer);
            int num_bytes = snprintf(tx_buffer, 16, "%d\n", cmd_res);
            int bytes_sent = send(sock, tx_buffer, num_bytes, 0);
            if (bytes_sent < num_bytes) {
                break;
            }
        }
    } while (len > 0);
}

int tcp_server_main(void) {
    // Variables
    struct sockaddr_storage dest_addr;
    struct sockaddr_in *dest_addr_ip4 = (struct sockaddr_in *)&dest_addr;
    dest_addr_ip4->sin_addr.s_addr = htonl(INADDR_ANY);
    dest_addr_ip4->sin_family = AF_INET;
    dest_addr_ip4->sin_port = htons(PORT);
    
    // Create socket
    int listen_sock = socket(AF_INET, SOCK_STREAM, IPPROTO_IP);
    if (listen_sock < 0) {
        ESP_LOGE(TAGIO, "Unable to create socket: errno %d", errno);
        return -1;
    }
    int opt = 1;
    setsockopt(listen_sock, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt));
    
    // Bind socket
    int err = bind(listen_sock, (struct sockaddr *)&dest_addr, sizeof(dest_addr));
    if (err != 0) {
        ESP_LOGE(TAGIO, "Socket unable to bind: errno %d", errno);
        close(listen_sock);
        return -1;
    }
    
    // Listen for connections
    err = listen(listen_sock, 1);
    if (err != 0) {
        ESP_LOGE(TAGIO, "Error occurred during listen: errno %d", errno);
        close(listen_sock);
        return -1;
    }
    
    // Loop for clients
    while (true) {
        // Accept
        struct sockaddr_storage source_addr;
        socklen_t addr_len = sizeof(source_addr);
        int sock = accept(listen_sock, (struct sockaddr *)&source_addr, &addr_len);
        if (sock < 0) {
            break;
        }
        
        // Options
        setsockopt(sock, SOL_SOCKET, SO_KEEPALIVE, &keepAlive, sizeof(int));
        setsockopt(sock, IPPROTO_TCP, TCP_KEEPIDLE, &keepIdle, sizeof(int));
        setsockopt(sock, IPPROTO_TCP, TCP_KEEPINTVL, &keepInterval, sizeof(int));
        setsockopt(sock, IPPROTO_TCP, TCP_KEEPCNT, &keepCount, sizeof(int));
        setsockopt(sock, IPPROTO_TCP, TCP_NODELAY, &noDelay, sizeof(int));
        
        // Communication
        communicate_with_client(sock);
        
        // Closure
        shutdown(sock, 0);
        close(sock);
        
        // After
        gpio_set_level(LIGHT_PIN, 0);
        gpio_set_level(VACPUMP_PIN, 0);
    }
    
    return 0;
}

void app_main(void)
{
    // Init
    esp_err_t err = nvs_flash_init();
    if (err == ESP_ERR_NVS_NO_FREE_PAGES || err == ESP_ERR_NVS_NEW_VERSION_FOUND) {
        ESP_ERROR_CHECK(nvs_flash_erase());
        err = nvs_flash_init();
    }
    ESP_ERROR_CHECK(err);
    init_gpio();
    if (init_wifi() != ESP_OK) {
        esp_restart();
        return;
    }
    
    // Server
    tcp_server_main();
}
