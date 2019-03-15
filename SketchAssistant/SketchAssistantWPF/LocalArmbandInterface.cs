using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SketchAssistantWPF
{
    class LocalArmbandInterface
    {

        [DllImport (@"StaticLibMotors.dll")]
        public static extern int setupArmband();

        [DllImport(@"StaticLibMotors.dll")]
        public static extern void startVibrate(int motorNumber, float intensity);

        [DllImport(@"StaticLibMotors.dll")]
        public static extern void stopVibration(int motorNumber);

        //public void Vibrate()

    }
}
