/*
 *  ArduinoHub.h
 *
 *  Copyright (C)
 *  Honda Research Institute Europe GmbH
 *  Carl-Legien-Str. 30
 *  63073 Offenbach/Main
 *  Germany
 *
 *  UNPUBLISHED PROPRIETARY MATERIAL.
 *  ALL RIGHTS RESERVED.
 *
 */

#pragma once
#include <pthread.h>
#include <stdbool.h>
#include <stdint.h>
#include <time.h>
#include <windows.h>
#include "Serial.h"
#include "Actuator.h"

typedef enum
{
    ARDUINOHUB_TYPE_PIEZO = 'P',
    ARDUINOHUB_TYPE_ERM   = 'E',
    ARDUINOHUB_TYPE_EMS   = 'M'
} ArduinoHub_Type;

typedef struct ArduinoHub
{
    bool valid;
    pthread_mutex_t mutex;
    pthread_t thread;
    ArduinoHub_Type arduinoType;
    Serial* serial;
    Actuator* actuators;
} ArduinoHub;

ArduinoHub* ArduinoHub_new();
void ArduinoHub_init(ArduinoHub* self, char* port, ArduinoHub_Type type);
void ArduinoHub_clear(ArduinoHub* self);
void ArduinoHub_delete(ArduinoHub* self);


/**
 * Starts vibration for a specified duration. The command is non-blocking.
 * @intensity:
 *      Intensity covering the whole spectrum of the actuator if no range is set.
 *      An intensity range can be set with ArduinoHub_setIntensityRange().
 * @duration:
 *      Duration of the vibration in milliseconds.
 */
void ArduinoHub_vibrate(ArduinoHub* self, uint8_t tactor, float intensity, uint64_t duration);

/**
 * Starts continuous vibration. The command is non-blocking.
 * @intensity:
 *      Intensity covering the whole spectrum of the actuator if no range is set.
 *      An intensity range can be set with ArduinoActuator_setIntensityRange().
 */
void ArduinoHub_startVibration(ArduinoHub* self, uint8_t tactor, float intensity);

/**
 * Stops continuous vibration.
 */
void ArduinoHub_stopVibration(ArduinoHub* self, uint8_t tactor);

/**
* Sets the frequency of the vibration of all piezo actuators.
* No effect for ERM and EMS.
*/
void ArduinoHub_setFrequency(ArduinoHub* self, uint8_t tactor, uint16_t frequency);

/**
 * Set the intensity range. The default range is the actuators whole intensity [0, 1].
 * Modifying this setting cuts-out intensity values under min and above max.
 * E.g., for the range [0.2, 1.0] the intensity 0.5 maps to a device intensity of 0.6.
 */
void ArduinoHub_setIntensityRange(ArduinoHub* self, uint8_t tactor, float minIntensity, float maxIntensity);