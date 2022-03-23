using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace Game;

// Being used to learn OpenGL based on the tutorial on the OpenTK.net website.
class TestWindow : GameWindow
{

    float[] vertices = {
        -0.5f, -0.5f, 0.0f, // Bottom-left vertex
         0.5f, -0.5f, 0.0f, // Bottom-right vertex
         0.0f,  0.5f, 0.0f  // Top vertex
    };

    public TestWindow() : base(ApplicationSettings.MakeGWS(), ApplicationSettings.MakeNWS())
    {

    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        base.OnUpdateFrame(e);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        //Code goes here.

        Context.SwapBuffers();

        base.OnRenderFrame(args);
    }

    protected override void OnLoad()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        //Code goes here

        base.OnLoad();
    }
}

