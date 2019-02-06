using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketchAssistantWPF
{
    public class ImageDimension
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public ImageDimension(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void ChangeDimension(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
