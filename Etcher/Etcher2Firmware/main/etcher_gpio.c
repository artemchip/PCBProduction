#include "etcher_gpio.h"

// All GPIO: pumps, thermal loop, stepper feed loop
const char* GPIO_TAG = "Etcher-PumpControl";
spi_device_handle_t spi;

int operationType = 0;
bool directionUp = true;
float startMmPerSec = 0.0f;
float endMmPerSec = 0.0f;
float targetTempC = 25.0f;

int pump1DutyPercent = 0;
int pump2DutyPercent = 0;
bool shouldCancelCycle = false;

float curSpeedMmPerSec = 0.0f;
float curTempReadingC = 25.0f;
float curPositionMm = 0.0f;

// ADC
adc_oneshot_unit_handle_t adc1_handle;
adc_cali_handle_t adc1_cali_chan0_handle = NULL;
adc_cali_handle_t adc1_cali_chan1_handle = NULL;
bool do_calibration1_chan0 = false;
bool do_calibration1_chan1 = false;

void gpio_init(void) {
    // GPIO
    gpio_set_direction(HEATER_EN_PIN, GPIO_MODE_OUTPUT);
    gpio_set_direction(THERMAL1U_ADC_PIN, GPIO_MODE_INPUT);
    gpio_set_direction(THERMAL2U_ADC_PIN, GPIO_MODE_INPUT);
    gpio_set_direction(THERMAL3_ADC_PIN, GPIO_MODE_INPUT);
    gpio_set_direction(THERMAL4_ADC_PIN, GPIO_MODE_INPUT);
    gpio_set_direction(PUMP1_PIN, GPIO_MODE_OUTPUT);
    gpio_set_direction(PUMP2_PIN, GPIO_MODE_OUTPUT);
    gpio_set_direction(ACTUATOR_STEP_PIN, GPIO_MODE_OUTPUT);
    gpio_set_direction(ACTUATOR_DIR_PIN, GPIO_MODE_OUTPUT);
    gpio_set_direction(DOWN_LIMIT_SW_PIN, GPIO_MODE_INPUT);
    gpio_set_direction(UP_LIMIT_SW_PIN, GPIO_MODE_INPUT);
    gpio_set_direction(FEED_VALVE_PIN, GPIO_MODE_OUTPUT);
    gpio_set_direction(UNUSED_VALVE_PIN, GPIO_MODE_OUTPUT);
    gpio_set_direction(DRAIN_VALVE_PIN, GPIO_MODE_OUTPUT);
    gpio_set_level(HEATER_EN_PIN, 0);
    gpio_set_level(PUMP1_PIN, 0);
    gpio_set_level(PUMP2_PIN, 0);
    gpio_set_level(ACTUATOR_STEP_PIN, 0);
    gpio_set_level(ACTUATOR_DIR_PIN, 0);
    gpio_set_level(FEED_VALVE_PIN, 0);
    gpio_set_level(UNUSED_VALVE_PIN, 0);
    gpio_set_level(DRAIN_VALVE_PIN, 0);

    // PWM
    ledc_timer_config_t ledc_timer = {
        .speed_mode       = LEDC_LOW_SPEED_MODE,
        .duty_resolution  = LEDC_TIMER_10_BIT,
        .timer_num        = LEDC_TIMER_0,
        .freq_hz          = 1000,
        .clk_cfg          = LEDC_AUTO_CLK
    };
    ESP_ERROR_CHECK(ledc_timer_config(&ledc_timer));
    ledc_channel_config_t ledc_channel_0 = {
        .speed_mode     = LEDC_LOW_SPEED_MODE,
        .channel        = LEDC_CHANNEL_0,
        .timer_sel      = LEDC_TIMER_0,
        .intr_type      = LEDC_INTR_DISABLE,
        .gpio_num       = PUMP1_PIN,
        .duty           = 0,
        .hpoint         = 0
    };
    ESP_ERROR_CHECK(ledc_channel_config(&ledc_channel_0));
    ledc_channel_config_t ledc_channel_1 = {
        .speed_mode     = LEDC_LOW_SPEED_MODE,
        .channel        = LEDC_CHANNEL_1,
        .timer_sel      = LEDC_TIMER_0,
        .intr_type      = LEDC_INTR_DISABLE,
        .gpio_num       = PUMP2_PIN,
        .duty           = 0,
        .hpoint         = 0
    };
    ESP_ERROR_CHECK(ledc_channel_config(&ledc_channel_1));

    // ADC (thermal) - init
    adc_oneshot_unit_init_cfg_t init_config1 = {
        .unit_id = ADC_UNIT_1,
    };
    ESP_ERROR_CHECK(adc_oneshot_new_unit(&init_config1, &adc1_handle));

    // ADC (thermal) - config
    adc_oneshot_chan_cfg_t config = {
        .atten = ADC_ATTEN_DB_12,
        .bitwidth = ADC_BITWIDTH_DEFAULT,
    };
    ESP_ERROR_CHECK(adc_oneshot_config_channel(adc1_handle, THERMAL3_ADC_CHANNEL, &config));
    ESP_ERROR_CHECK(adc_oneshot_config_channel(adc1_handle, THERMAL4_ADC_CHANNEL, &config));

    // ADC (thermal) - calibrate
    do_calibration1_chan0 = adc_calibration_init(ADC_UNIT_1, THERMAL3_ADC_CHANNEL, ADC_ATTEN_DB_12, &adc1_cali_chan0_handle);
    do_calibration1_chan1 = adc_calibration_init(ADC_UNIT_1, THERMAL4_ADC_CHANNEL, ADC_ATTEN_DB_12, &adc1_cali_chan1_handle);
}

