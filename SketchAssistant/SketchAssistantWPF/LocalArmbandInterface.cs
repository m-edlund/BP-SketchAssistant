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

        [DllImport (@"../Debug/StaticLibMotors.dll", EntryPoint = "?setupArmband@ArmbandInterface@@QAAHXZ",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern int setupArmband();

        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?startVibrate@ArmbandInterface@@QAAXHM@Z",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern void startVibrate(int motorNumber, float intensity);

        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?stopVibrate@ArmbandInterface@@QAAXH@Z",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern void stopVibration(int motorNumber);

        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?actuate@ArmbandInterface@@QAAXHNH@Z",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern void actuate(int motorNumber, double intensity, int duration);

        //public void Vibrate()

    }
}
