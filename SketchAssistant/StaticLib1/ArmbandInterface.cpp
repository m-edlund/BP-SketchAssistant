#include "stdafx.h"
extern "C" {
#include "MotorHeader/BodyActuator.h"
}
#include <ctime>
#include <stdio.h>


namespace StaticLib1
{		
	class UnitTest1
	{

		BodyActuator* armband;
		char *port = new char[5] {'C', 'O', 'M', '5', '\0'};

		HINSTANCE lib;
		typedef void (__cdecl *InitFunctionType)(BodyActuator*, BodyActuator_Type, char*, int);
		InitFunctionType initFunctionHandle;
		typedef void(__cdecl *StartFunctionType)(BodyActuator*, uint8_t, float);
		StartFunctionType startFunctionHandle;
		typedef void(__cdecl *StopFunctionType)(BodyActuator*, uint8_t);
		StopFunctionType stopFunctionHandle;

	public : 



		int main() {
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
		}

		void setupMotors() {
			(initFunctionHandle) (armband, BODYACTUATOR_TYPE_EAI, port, 8);
			printf("armband initialized");
		}
		void startVibrate(uint8_t tactor, float intensity) {
			(startFunctionHandle) (armband, tactor, intensity);
		}
		void stopVibration(uint8_t tactor) {
			(stopFunctionHandle) (armband, tactor);
		}
	};
}