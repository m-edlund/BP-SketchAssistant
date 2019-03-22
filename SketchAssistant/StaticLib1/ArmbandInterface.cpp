#include "stdafx.h"
extern "C" {
#include "MotorHeader/BodyActuator.h"
}
#include <ctime>
#include <stdio.h>
#include "ArmbandInterface.h"
#include <stdlib.h>

typedef void(__cdecl *InitFunctionType)(BodyActuator*, BodyActuator_Type, char*, int);
typedef void(__cdecl *StopFunctionType)(BodyActuator*, uint8_t);
typedef void(__cdecl *StartFunctionType)(BodyActuator*, uint8_t, float);
typedef void(__cdecl *ActuateFunctionType)(BodyActuator*, uint8_t, double, uint64_t);
typedef void(__cdecl *DeleteFunctionType)(BodyActuator*);

static InitFunctionType initFunctionHandle;
static StartFunctionType startFunctionHandle;
static StopFunctionType stopFunctionHandle;
static ActuateFunctionType actuateFunctionHandle;
static DeleteFunctionType deleteFunctionHandle;

static BodyActuator* armband;
//static char *port = 

static HINSTANCE lib;

extern "C" {
	DllExport int __cdecl ArmbandInterface::setupArmband() {
			lib = LoadLibrary(TEXT("BodyActuator.dll"));
			if (lib == NULL) {
				printf("ERROR: library could not be loaded");
				return 0;
			}
			initFunctionHandle = (InitFunctionType)GetProcAddress(lib, "BodyActuator_init");
			if (initFunctionHandle == NULL) {
				printf("ERROR: init function could not be retrieved");
				return 1;
			}
			startFunctionHandle = (StartFunctionType)GetProcAddress(lib, "BodyActuator_startActuation");
			if (startFunctionHandle == NULL) {
				printf("ERROR: start function could not be retrieved");
				return 2;
			}
			stopFunctionHandle = (StopFunctionType)GetProcAddress(lib, "BodyActuator_stopActuation");
			if (stopFunctionHandle == NULL) {
				printf("ERROR: stop function could not be retrieved");
				return 3;
			}
			actuateFunctionHandle = (ActuateFunctionType)GetProcAddress(lib, "BodyActuator_actuate");
			if (actuateFunctionHandle == NULL) {
				printf("ERROR: actuate function could not be retrieved");
				return 4;
			}
			deleteFunctionHandle = (DeleteFunctionType)GetProcAddress(lib, "BodyActuator_delete");
			if (deleteFunctionHandle == NULL) {
				printf("ERROR: delete function could not be retrieved");
				return 5;
			}
			//strcpy(port, "COM5");
			setupMotors();
			//startVibrate(0, 1.0);
			return -1;
		}

		DllExport void __cdecl ArmbandInterface::startVibrate(int tactor, float intensity) {
			(startFunctionHandle)(armband, (uint8_t)tactor, intensity);
		}

		DllExport void __cdecl ArmbandInterface::stopVibrate(int tactor) {
			(stopFunctionHandle)(armband, (uint8_t)tactor);
		}
		
		DllExport void __cdecl ArmbandInterface::actuate(int tactor, double intensity, int duration) {
			(actuateFunctionHandle)(armband, (uint8_t)tactor, intensity, (uint64_t)duration);
		}
	}

 void ArmbandInterface::setupMotors() {
	 char* port = (char*) "COM5";//malloc(7);
	 armband = (BodyActuator*) malloc(sizeof(BodyActuator*));
	 //strcpy_s(port, "COM5");
			(initFunctionHandle) (armband, BODYACTUATOR_TYPE_EAI, port, 8);
			printf("armband initialized");
		}

 void ArmbandInterface::deleteArmband() {
	 deleteFunctionHandle(armband);
 }
