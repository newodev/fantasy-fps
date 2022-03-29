using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

// Used to load the window icon
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Game;

static class ApplicationSettings
{
    // Update frequency does not need to be dynamic as only clients are running OpenTK
    // Servers may require dynamic rates, but is not necessary for clients.
    private static double RenderFrequency = 60;
    private static double UpdateFrequency = 60;

    // Settings for the OS window
    private static string IconPath = "Resources/pepe.jpg";
    private static bool StartFocused = true;
    private static bool StartVisible = true;
    private static string WindowName = "ECSENGINE";
    private static WindowBorder ResizeMode = WindowBorder.Fixed;
    private static WindowState StartMode = WindowState.Maximized;

    public static GameWindowSettings MakeGWS()
    {
        GameWindowSettings gws = new GameWindowSettings();

        gws.RenderFrequency = RenderFrequency;
        gws.UpdateFrequency = UpdateFrequency;

        return gws;
    }

    public static NativeWindowSettings MakeNWS()
    {
        NativeWindowSettings nws = new NativeWindowSettings();

        nws.Icon = MakeWindowIcon(IconPath);

        nws.StartFocused = StartFocused;
        nws.StartVisible = StartVisible;
        nws.Title = WindowName;
        nws.WindowBorder = ResizeMode;
        nws.WindowState = StartMode;

        return nws;
    }

    // Ideally have this handled and OpenGL textures in a resource loader
    private static WindowIcon MakeWindowIcon(string iconPath)
    {
        var image = SixLabors.ImageSharp.Image.Load<Rgba32>(iconPath);

        // Convert to array of color bytes
        var pixels = new List<byte>(4 * image.Width * image.Height);

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
        OpenTK.Windowing.Common.Input.Image img = new(image.Width, image.Height, pixels.ToArray());
        return new WindowIcon(img);
    }
}
