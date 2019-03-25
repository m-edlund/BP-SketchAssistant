using System;

namespace SketchAssistantWPF
{
    public class Wristband
    {
        
        /// <summary>
        /// Function to call when the wristband should push forwards.
        /// </summary>
        public void PushForward()
        {
            Console.WriteLine("FORWARD_PUSH");
        }

        /// <summary>
        /// Function to call when the wristband should push backwards.
        /// </summary>
        public void PushBackward()
        {
            Console.WriteLine("BACKWARD_PUSH");
        }
    }
}