using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OptiTrack
{
    public class HighPerformanceTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long start;
        private long freq;

        public HighPerformanceTimer()
        {
            start = 0;
            QueryPerformanceFrequency(out freq);
        }

        public void Start()
        {
            QueryPerformanceCounter(out start);
        }

        public double Stop()
        {
            long stop;
            QueryPerformanceCounter(out stop);
            return (double)(stop - start) / (double)freq;
        }
    }
}
