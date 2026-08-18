#include "etcher_server.h"
#include "etcher_gpio.h"

// MQTT
const char* HTTP_TAG = "Etcher-HTTP";

esp_err_t main_handler(httpd_req_t *req) {
    // Page start
    httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
    httpd_resp_set_hdr(req, "Connection", "close");
    httpd_resp_sendstr_chunk(req, "<!DOCTYPE html><html><meta charset=\"UTF-8\">");
    httpd_resp_sendstr_chunk(req, "<head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
    httpd_resp_sendstr_chunk(req, "<link rel=\"icon\" href=\"data:,\">");
    httpd_resp_sendstr_chunk(req, "<title>Etcher</title>");
    httpd_resp_sendstr_chunk(req, "<style>html { font-family: Helvetica; display: inline-block; margin: 0px auto; text-align: center;}");
    httpd_resp_sendstr_chunk(req, ".buttongreen { background-color: #4CAF50; border: none; color: white; padding: 16px 40px;");
    httpd_resp_sendstr_chunk(req, "text-decoration: none; font-size: 30px; margin: 2px; cursor: pointer;}");
    httpd_resp_sendstr_chunk(req, ".buttongrey {background-color: #555555; border: none; color: white; padding: 16px 40px;");
    httpd_resp_sendstr_chunk(req, "text-decoration: none; font-size: 30px; margin: 2px; cursor: pointer;}");
    httpd_resp_sendstr_chunk(req, ".buttonred {background-color: #ff0000; border: none; color: white; padding: 16px 40px;");
    httpd_resp_sendstr_chunk(req, "text-decoration: none; font-size: 30px; margin: 2px; cursor: pointer;}");
    httpd_resp_sendstr_chunk(req, "</style></head>");
    httpd_resp_sendstr_chunk(req, "<body><h1>Etcher</h1>");

    if (gpio_get_start_speed_mm_sec() > 0.0f && gpio_get_end_speed_mm_sec() > 0.0f && gpio_get_operation_type() > 0) {
        // Etch cycle in progress
        char line1[255];
        char line2[255];
        char line3[255];
        char move_direction[6] = "";
        char optype[8] = "";
        if (gpio_get_move_direction_up()) strcpy(move_direction, "UP"); else strcpy(move_direction, "DOWN");
        if (gpio_get_operation_type() == 1) {
            strcpy(optype, "FEED");
        } else if (gpio_get_operation_type() == 2) {
            strcpy(optype, "PROCESS");
        } else {
            strcpy(optype, "DRAIN");
        }
        snprintf(line1, 255, "Operation type: %s, move direction: %s, start speed: %.2f mm/sec, end speed: %.2f mm/sec, current speed: %.2f mm/sec, current position: %.2f mm", optype, move_direction, gpio_get_start_speed_mm_sec(), gpio_get_end_speed_mm_sec(), gpio_get_current_speed_mm_sec(), gpio_get_current_position_mm());
        snprintf(line2, 255, "Current duty cycle for pump 1: %d%%, pump 2: %d%%", gpio_get_pump1_duty_percent(), gpio_get_pump2_duty_percent());
        snprintf(line3, 255, "Current temperature: %.1f °C, target temperature: %.1f °C", gpio_get_current_temp_c(), gpio_get_target_temp_c());

        // Status of pending etch cycle
        httpd_resp_sendstr_chunk(req, "<p>");
        httpd_resp_sendstr_chunk(req, line1);
        httpd_resp_sendstr_chunk(req, "</p>");
        httpd_resp_sendstr_chunk(req, "<br>");
        httpd_resp_sendstr_chunk(req, "<p>");
        httpd_resp_sendstr_chunk(req, line2);
        httpd_resp_sendstr_chunk(req, "</p>");
        httpd_resp_sendstr_chunk(req, "<br>");
        httpd_resp_sendstr_chunk(req, "<p>");
        httpd_resp_sendstr_chunk(req, line3);
        httpd_resp_sendstr_chunk(req, "</p>");
        httpd_resp_sendstr_chunk(req, "<br>");

        // Pump 1 adjust
        httpd_resp_sendstr_chunk(req, "<form action=\"setpump1\" method=\"GET\">");
        httpd_resp_sendstr_chunk(req, "<p>Pump 1 duty cycle percent:</p>");
        httpd_resp_sendstr_chunk(req, "<input type=\"number\" id=\"value\" name=\"value\" min=\"0\" max=\"100\" step=\"1\"></input>");
        httpd_resp_sendstr_chunk(req, "<p><input type=\"submit\" value=\"Save\" /></p>");
        httpd_resp_sendstr_chunk(req, "</form>");
        
        // Space
        httpd_resp_sendstr_chunk(req, "<br>");

        // Pump 2 adjust
        httpd_resp_sendstr_chunk(req, "<form action=\"setpump2\" method=\"GET\">");
        httpd_resp_sendstr_chunk(req, "<p>Pump 2 duty cycle percent:</p>");
        httpd_resp_sendstr_chunk(req, "<input type=\"number\" id=\"value\" name=\"value\" min=\"0\" max=\"100\" step=\"1\"></input>");
        httpd_resp_sendstr_chunk(req, "<p><input type=\"submit\" value=\"Save\" /></p>");
        httpd_resp_sendstr_chunk(req, "</form>");

        // Space
        httpd_resp_sendstr_chunk(req, "<br>");

        // Cancel cycle
        httpd_resp_sendstr_chunk(req, "<form action=\"cancel\" method=\"GET\">");
        httpd_resp_sendstr_chunk(req, "<p>Cancel cycle:</p>");
        httpd_resp_sendstr_chunk(req, "<p><input type=\"submit\" value=\"Cancel\" /></p>");
        httpd_resp_sendstr_chunk(req, "</form>");
    } else {
        // No etch cycle
        char line1[255];
        snprintf(line1, 255, "Current temperature: %.1f °C", gpio_get_current_temp_c());
        httpd_resp_sendstr_chunk(req, "<p>");
        httpd_resp_sendstr_chunk(req, line1);
        httpd_resp_sendstr_chunk(req, "</p>");
        httpd_resp_sendstr_chunk(req, "<br>");

        // Offer to start etch cycle
        httpd_resp_sendstr_chunk(req, "<h2>Start cycle</h2>");
        httpd_resp_sendstr_chunk(req, "<form action=\"start\" method=\"GET\">");
        httpd_resp_sendstr_chunk(req, "<p>Operation type:</p>");
        httpd_resp_sendstr_chunk(req, "<input type=\"radio\" id=\"operation_feed\" name=\"operation\" value=\"1\">Feed</input>");
        httpd_resp_sendstr_chunk(req, "<input type=\"radio\" id=\"operation_process\" name=\"operation\" value=\"2\" checked>Process</input>");
        httpd_resp_sendstr_chunk(req, "<input type=\"radio\" id=\"operation_drain\" name=\"operation\" value=\"3\">Drain</input>");
        httpd_resp_sendstr_chunk(req, "<p>Target temperature, Celsius:</p>");
        httpd_resp_sendstr_chunk(req, "<input type=\"number\" id=\"tempc\" name=\"tempc\" min=\"25\" max=\"60\" step=\"1\" value=\"25\"></input>");
        httpd_resp_sendstr_chunk(req, "<p>Direction:</p>");
        httpd_resp_sendstr_chunk(req, "<input type=\"radio\" id=\"direction_up\" name=\"direction\" value=\"true\" checked>Up</input>");
        httpd_resp_sendstr_chunk(req, "<input type=\"radio\" id=\"direction_down\" name=\"direction\" value=\"false\">Down</input>");
        httpd_resp_sendstr_chunk(req, "<p>Start speed, mm/sec:</p>");
        httpd_resp_sendstr_chunk(req, "<input type=\"number\" id=\"startspeed\" name=\"startspeed\" min=\"0.01\" max=\"6\" step=\"0.01\" value=\"0.4\"></input>");
        httpd_resp_sendstr_chunk(req, "<p>End speed, mm/sec:</p>");
        httpd_resp_sendstr_chunk(req, "<input type=\"number\" id=\"endspeed\" name=\"endspeed\" min=\"0.01\" max=\"6\" step=\"0.01\" value=\"6\"></input>");
        httpd_resp_sendstr_chunk(req, "<p>Pump 1 speed:</p>");
        httpd_resp_sendstr_chunk(req, "<input type=\"number\" id=\"pump1dc\" name=\"pump1dc\" min=\"0\" max=\"100\" step=\"1\" value=\"95\"></input>");
        httpd_resp_sendstr_chunk(req, "<p>Pump 2 speed:</p>");
        httpd_resp_sendstr_chunk(req, "<input type=\"number\" id=\"pump2dc\" name=\"pump2dc\" min=\"0\" max=\"100\" step=\"1\" value=\"95\"></input>");
        httpd_resp_sendstr_chunk(req, "<p><input type=\"submit\" value=\"Start\" /></p>");
        httpd_resp_sendstr_chunk(req, "</form>");
    }

    // Page end
    httpd_resp_sendstr_chunk(req, "</body></html>\0");
    
    // Send response
    httpd_resp_sendstr_chunk(req, NULL);
    return ESP_OK;
}

esp_err_t start_handler(httpd_req_t *req) {
    char buf[255] = {0};
    char setting_value[32] = {0};
    float tempc = 25.0f;
    float iSpeed = 1.0f;
    float fSpeed = 1.0f;
    int pump1DC = 0;
    int pump2DC = 0;
    int optype = 0;
    bool dirup = false;
    esp_err_t err = httpd_req_get_url_query_str(req, buf, 255);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    err = httpd_query_key_value(buf, "tempc", setting_value, 32);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    tempc = atof(setting_value);
    err = httpd_query_key_value(buf, "operation", setting_value, 32);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    optype = atoi(setting_value);
    err = httpd_query_key_value(buf, "direction", setting_value, 32);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    dirup = (setting_value[0] == '1' || setting_value[0] == 't' || setting_value[0] == 'T');
    err = httpd_query_key_value(buf, "startspeed", setting_value, 32);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    iSpeed = atof(setting_value);
    err = httpd_query_key_value(buf, "endspeed", setting_value, 32);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    fSpeed = atof(setting_value);
    err = httpd_query_key_value(buf, "pump1dc", setting_value, 32);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    pump1DC = atoi(setting_value);
    err = httpd_query_key_value(buf, "pump2dc", setting_value, 32);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    pump2DC = atoi(setting_value);
    
    gpio_start_etch_cycle(optype, dirup, iSpeed, fSpeed, tempc, pump1DC, pump2DC);
    httpd_resp_set_type(req, "text/html");
    httpd_resp_set_status(req, "302 Found");
    httpd_resp_set_hdr(req, "Location", "/");
    httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
    httpd_resp_set_hdr(req, "Connection", "close");
    httpd_resp_send(req, NULL, 0);
    return ESP_OK;
}

esp_err_t cancel_handler(httpd_req_t *req) {
    gpio_cancel_etch_cycle();
    httpd_resp_set_type(req, "text/html");
    httpd_resp_set_status(req, "302 Found");
    httpd_resp_set_hdr(req, "Location", "/");
    httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
    httpd_resp_set_hdr(req, "Connection", "close");
    httpd_resp_send(req, NULL, 0);
    return ESP_OK;
}

esp_err_t setpump1_handler(httpd_req_t *req) {
    char buf[255] = {0};
    esp_err_t err = httpd_req_get_url_query_str(req, buf, 255);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    char setting_value[32] = {0};
    err = httpd_query_key_value(buf, "value", setting_value, 32);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    int new_pump_val = atoi(setting_value);
    if (new_pump_val < 0) new_pump_val = 0;
    if (new_pump_val > 100) new_pump_val = 100;

    gpio_set_pump1_duty_percent(new_pump_val);

    httpd_resp_set_type(req, "text/html");
    httpd_resp_set_status(req, "302 Found");
    httpd_resp_set_hdr(req, "Location", "/");
    httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
    httpd_resp_set_hdr(req, "Connection", "close");
    httpd_resp_send(req, NULL, 0);
    return ESP_OK;
}

esp_err_t setpump2_handler(httpd_req_t *req) {
    char buf[255] = {0};
    esp_err_t err = httpd_req_get_url_query_str(req, buf, 255);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    char setting_value[32] = {0};
    err = httpd_query_key_value(buf, "value", setting_value, 32);
    if (err != ESP_OK) {
        httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
        httpd_resp_set_hdr(req, "Connection", "close");
        httpd_resp_sendstr(req, "BAD_REQUEST");
        return err;
    }
    int new_pump_val = atoi(setting_value);
    if (new_pump_val < 0) new_pump_val = 0;
    if (new_pump_val > 100) new_pump_val = 100;

    gpio_set_pump2_duty_percent(new_pump_val);

    httpd_resp_set_type(req, "text/html");
    httpd_resp_set_status(req, "302 Found");
    httpd_resp_set_hdr(req, "Location", "/");
    httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
    httpd_resp_set_hdr(req, "Connection", "close");
    httpd_resp_send(req, NULL, 0);
    return ESP_OK;
}

esp_err_t reboot_handler(httpd_req_t *req) {
    httpd_resp_set_type(req, "text/html");
    httpd_resp_set_status(req, "302 Found");
    httpd_resp_set_hdr(req, "Location", "/");
    httpd_resp_set_hdr(req, "Cache-Control", "no-cache");
    httpd_resp_set_hdr(req, "Connection", "close");
    httpd_resp_send(req, NULL, 0);
    esp_restart();
    return ESP_OK;
}

httpd_handle_t http_server_start(void) {
    httpd_config_t config = HTTPD_DEFAULT_CONFIG();
    config.lru_purge_enable = true;
    httpd_handle_t server = NULL;
    if (httpd_start(&server, &config) == ESP_OK) {
        static const httpd_uri_t main_uri = {
            .uri = "/",
            .method = HTTP_GET,
            .handler = main_handler,
            .user_ctx = NULL
        };
        static const httpd_uri_t reboot_uri = {
            .uri = "/reboot",
            .method = HTTP_GET,
            .handler = reboot_handler,
            .user_ctx = NULL
        };
        static const httpd_uri_t setpump1_uri = {
            .uri = "/setpump1",
            .method = HTTP_GET,
            .handler = setpump1_handler,
            .user_ctx = NULL
        };
        static const httpd_uri_t setpump2_uri = {
            .uri = "/setpump2",
            .method = HTTP_GET,
            .handler = setpump2_handler,
            .user_ctx = NULL
        };
        static const httpd_uri_t cancel_uri = {
            .uri = "/cancel",
            .method = HTTP_GET,
            .handler = cancel_handler,
            .user_ctx = NULL
        };
        static const httpd_uri_t start_uri = {
            .uri = "/start",
            .method = HTTP_GET,
            .handler = start_handler,
            .user_ctx = NULL
        };
        httpd_register_uri_handler(server, &main_uri);
        httpd_register_uri_handler(server, &reboot_uri);
        httpd_register_uri_handler(server, &setpump1_uri);
        httpd_register_uri_handler(server, &setpump2_uri);
        httpd_register_uri_handler(server, &cancel_uri);
        httpd_register_uri_handler(server, &start_uri);
    }
    return server;
}
