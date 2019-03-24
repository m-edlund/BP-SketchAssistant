using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SketchAssistantWPF
{
    /// <summary>
    /// interface providing access to the vibrotactile armband
    /// accessing the BodyActuator.dll C library via the StaticLibMotors.dll c++ library
    /// </summary>
    public static class LocalArmbandInterface
    {
        /// <summary>
        /// initializes the armband (and binds the C dll)
        /// must be called before calling any other of the methods of this class
        /// explicitly allocates memory and therefore must only be called once and must be followed by a call to DestroyArmband eventually
        /// </summary>
        /// <returns>an integer purely for debugging purposes, -1 means no unexpected behaviour occured and the initialization was successful</returns>
        [DllImport (@"../Debug/StaticLibMotors.dll", EntryPoint = "?setupArmband@ArmbandInterface@@QAAHXZ",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetupArmband();

        /// <summary>
        /// destroys the armband instance created by SetupArmband (thus freeing its allocated memory)
        /// </summary>
        /// <returns></returns>
        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?destroyArmband@ArmbandInterface@@QAAHXZ",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern int DestroyArmband();

        /// <summary>
        /// starts actuation of the specified tactor (motor) at the specified intensity (until it is stopped)
        /// </summary>
        /// <param name="motorNumber">integer from 0 to 7 specifying the number of the tactor to actuate</param>
        /// <param name="intensity">intensity, ranging from 0.0 to 1.0 by default</param>
        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?startVibrate@ArmbandInterface@@QAAXHM@Z",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern void StartVibrate(int motorNumber, double intensity);

        /// <summary>
        /// stop actuation of the specified tactor (motor)
        /// </summary>
        /// <param name="motorNumber">integer from 0 to 7 specifying the number of the tactor to stop</param>
        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?stopVibrate@ArmbandInterface@@QAAXH@Z",                       
     CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopVibration(int motorNumber);

        /// <summary>
        /// starts actuation of the specified tactor (motor) at the specified intensity for a specified amount of time
        /// </summary>
        /// <param name="tactor">integer from 0 to 7 specifying the number of the tactor to actuate</param>
        /// <param name="intensity">intensity, ranging from 0.0 to 1.0 by default</param>
        /// <param name="duration">number of millisecons to actuate the tactor for</param>
        [DllImport(@"../Debug/StaticLibMotors.dll", EntryPoint = "?actuate@ArmbandInterface@@QAAXXZ",
     CallingConvention = CallingConvention.Cdecl)]
        public static extern void Actuate(int tactor, double intensity, int duration);

    }
}
