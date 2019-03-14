#include "stdafx.h"
extern "C" {
#include "MotorHeader/BodyActuator.h"
}
#include <ctime>
#include <stdio.h>
#include "ArmbandInterface.h"


extern "C" {
		__declspec(dllexport) int __cdecl ArmbandInterface::setupArmband() {
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
			//strcpy(port, "COM5");
			setupMotors();
			startVibrate(0, 1.0);
			return -1;
		}
		__declspec(dllexport) void __cdecl ArmbandInterface::startVibrate(int tactor, float intensity) {
			(startFunctionHandle)(armband, (uint8_t)tactor, intensity);
		}
		__declspec(dllexport) void __cdecl ArmbandInterface::stopVibrate(int tactor) {
			(stopFunctionHandle)(armband, (uint8_t)tactor);
		}
	}

 void ArmbandInterface::setupMotors() {
			(initFunctionHandle) (armband, BODYACTUATOR_TYPE_EAI, new char[5]{ 'C', 'O', 'M', '5', '\0' }, 8);
			printf("armband initialized");
		}
 