bool adc_calibration_init(adc_unit_t unit, adc_channel_t channel, adc_atten_t atten, adc_cali_handle_t *out_handle)
{
    adc_cali_handle_t handle = NULL;
    esp_err_t ret = ESP_FAIL;
    bool calibrated = false;

#if ADC_CALI_SCHEME_CURVE_FITTING_SUPPORTED
    if (!calibrated) {
        adc_cali_curve_fitting_config_t cali_config = {
            .unit_id = unit,
            .chan = channel,
            .atten = atten,
            .bitwidth = ADC_BITWIDTH_DEFAULT,
        };
        ret = adc_cali_create_scheme_curve_fitting(&cali_config, &handle);
        if (ret == ESP_OK) {
            calibrated = true;
        }
    }
#endif

#if ADC_CALI_SCHEME_LINE_FITTING_SUPPORTED
    if (!calibrated) {
        adc_cali_line_fitting_config_t cali_config = {
            .unit_id = unit,
            .atten = atten,
            .bitwidth = ADC_BITWIDTH_DEFAULT,
        };
        ret = adc_cali_create_scheme_line_fitting(&cali_config, &handle);
        if (ret == ESP_OK) {
            calibrated = true;
        }
    }
#endif

    *out_handle = handle;

    return calibrated;
}

void adc_calibration_deinit(adc_cali_handle_t handle)
{
#if ADC_CALI_SCHEME_CURVE_FITTING_SUPPORTED
    ESP_ERROR_CHECK(adc_cali_delete_scheme_curve_fitting(handle));

#elif ADC_CALI_SCHEME_LINE_FITTING_SUPPORTED
    ESP_ERROR_CHECK(adc_cali_delete_scheme_line_fitting(handle));
#endif
}

