#include "etcher_constants.h"

#pragma once
void gpio_init(void);
bool adc_calibration_init(adc_unit_t unit, adc_channel_t channel, adc_atten_t atten, adc_cali_handle_t *out_handle);
void adc_calibration_deinit(adc_cali_handle_t handle);
float adc_measure_temperature(void);
float adc_measure_avg_temperature(int count);
void gpio_destroy(void);
void gpio_main_loop(void);
void gpio_start_etch_cycle(int opType, bool dirUp, float mmPerSecStart, float mmPerSecEnd, float tgtTempC, int p1Percent, int p2Percent);
void gpio_set_pump1_duty_percent(int percent);
void gpio_set_pump2_duty_percent(int percent);
void gpio_cancel_etch_cycle(void);
int gpio_get_operation_type(void);
bool gpio_get_move_direction_up(void);
float gpio_get_start_speed_mm_sec(void);
float gpio_get_end_speed_mm_sec(void);
int gpio_get_pump1_duty_percent(void);
int gpio_get_pump2_duty_percent(void);
float gpio_get_current_speed_mm_sec(void);
float gpio_get_current_temp_c(void);
float gpio_get_current_position_mm(void);
float gpio_get_target_temp_c(void);