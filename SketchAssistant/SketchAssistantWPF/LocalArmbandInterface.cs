using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SketchAssistantWPF
{
    public static class LocalArmbandInterface
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

        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?actuate100@ArmbandInterface@@QAAXHMH@Z",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern void actuate100(int motorNumber, double intensity, int duration);

        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?actuate66@ArmbandInterface@@QAAXHMH@Z",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern void actuate66(int motorNumber, double intensity, int duration);

        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?actuate33@ArmbandInterface@@QAAXHMH@Z",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern void actuate33(int motorNumber, double intensity, int duration);

        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?deleteArmband@ArmbandInterface@@QAAXXZ",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern void deleteArmband();

        //public void Vibrate()

    }
}