float adc_measure_temperature(void) {
    // Read ADC voltage
    int adcRaw1 = 0;
    int voltage1MV = 0;
    int adcRaw2 = 0;
    int voltage2MV = 0;
    const int halfVoltageMV = 1650;
    ESP_ERROR_CHECK(adc_oneshot_read(adc1_handle, THERMAL3_ADC_CHANNEL, &adcRaw1));
    if (do_calibration1_chan0) {
        ESP_ERROR_CHECK(adc_cali_raw_to_voltage(adc1_cali_chan0_handle, adcRaw1, &voltage1MV));
    } else {
        return 100.0f;
    }
    vTaskDelay(pdMS_TO_TICKS(300));
    ESP_ERROR_CHECK(adc_oneshot_read(adc1_handle, THERMAL4_ADC_CHANNEL, &adcRaw2));
    if (do_calibration1_chan1) {
        ESP_ERROR_CHECK(adc_cali_raw_to_voltage(adc1_cali_chan1_handle, adcRaw2, &voltage2MV));
    } else {
        return 100.0f;
    }
    vTaskDelay(pdMS_TO_TICKS(300));

    // Full voltage (sensor not connected)
    if (voltage1MV >= 2450 || voltage2MV >= 2450) {
        return 100.0f;
    }

    // Convert voltage to resistance
    float avgVoltageMV = (float)(voltage1MV + voltage2MV) / 2.0f;
    float avgResistanceToGroundOhms = ((10000.0f * avgVoltageMV) / (float)halfVoltageMV);
    float avgResistanceToNominalRatio = (avgResistanceToGroundOhms / 10000.0f);
    
    // Resistance to temperature
    const float RT[20] = {
        3.265f, // 0C
        2.539f,
        1.99f,
        1.571f,
        1.249f,
        1.0000f, // 25C
        0.8057f,
        0.6531f,
        0.5327f,
        0.4369f,
        0.3603f,
        0.2986f,
        0.2488f,
        0.2083f,
        0.1752f,
        0.1481f,
        0.1258f,
        0.1072f,
        0.09177f,
        0.07885f // 95C
    };
    if (avgResistanceToNominalRatio >= RT[0]) {
        return 0.0f;
    } else if (avgResistanceToNominalRatio <= RT[19]) {
        return 95.0f;
    }
    for (int i = 1; i < 20; i++) {
        if (avgResistanceToNominalRatio <= RT[i - 1] && avgResistanceToNominalRatio >= RT[i]) {
            float OldMin = RT[i];
            float OldMax = RT[i - 1];
            float NewValueDegrees = ((avgResistanceToNominalRatio - OldMin) / (OldMax - OldMin)) * 5.0f;
            return ((((float)i) * 5.0f) - NewValueDegrees);
        }
    }
    return 100.0f;
}

float adc_measure_avg_temperature(int count) {
    float avg_temp = 0.0f;
    for (int i = 0; i < count; i++) {
        float this_reading = adc_measure_temperature();
        avg_temp += (this_reading / ((float)count));
    }
    return avg_temp;
}

void gpio_destroy(void) {
    ESP_ERROR_CHECK(adc_oneshot_del_unit(adc1_handle));
    if (do_calibration1_chan0) {
        adc_calibration_deinit(adc1_cali_chan0_handle);
    }
    if (do_calibration1_chan1) {
        adc_calibration_deinit(adc1_cali_chan1_handle);
    }
}

