using System;
using System.Runtime.InteropServices;

namespace SketchAssistantWPF
{
    internal class Armband
    {
        //[DllImport("BodyActuator.dll")]
        //static extern void pushForward(BodyActuator* self, uint8_t tactor, double intensity, uint64_t duration);
        internal void pushForward()
        {
            Console.WriteLine("FORWARD_PUSH");
        }

        internal void pushBackward()
        {
            Console.WriteLine("BACKWARD_PUSH");
        }
    }
}