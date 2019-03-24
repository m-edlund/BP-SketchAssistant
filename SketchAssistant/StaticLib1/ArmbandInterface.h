#pragma once
#include "stdafx.h"
extern "C" {
#include "MotorHeader/BodyActuator.h"
}
#include <time.h>
#include <stdio.h>
#define DllExport extern "C" __declspec( dllexport )



DllExport int setupArmband();
DllExport void startVibrate(int tactor, float intensity);
DllExport void stopVibrate(int tactor);
DllExport void actuate100();
DllExport void actuate66();
DllExport void actuate33();
DllExport void deleteArmband();

class ArmbandInterface
{
	/*
	typedef void(__cdecl *InitFunctionType)(BodyActuator*, BodyActuator_Type, char*, int);
	static InitFunctionType initFunctionHandle;
	typedef void(__cdecl *StartFunctionType)(BodyActuator*, uint8_t, float);
	static StartFunctionType startFunctionHandle;
	typedef void(__cdecl *StopFunctionType)(BodyActuator*, uint8_t);
	static StopFunctionType stopFunctionHandle; */

	public:
		__declspec(dllexport) int __cdecl  setupArmband();
		__declspec(dllexport) void __cdecl startVibrate(int tactor, float intensity);
		__declspec(dllexport) void __cdecl stopVibrate(int tactor);
		__declspec(dllexport) void __cdecl actuate100();
		__declspec(dllexport) void __cdecl actuate66();
		__declspec(dllexport) void __cdecl actuate33();
		__declspec(dllexport) void __cdecl deleteArmband();
		void setupMotors();
	//	void actuate(int tactor, double intensity, int duration);
};