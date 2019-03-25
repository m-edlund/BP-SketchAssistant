#include "stdafx.h"
extern "C" {
#include "MotorHeader/BodyActuator.h"
}
#include <time.h>
#include <stdio.h>
#include "ArmbandInterface.h"
#include <stdlib.h>
/*
This is a static class acting as a interface for the BodActuator dll, which is plain C code and can't be called directly from C# (managed) code.
Therefore this class will encapsulate all memory management necessary to instantiate and use an Object of the BodyActuator type, aswell as provide all modyfying methods to external callers.
Basically, this class acts as a single static BodyActuator instance towards outside callers, and can be used from within managed (specifically C#) code.
(side note: the use of the terms 'function' and 'method' might be a bit messy here...)
*/

/*
function handles are defined as a custom type to be able to dynamically bind and use methods from the C dll (no lib, neither source code for BodyActuator available, therefore only dynamic linking is possible)
*/
//type of the BodyActuator_init function
typedef void(__cdecl *InitFunctionType)(BodyActuator*, BodyActuator_Type, char*, int);
//type of the BodyActuator_clear function
typedef void(__cdecl *ClearFunctionType)(BodyActuator*);
//type of the BodyActuator_delete function
typedef void(__cdecl *DeleteFunctionType)(BodyActuator*);
//type of the BodyActuator_actuate function
typedef void(__cdecl *ActuateFunctionType)(BodyActuator*, uint8_t, float, uint64_t);
//type of the BodyActuator_startActuation function
typedef void(__cdecl *StartFunctionType)(BodyActuator*, uint8_t, float);
//type of the BodyActuator_stopActuation function
typedef void(__cdecl *StopFunctionType)(BodyActuator*, uint8_t);
//type of the BodyActuator_setFrequency function
typedef void(__cdecl *SetFrequencyFunctionType)(BodyActuator*, uint8_t, uint16_t);
//type of the BodyActuator_setIntensityRange function
typedef void(__cdecl *SetIntensityRangeFunctionType)(BodyActuator*, uint8_t, float, float);

/*
static variables to hold the dynamically linked function handles, one per linked function in the BodyActuator dll
*/
//binding of the BodyActuator_init function
static InitFunctionType initFunctionHandle;
//binding of the BodyActuator_clear function
static ClearFunctionType clearFunctionHandle;
//binding of the BodyActuator_delete function
static DeleteFunctionType deleteFunctionHandle;
//binding of the BodyActuator_actuate function
static ActuateFunctionType actuateFunctionHandle;
//binding of the BodyActuator_startActuation function
static StartFunctionType startFunctionHandle;
//binding of the BodyActuator_stopActuation function
static StopFunctionType stopFunctionHandle;
//binding of the BodyActuator_setFrequency function
static SetFrequencyFunctionType setFrequencyFunctionHandle;
//binding of the BodyActuator_setIntensityRange function
static SetIntensityRangeFunctionType setIntensityRangeFunctionHandle;


//static variable to hold the dynamically loaded 'BodyActuator.dll' dll after loading for further processing (acquiring function handles)
static HINSTANCE lib;

//static variable holding the instance of BodyActuator on/with which the methods are called
static BodyActuator* armband;

//port has to be COM5, as it is hardcoded in the given BodyActuator dll and can not be changed //static char *port = 
//if the tactor control unit is connected to a different port: under windows 10 the Serial Port of a device can be changed via the device manager