void gpio_main_loop(void) {
    while (true) {
        if (startMmPerSec > 0.0f && endMmPerSec > 0.0f && operationType > 0) {
            if (operationType == 1) {
                // Feed
                gpio_set_level(FEED_VALVE_PIN, 1);
                for (int i = 0; i < (FEED_TIME_SEC * 10); i++) {
                    vTaskDelay(pdMS_TO_TICKS(100));
                    if (shouldCancelCycle) {
                        break;
                    }
                }
                gpio_set_level(FEED_VALVE_PIN, 0);
                pump1DutyPercent = 0;
                pump2DutyPercent = 0;
                curPositionMm = 0.0f;
                curSpeedMmPerSec = 0.0f;
                startMmPerSec = 0.0f;
                endMmPerSec = 0.0f;
                directionUp = true;
                targetTempC = 25.0f;
                operationType = 0;
                shouldCancelCycle = false;
            } else if (operationType == 2) {
                // Process
                for (int i = 0; i < MAX_POSITION_MM; i++) {
                    // Manage temperature
                    if (targetTempC > 25.0f && (i % 5 == 0)) {
                        // Measure it
                        curTempReadingC = adc_measure_avg_temperature(5);

                        // Cancel
                        if (shouldCancelCycle) {
                            gpio_set_level(HEATER_EN_PIN, 0);
                            break;
                        }

                        // Is it enough - heat it
                        float prevTempReading = 0.0f;
                        while (curTempReadingC < targetTempC) {
                            // Turn on heat & stir
                            gpio_set_level(HEATER_EN_PIN, 1);
                            ledc_set_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_0, 680);
                            ledc_update_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_0);
                            ledc_set_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_1, 680);
                            ledc_update_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_1);

                            // Wait until it heats up
                            for (int j = 0; j < 10; j++) {
                                vTaskDelay(pdMS_TO_TICKS(500));
                                if (shouldCancelCycle) break;
                            }

                            // Turn off heat & stir
                            gpio_set_level(HEATER_EN_PIN, 0);
                            ledc_set_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_0, 0);
                            ledc_update_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_0);
                            ledc_set_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_1, 0);
                            ledc_update_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_1);

                            // Wait for inertia
                            for (int j = 0; j < 15; j++) {
                                vTaskDelay(pdMS_TO_TICKS(500));
                                if (shouldCancelCycle) break;
                            }

                            // Measure again
                            curTempReadingC = adc_measure_avg_temperature(5);
                            if (curTempReadingC <= prevTempReading) {
                                // We heat, but the temperature does not increase, fail
                                shouldCancelCycle = true;
                                break;
                            }
                            prevTempReading = curTempReadingC + 0.0f;
                        }

                        // Fail-safe
                        gpio_set_level(HEATER_EN_PIN, 0);
                    }

                    // Spray
                    gpio_set_level(HEATER_EN_PIN, 0);
                    int dutyInt1 = (int)(1024.0f * ((float)pump1DutyPercent / 100.0f));
                    int dutyInt2 = (int)(1024.0f * ((float)pump2DutyPercent / 100.0f));
                    ledc_set_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_0, dutyInt1);
                    ledc_update_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_0);
                    ledc_set_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_1, dutyInt2);
                    ledc_update_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_1);
                    if (shouldCancelCycle) break;

                    // Calculate current movement speed
                    curPositionMm = i;
                    curSpeedMmPerSec = startMmPerSec + ((endMmPerSec - startMmPerSec) * (curPositionMm / (float)MAX_POSITION_MM));

                    // Move a millimiter
                    gpio_set_level(ACTUATOR_DIR_PIN, directionUp ? 0 : 1);
                    int delayMicrosec = (int)(1000000.0f / (curSpeedMmPerSec * (float)STEPS_PER_MM));
                    for (int st = 0; st < STEPS_PER_MM; st++) {
                        gpio_set_level(ACTUATOR_STEP_PIN, 1);
                        ets_delay_us(delayMicrosec);
                        gpio_set_level(ACTUATOR_STEP_PIN, 0);
                        ets_delay_us(delayMicrosec);

                        // Check limit switch
                        bool isLimitSW1 = gpio_get_level(UP_LIMIT_SW_PIN) && directionUp;
                        bool isLimitSW2 = gpio_get_level(DOWN_LIMIT_SW_PIN) && !directionUp;
                        if (isLimitSW1 || isLimitSW2) {
                            shouldCancelCycle = true;
                            break;
                        }

                        // Check cancel
                        if (shouldCancelCycle) break;
                    }

                    // Handle cycle cancellation
                    if (shouldCancelCycle) break;
                }

                // Reset when complete and wait for next command
                gpio_set_level(HEATER_EN_PIN, 0);
                ledc_set_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_0, 0);
                ledc_update_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_0);
                ledc_set_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_1, 0);
                ledc_update_duty(LEDC_LOW_SPEED_MODE, LEDC_CHANNEL_1);
                pump1DutyPercent = 0;
                pump2DutyPercent = 0;
                curPositionMm = 0.0f;
                curSpeedMmPerSec = 0.0f;
                startMmPerSec = 0.0f;
                endMmPerSec = 0.0f;
                directionUp = true;
                targetTempC = 25.0f;
                operationType = 0;
                shouldCancelCycle = false;
            } else {
                // Drain
                gpio_set_level(DRAIN_VALVE_PIN, 1);
                for (int i = 0; i < (DRAIN_TIME_SEC * 10); i++) {
                    vTaskDelay(pdMS_TO_TICKS(100));
                    if (shouldCancelCycle) {
                        break;
                    }
                }
                gpio_set_level(DRAIN_VALVE_PIN, 0);
                pump1DutyPercent = 0;
                pump2DutyPercent = 0;
                curPositionMm = 0.0f;
                curSpeedMmPerSec = 0.0f;
                startMmPerSec = 0.0f;
                endMmPerSec = 0.0f;
                directionUp = true;
                targetTempC = 25.0f;
                operationType = 0;
                shouldCancelCycle = false;
            }
        } else {
            // Wait for command
            curTempReadingC = adc_measure_avg_temperature(5);
        }
    }
}

