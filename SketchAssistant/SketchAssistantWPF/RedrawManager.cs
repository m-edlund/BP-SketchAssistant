using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SketchAssistantWPF
{
    public class RedrawManager
    {
        public int radius { get; private set; }

        private RedrawLine[] redrawLines;

        public int currentLine { get; private set; }

        public RedrawManager(List<InternalLine> linesToRedraw)
        {
            radius = 5;
            redrawLines = new RedrawLine[linesToRedraw.Count];
            Task[] taskPool = new Task[linesToRedraw.Count];
            Console.WriteLine("STARTED THREAD CREATION");
            Console.WriteLine("Processor Count {0}", Environment.ProcessorCount);

            for(int i = 0; i < linesToRedraw.Count; i++)
            {
                InternalLine line = linesToRedraw[i];
                redrawLines[i] = (new RedrawLine(radius));

                object arg = new Tuple<RedrawLine, InternalLine> (redrawLines[i], line);
                taskPool[i] = Task.Factory.StartNew(new Action<object>((x) =>
                {
                    ((Tuple<RedrawLine, InternalLine>)arg).Item1.Init(((Tuple<RedrawLine, InternalLine>)arg).Item2);
                }),arg);
            }
            Console.WriteLine("STARTED {0} THREADS", linesToRedraw.Count);
            Task.WaitAll(taskPool);
            Console.WriteLine("FINISHED ALL THREADS");

            currentLine = 0;
        }

        public Tuple<bool, Point> GetOverlayPosition()
        {
            if(currentLine < 0)
            {
                return new Tuple<bool, Point>(false, new Point(0, 0));
            }
            else
            {
                return new Tuple<bool, Point>(true, redrawLines[currentLine].GetOverlayPosition());
            }
        }

        public Angle GetDirection(Point p)
        {
            return redrawLines[currentLine].GetDirection(p);
        }
    }
}