//specifies the export to follow C style rules (e.g. C style name mangling), only necessary for 'public' methods
extern "C" {

	/*
	 public initialization method for external calls, loading the BodyActuator dll and linking its methods to the previously defined function 
	 handles, aswell as creating and initializing the single static BodyActuator object afterwards (allocates memory to that pointer without 
	 releasing it, as the pointer is still needed outside of this method -> mus be freed later)
	 return value may be used for debugging purposes and holds no other purpose
	 */
	DllExport int __cdecl ArmbandInterface::setupArmband() {
		//load the library at runtime and assign it to the variable
		lib = LoadLibrary(TEXT("BodyActuator.dll"));
		if (lib == NULL) {
			printf("ERROR: library could not be loaded");
			return 0;
		}
		//bind the various function handles
		initFunctionHandle = (InitFunctionType)GetProcAddress(lib, "BodyActuator_init");
		if (initFunctionHandle == NULL) {
			printf("ERROR: init function could not be retrieved");
			return 1;
		}
		clearFunctionHandle = (ClearFunctionType)GetProcAddress(lib, "BodyActuator_clear");
		if (clearFunctionHandle == NULL) {
			printf("ERROR: init function could not be retrieved");
			return 2;
		}
		deleteFunctionHandle = (DeleteFunctionType)GetProcAddress(lib, "BodyActuator_delete");
		if (deleteFunctionHandle == NULL) {
			printf("ERROR: delete function could not be retrieved");
			return 3;
		}
		startFunctionHandle = (StartFunctionType)GetProcAddress(lib, "BodyActuator_startActuation");
		if (startFunctionHandle == NULL) {
			printf("ERROR: start function could not be retrieved");
			return 4;
		}
		stopFunctionHandle = (StopFunctionType)GetProcAddress(lib, "BodyActuator_stopActuation");
			if (stopFunctionHandle == NULL) {
			printf("ERROR: stop function could not be retrieved");
			return 5;
		}
		actuateFunctionHandle = (ActuateFunctionType)GetProcAddress(lib, "BodyActuator_actuate");
			if (actuateFunctionHandle == NULL) {
			printf("ERROR: actuate function could not be retrieved");
			return 6;
		}
		setFrequencyFunctionHandle = (SetFrequencyFunctionType)GetProcAddress(lib, "BodyActuator_setFrequency");
		if (setFrequencyFunctionHandle == NULL) {
			printf("ERROR: setFrequency function could not be retrieved");
			return 7;
		}
		setIntensityRangeFunctionHandle = (SetIntensityRangeFunctionType)GetProcAddress(lib, "BodyActuator_setIntensityRange");
		if (setIntensityRangeFunctionHandle == NULL) {
			printf("ERROR: setIntensityRange function could not be retrieved");
			return 8;
		}
		//initialize the BodyActuator instance
		return setupMotors();
	}

	/*
	resets the instance of BodyActuator to a clean state
	*/
	DllExport void __cdecl ArmbandInterface::clearArmband() {
		(clearFunctionHandle)(armband);
		//printf("armband cleared");
	}

	/*
	destructor method destroying the instance of BodyActuator and freeing the memory held by the instance pointer
	*/
	DllExport void __cdecl ArmbandInterface::deleteArmband() {
		(deleteFunctionHandle)(armband);
		free(armband);
		//printf("armband deleted");
	}

	/*
	start vibrating the specified tactor (number from 0 to 7) at the specified intensity until it is stopped (explicitly or implicitly)
	provides access to the DLLs BodyActuator_startActuation method and handles type conversion to C types required by the DLL which are not available in C#
	*/
	DllExport void __cdecl ArmbandInterface::startVibrate(int tactor, double intensity) {
		(startFunctionHandle)(armband, tactor, intensity);
		//printf("sollte gehen");
	}
			
	/*
	explicitly stop actuating the specified tactor (number from 0 to 7)
	provides access to the DLLs BodyActuator_stopActuation method and handles type conversion to C types required by the DLL which are not available in C#
	*/
	DllExport void __cdecl ArmbandInterface::stopVibrate(int tactor) {
		(stopFunctionHandle)(armband, (uint8_t)tactor);
	}
	
	/*
	make the specified tactor (number from 0 to 7) actuate at intensity 1.0 (default: between 0.0 and 1.0, but range may be set using the setIntensityRange function) for the specified duration (number of milliseconds) ,or until it is stopped
	provides access to the DLLs BodyActuator_actuate method and handles type conversion to C types required by the DLL which are not available in C#
	*/
	DllExport void __cdecl ArmbandInterface::actuate100(int tactor, double intensity, int duration) {
		(actuateFunctionHandle)(armband, tactor, 1.0, duration);
	}

	/*
	make the specified tactor (number from 0 to 7) actuate at a intensity 0.66 (default: between 0.0 and 1.0, but range may be set using the setIntensityRange function) for the specified duration (number of milliseconds) ,or until it is stopped
	provides access to the DLLs BodyActuator_actuate method and handles type conversion to C types required by the DLL which are not available in C#
	*/
	DllExport void __cdecl ArmbandInterface::actuate66(int tactor, int duration) {
		(actuateFunctionHandle)(armband, tactor, 0.66, duration);
	}

	/*
	make the specified tactor (number from 0 to 7) actuate at intensity 0.33 (default: between 0.0 and 1.0, but range may be set using the setIntensityRange function) for the specified duration (number of milliseconds) ,or until it is stopped
	provides access to the DLLs BodyActuator_actuate method and handles type conversion to C types required by the DLL which are not available in C#
	*/
	DllExport void __cdecl ArmbandInterface::actuate33(int tactor, int duration) {
		(actuateFunctionHandle)(armband, tactor, 0.33, duration);
	}

	/*
	sets the frequency of the specified tactor to a new value (unit unknown atm...)
	*/
	DllExport void __cdecl ArmbandInterface::setFrequency(int tactor, int frequency) {
		(setFrequencyFunctionHandle)(armband, tactor, frequency);
	}

	/*
	sets a new intensity range for a single actuator to make different actuators react differently even when receiving an actuation command with the same intensity (e.g. to to compensate differing tactile sensitivity on different parts of the human body)
	*/
	DllExport void __cdecl ArmbandInterface::setIntensityRange(int tactor, double minIntensity, double maxIntensity) {
		(setIntensityRangeFunctionHandle)(armband, tactor, minIntensity, maxIntensity);
	}
}

/*
internal method to initialize the BodyActuator object (and handle the memory allocation involved)
*/
int ArmbandInterface::setupMotors() {
	 char* port = (char*) "COM5";//malloc(7);
	 armband = (BodyActuator*) malloc(sizeof(BodyActuator*));
	 //strcpy_s(port, "COM5");
	 try {
		 (initFunctionHandle)(armband, BODYACTUATOR_TYPE_EAI, port, 8);
		 return -1;
	 }
	 catch (...) {
		 return -99;
	 }
	//printf("armband initialized");
}