/*
 *  EAIHub.h
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
#include <stdbool.h>
#include <stdint.h>
#include <time.h>
#include <pthread.h>
#include "Actuator.h"

typedef struct EAIHub
{
    bool valid;
    pthread_mutex_t mutex;
    pthread_t thread;
    Actuator* actuators;
    uint16_t tactorType;
    uint16_t modulation; // Default: 250Hz
    int deviceID;
} EAIHub;

EAIHub* EAIHub_new();
void EAIHub_init(EAIHub* self, char* eaiPort);
void EAIHub_clear(EAIHub* self);
void EAIHub_delete(EAIHub* self);

/**
 * Starts vibration for a specified duration. The command is non-blocking.
 * @intensity:
 *      Intensity covering the whole spectrum of the actuator if no range is set.
 *      An intensity range can be set with EAIHub_setIntensityRange().
 * @duration:
 *      Duration of the vibration in milliseconds.
 */
void EAIHub_vibrate(EAIHub* self, uint8_t tactor, float intensity, uint64_t duration);

/**
 * Starts continuous vibration. The command is non-blocking.
 * @intensity:
 *      Intensity covering the whole spectrum of the actuator if no range is set.
 *      An intensity range can be set with ArduinoActuator_setIntensityRange().
 */
void EAIHub_startVibration(EAIHub* self, uint8_t tactor, float intensity);

/**
 * Stops continuous vibration.
 */
void EAIHub_stopVibration(EAIHub* self, uint8_t tactor);

/**
* Sets the frequency of the vibration of an actuator.
*/
void EAIHub_setFrequency(EAIHub* self, uint8_t tactor, uint16_t frequency);
/**
 * Set the intensity range. The default range is the actuators whole intensity [0, 1].
 * Modifying this setting cuts-out intensity values under min and above max.
 * E.g., for the range [0.2, 1.0] the intensity 0.5 maps to a device intensity of 0.6.
 */
void EAIHub_setIntensityRange(EAIHub* self, uint8_t tactor, float minIntensity, float maxIntensity);