using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SketchAssistantWPF
{
    internal class Wristband
    {
        //[StructLayout(LayoutKind.Sequential)]
        //public class BodyActuator
        //{
        //    enum BodyActuator_Type
        //    {
        //        BODYACTUATOR_TYPE_NONE,
        //        BODYACTUATOR_TYPE_EAI,
        //        BODYACTUATOR_TYPE_PIEZO,
        //        BODYACTUATOR_TYPE_ERM,
        //        BODYACTUATOR_TYPE_EMS
        //    };
        //    bool valid;
        //    ushort actuatorCount;
        //    BodyActuator_Type type;
        //    ArduinoHub arduinoHub;
        //    EAIHub eaiHub;
        //}


        //[StructLayout(LayoutKind.Sequential)]
        //public class EAIHub
        //{
        //    bool valid;
        //    pthread_mutex_t mutex; //TODO fix this
        //    pthread_t thread;
        //    Actuator* actuators; //TODO
        //    uint tactorType;
        //    uint modulation;
        //    int deviceID;
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //public class ArduinoHub
        //{
        //    enum ArduinoHub_Type
        //    {
        //        ARDUINOHUB_TYPE_PIEZO = 'P',
        //        ARDUINOHUB_TYPE_ERM = 'E',
        //        ARDUINOHUB_TYPE_EMS = 'M'
        //    }
        //    bool valid;
        //    pthread_mutex_t mutex; //TODO fix this
        //    pthread_t thread;
        //    ArduinoHub_Type arduinoType;
        //    Serial* serial;
        //    Actuator* actuators;
        //}

        //[DllImport("BodyActuator.dll", EntryPoint = "BodyActuator_actuate")]
        //static extern void pushForward(ref BodyActuator self, byte tactor, double intensity, ulong duration);

        /// <summary>
        /// Function to call when the wristband should push forwards.
        /// </summary>
        internal void pushForward()
        {
            Console.WriteLine("FORWARD_PUSH");
        }

        /// <summary>
        /// Function to call when the wristband should push backwards.
        /// </summary>
        internal void pushBackward()
        {
            Console.WriteLine("BACKWARD_PUSH");
        }
    }
}