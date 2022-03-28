using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Game;

static class ApplicationSettings
{
    // Update frequency does not need to be dynamic as only clients are running OpenTK
    // Servers may require dynamic rates, but is not necessary for clients.
    private static double RenderFrequency = 60;
    private static double UpdateFrequency = 60;

    // Settings for the OS window
    private static WindowIcon AppIcon = new WindowIcon();
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
        nws.Icon = AppIcon;
        nws.StartFocused = StartFocused;
        nws.StartVisible = StartVisible;
        nws.Title = WindowName;
        nws.WindowBorder = ResizeMode;
        nws.WindowState = StartMode;

        return nws;
    }
}
