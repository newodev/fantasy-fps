using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using Game.Resources;

namespace Game;

static class ApplicationSettings
{
    // Update frequency does not need to be dynamic as only clients are running OpenTK
    // Servers may require dynamic rates, but is not necessary for clients.
    private static double RenderFrequency = 0.0;
    private static double UpdateFrequency = 60.0;

    // Settings for the OS window
    private static string IconPath = "Resources/Textures/pepe.jpg";
    private static bool StartFocused = true;
    private static bool StartVisible = true;
    private static string WindowName = "ECSENGINE";
    private static WindowBorder ResizeMode = WindowBorder.Fixed;
    private static WindowState StartMode = WindowState.Maximized;
    private static int AASamples = 4;

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
        nws.NumberOfSamples = AASamples;

        return nws;
    }

    private static WindowIcon MakeWindowIcon(string iconPath)
    {
        Bitmap resource = Resource.LoadBitmap(iconPath);
        Image img = new(resource.Width, resource.Height, resource.Data);
        return new WindowIcon(img);
    }
}
