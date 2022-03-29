using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Game.Resources;

static class Resource
{
    public static Bitmap LoadBitmap(string path, bool flipY = false)
    {
        Image<Rgba32> image = Image.Load<Rgba32>(path);

        if (flipY)
        {
            // Flip image vertically as OpenGL reads from bottom to top
            image.Mutate(x => x.Flip(FlipMode.Vertical));
        }

        // Convert to array of color bytes
        List<byte> pixels = new(4 * image.Width * image.Height);

        image.ProcessPixelRows(pixelAccessor =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = pixelAccessor.GetRowSpan(y);

                for (int x = 0; x < image.Width; x++)
                {
                    pixels.Add(row[x].R);
                    pixels.Add(row[x].G);
                    pixels.Add(row[x].B);
                    pixels.Add(row[x].A);
                }
            }
        });

        Bitmap map = new(image.Height, image.Width, pixels.ToArray());
        return map;
    }
}
