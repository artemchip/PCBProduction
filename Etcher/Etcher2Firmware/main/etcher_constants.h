#include <stdio.h>
#include <esp_log.h>
#include <esp_system.h>
#include <nvs_flash.h>
#include <sys/param.h>
#include <string.h>
#include <esp_http_server.h>
#include "esp_event.h"
#include "esp_netif.h"
#include <esp_wifi.h>
#include "esp_timer.h"
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "esp_tls.h"
#include "esp_ota_ops.h"
#include "esp_mac.h"
#include "freertos/event_groups.h"
#include "esp_netif_sntp.h"
#include "lwip/ip_addr.h"
#include "esp_sntp.h"
#include "driver/gpio.h"
#include "driver/ledc.h"
#include <rom/ets_sys.h>
#include "driver/spi_master.h"
#include <math.h>
#include "esp_adc/adc_oneshot.h"
#include "esp_adc/adc_cali.h"
#include "esp_adc/adc_cali_scheme.h"

#pragma once

#define ETCHER_WIFI_SSID "ArtemChip-H24"
#define ETCHER_WIFI_PASSWORD "secret"

#define HEATER_EN_PIN 32

#define THERMAL1U_ADC_PIN 36
#define THERMAL2U_ADC_PIN 39
#define THERMAL3_ADC_PIN 34
#define THERMAL3_ADC_CHANNEL ADC_CHANNEL_6
#define THERMAL4_ADC_PIN 35
#define THERMAL4_ADC_CHANNEL ADC_CHANNEL_7

#define PUMP1_PIN 33
#define PUMP2_PIN 25

#define ACTUATOR_STEP_PIN 19
#define ACTUATOR_DIR_PIN 21

#define DOWN_LIMIT_SW_PIN 22
#define UP_LIMIT_SW_PIN 23

#define FEED_VALVE_PIN 26
#define UNUSED_VALVE_PIN 27
#define DRAIN_VALVE_PIN 5

#define FEED_TIME_SEC 50
#define DRAIN_TIME_SEC 30

#define STEPS_PER_MM 3160
#define MAX_POSITION_MM 120

// R/T 8016 characteristic https://magazin-elektronika.ru/uploads/PDF/uploads/B57861S0103F040_info.pdf