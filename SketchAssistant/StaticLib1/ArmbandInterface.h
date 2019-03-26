#pragma once
#include "stdafx.h"
extern "C" {
#include "MotorHeader/BodyActuator.h"
}
#include <time.h>
#include <stdio.h>
#define DllExport extern "C" __declspec( dllexport )
/*
This is the header file of a static class acting as a interface for the BodActuator dll, which is plain C code and can't be called directly from C# (managed) code.
Therefore this class will encapsulate all memory management necessary to instantiate and use an Object of the BodyActuator type, aswell as provide all modyfying methods to external callers.
Basically, this class acts as a single static BodyActuator instance towards outside callers, and can be used from within managed (specifically C#) code.
(side note: the use of the terms 'function' and 'method' might be a bit messy here...)
*/

DllExport int setupArmband();
DllExport void startVibrate(int tactor, float intensity);
DllExport void stopVibrate(int tactor);
DllExport void actuate100(int tactor, double intensity, int duration);
DllExport void actuate66(int tactor, double intensity, int duration);
DllExport void actuate33(int tactor, double intensity, int duration);
DllExport void deleteArmband();

class ArmbandInterface
{
	public:
		/*
		 public initialization method for external calls, loading the BodyActuator dll and linking its methods to the previously defined function
		 handles, aswell as creating and initializing the single static BodyActuator object afterwards (allocates memory to that pointer without
		 releasing it, as the pointer is still needed outside of this method -> mus be freed later)
		 return value may be used for debugging purposes and holds no other purpose
		 */
		__declspec(dllexport) int __cdecl  setupArmband();
		/*
		resets the instance of BodyActuator to a clean state
		*/
		__declspec(dllexport) void __cdecl clearArmband();
		/*
		destructor method destroying the instance of BodyActuator and freeing the memory held by the instance pointer
		*/
		__declspec(dllexport) void __cdecl deleteArmband();
		/*
		start vibrating the specified tactor (number from 0 to 7) at the specified intensity until it is stopped (explicitly or implicitly)
		provides access to the DLLs BodyActuator_startActuation method and handles type conversion to C types required by the DLL which are not available in C#
		*/
		__declspec(dllexport) void __cdecl startVibrate(int tactor, double intensity);
		/*
		explicitly stop actuating the specified tactor (number from 0 to 7)
		provides access to the DLLs BodyActuator_stopActuation method and handles type conversion to C types required by the DLL which are not available in C#
		*/
		__declspec(dllexport) void __cdecl stopVibrate(int tactor);
		/*
		make the specified tactor (number from 0 to 7) actuate at intensity 1.0 (default: between 0.0 and 1.0, but range may be set using the setIntensityRange function) for the specified duration (number of milliseconds) ,or until it is stopped
		provides access to the DLLs BodyActuator_actuate method and handles type conversion to C types required by the DLL which are not available in C#
		*/
		__declspec(dllexport) void __cdecl actuate100(int tactor, double intensity, int duration);
		/*
		make the specified tactor (number from 0 to 7) actuate at intensity 0.66 (default: between 0.0 and 1.0, but range may be set using the setIntensityRange function) for the specified duration (number of milliseconds) ,or until it is stopped
		provides access to the DLLs BodyActuator_actuate method and handles type conversion to C types required by the DLL which are not available in C#
		*/
		__declspec(dllexport) void __cdecl actuate66(int tactor, double intensity, int duration);
		/*
		make the specified tactor (number from 0 to 7) actuate at intensity 0.33 (default: between 0.0 and 1.0, but range may be set using the setIntensityRange function) for the specified duration (number of milliseconds) ,or until it is stopped
		provides access to the DLLs BodyActuator_actuate method and handles type conversion to C types required by the DLL which are not available in C#
		*/
		__declspec(dllexport) void __cdecl actuate33(int tactor, double intensity, int duration);
		/*
		sets the frequency of the specified tactor to a new value (unit unknown, possibly Hz...)
		*/
		__declspec(dllexport) void __cdecl setFrequency(int tactor, int frequency);
		/*
		sets a new intensity range for a single actuator to make different actuators react differently even when receiving an actuation command with the same intensity (e.g. to to compensate differing tactile sensitivity on different parts of the human body)
		*/
		__declspec(dllexport) void __cdecl setIntensityRange(int tactor, double minIntensity, double maxIntensity);
	//private:
		/*
		internal method to initialize the BodyActuator object (and handle the memory allocation involved)
		*/
		int setupMotors();
};