void gpio_start_etch_cycle(int opType, bool dirUp, float mmPerSecStart, float mmPerSecEnd, float tgtTempC, int p1Percent, int p2Percent) {
    if (opType == 1 || opType == 3) {
        mmPerSecStart = 1.0f;
        mmPerSecEnd = 1.0f;
    }
    if (mmPerSecStart < 0.01f) mmPerSecStart = 0.01f;
    if (mmPerSecStart > 6.0f) mmPerSecStart = 6.0f;
    if (mmPerSecEnd < 0.01f) mmPerSecEnd = 0.01f;
    if (mmPerSecEnd > 6.0f) mmPerSecEnd = 6.0f;
    if (tgtTempC < 25.0f) tgtTempC = 25.0f;
    if (tgtTempC > 60.0f) tgtTempC = 60.0f;
    if (p1Percent < 0) p1Percent = 0;
    if (p1Percent > 100) p1Percent = 100;
    if (p2Percent < 0) p2Percent = 0;
    if (p2Percent > 100) p2Percent = 100;
    curPositionMm = 0.0f;
    curSpeedMmPerSec = 0.0f;
    shouldCancelCycle = false;
    directionUp = dirUp;
    targetTempC = tgtTempC;
    pump1DutyPercent = p1Percent;
    pump2DutyPercent = p2Percent;
    operationType = opType;
    startMmPerSec = mmPerSecStart;
    endMmPerSec = mmPerSecEnd;
}

void gpio_set_pump1_duty_percent(int percent) {
    if (percent < 0) percent = 0;
    if (percent > 100) percent = 100;
    pump1DutyPercent = percent;
}

void gpio_set_pump2_duty_percent(int percent) {
    if (percent < 0) percent = 0;
    if (percent > 100) percent = 100;
    pump2DutyPercent = percent;
}

void gpio_cancel_etch_cycle(void) {
    shouldCancelCycle = true;
}

int gpio_get_operation_type(void) {
    return operationType;
}

bool gpio_get_move_direction_up(void) {
    return directionUp;
}

float gpio_get_start_speed_mm_sec(void) {
    return startMmPerSec;
}

float gpio_get_end_speed_mm_sec(void) {
    return endMmPerSec;
}

int gpio_get_pump1_duty_percent(void) {
    return pump1DutyPercent;
}

int gpio_get_pump2_duty_percent(void) {
    return pump2DutyPercent;
}

float gpio_get_current_speed_mm_sec(void) {
    return curSpeedMmPerSec;
}

float gpio_get_current_temp_c(void) {
    return curTempReadingC;
}

float gpio_get_current_position_mm(void) {
    return curPositionMm;
}

float gpio_get_target_temp_c(void) {
    return targetTempC;
}
