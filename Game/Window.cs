using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;

namespace Game;

public static class Settings
{
    public static float AspectRatio { get; set; }
}

class Window : GameWindow
{
    // Event that is called every game update. Used by the ECSClient to update the simulation.
    public event EventHandler<double>? GameUpdate;
    // Event used by the renderer to take drawing out of this window
    public event EventHandler<double>? FrameUpdate;

    public Window() : base(ApplicationSettings.MakeGWS(), ApplicationSettings.MakeNWS())
    {
        Settings.AspectRatio = Size.X / (float)Size.Y;
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        // Invoke the game update event, which is used to run the game loop.
        GameUpdate.Invoke(this, e.Time);

        base.OnUpdateFrame(e);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        Settings.AspectRatio = Size.X / (float)Size.Y;

        FrameUpdate.Invoke(this, e.Time);

        base.OnRenderFrame(e);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
        base.OnResize(e);
    }
}
