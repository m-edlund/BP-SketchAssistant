/*
 *  Actuator.h
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

typedef struct Actuator
{
    bool active;
    bool continuous;
    clock_t endTime;
    float intensity;
    float minIntensity; // Cut off values under threshold (default: 0)
    float maxIntensity; // Cut off values over threshold  (default: 1)
    uint16_t frequency; // 250Hz, for Pacinian corpuscles use 200-300Hz
} Actuator;

void Actuator_setModeOnce(Actuator* actuator, float intensity, uint64_t duration);
void Actuator_setModeContinuous(Actuator* actuator, float intensity);
void Actuator_setModeStop(Actuator* actuator);
