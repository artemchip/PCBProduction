#include "etcher_constants.h"
#include "etcher_gpio.h"
#include "etcher_wifi.h"
#include "etcher_server.h"

// Logging
const char* TAGIO = "Etcher-Main";

void app_main(void)
{
    gpio_init();
    esp_err_t err = nvs_flash_init();
    if (err == ESP_ERR_NVS_NO_FREE_PAGES || err == ESP_ERR_NVS_NEW_VERSION_FOUND) {
        ESP_ERROR_CHECK(nvs_flash_erase());
        err = nvs_flash_init();
    }
    wifi_start();
    http_server_start();
    gpio_main_loop();
    gpio_destroy();
}
