/*

 *  BodyActuator.h

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

#include "ArduinoHub.h"

#include "EAIHub.h"

typedef enum

{

    BODYACTUATOR_TYPE_NONE,

    BODYACTUATOR_TYPE_EAI,

    BODYACTUATOR_TYPE_PIEZO,

    BODYACTUATOR_TYPE_ERM,

    BODYACTUATOR_TYPE_EMS

} BodyActuator_Type;

extern char* BodyActuator_Type_Names[5];

typedef struct BodyActuator

{

    bool valid;

    uint8_t actuatorCount;

    BodyActuator_Type type;

    ArduinoHub* arduinoHub;

    EAIHub* eaiHub;

} BodyActuator;

BodyActuator* BodyActuator_new();

void BodyActuator_init(BodyActuator* self, BodyActuator_Type type, char* port, uint8_t actuatorCount);

void BodyActuator_clear(BodyActuator* self);

void BodyActuator_delete(BodyActuator* self);

/**

 * Starts actuation for a specified duration. The command is non-blocking.

 * @intensity:

 *      Intensity covering the whole spectrum of the actuator if no range is set.

 *      An intensity range can be set with BodyActuator_setIntensityRange().

 * @duration:

 *      Duration of the vibration in milliseconds.

 */

void BodyActuator_actuate(BodyActuator* self, uint8_t tactor, double intensity, uint64_t duration);

/**

 * Starts continuous actuation. The command is non-blocking.

 * @intensity:

 *      Intensity covering the whole spectrum of the actuator if no range is set.

 *      An intensity range can be set with ArduinoActuator_setIntensityRange().

 */

void BodyActuator_startActuation(BodyActuator* self, uint8_t tactor, double intensity);

/**

 * Stops continuous vibration.

 */

void BodyActuator_stopActuation(BodyActuator* self, uint8_t tactor);

/**

* Sets the frequency of the vibration of an actuator.

*/

void BodyActuator_setFrequency(BodyActuator* self, uint8_t tactor, uint16_t frequency);

/**

 * Set the intensity range. The default range is the actuators whole intensity [0, 1].

 * Modifying this setting cuts-out intensity values under min and above max.

 * E.g., for the range [0.2, 1.0] the intensity 0.5 maps to a device intensity of 0.6.

 */

void BodyActuator_setIntensityRange(BodyActuator* self, uint8_t tactor, double minIntensity, double maxIntensity);