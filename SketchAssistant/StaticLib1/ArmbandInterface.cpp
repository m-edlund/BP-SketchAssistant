#include "stdafx.h"
extern "C" {
#include "MotorHeader/ArduinoHub.h"
}
#include <ctime>


namespace StaticLib1
{		
	class UnitTest1
	{

		ArduinoHub* armband;

	public : 

		int main() {
			setupMotors();
			startVibrate(1, 1.0);
		}

		void setupMotors() {
			ArduinoHub_init(armband, "4", (ArduinoHub_Type) 'P');
		}
		void startVibrate(uint8_t tactor, float intensity) {
			ArduinoHub_startVibration(armband, tactor, intensity);
		}
		void nstopVibration(uint8_t tactor) {
			ArduinoHub_stopVibration(armband, tactor);
		}
	};
}