using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Resources;

// An intermediary image class that can be turned into OpenGL data or other images
class Bitmap
{
    public int Height { get; private set; }
    public int Width { get; private set; }

    public byte[] Data { get; private set; }

    internal Bitmap(int h, int w, byte[] data)
    {
        Height = h;
        Width = w;
        Data = data;
    }
}
