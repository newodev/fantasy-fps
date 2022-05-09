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
using OpenTK.Mathematics;
using Game.Rendering;
using Game.Resources;

namespace Game;

// Being used to learn OpenGL based on the tutorial on the OpenTK.net website.
/*
class TestWindow : GameWindow
{
    Shader shader;

    float[] vertices = {
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
         0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
    };

    int VertexBufferObject;
    int VertexArrayObject;
    int ElementBufferObject;
    Texture tex;
    Texture tex2;
    Matrix4 model;
    TestCamera cam;
    bool firstMove;
    Vector2 lastPos;
    public TestWindow() : base(ApplicationSettings.MakeGWS(), ApplicationSettings.MakeNWS())
    {

    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        time += 20f * (float)e.Time;
        model = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(time));
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        const float cameraSpeed = 1.5f;
        const float sensitivity = 0.2f;

        if (KeyboardState.IsKeyDown(Keys.W))
        {
            cam.Position += cam.Front * cameraSpeed * (float)e.Time; // Forward
        }

        if (KeyboardState.IsKeyDown(Keys.S))
        {
            cam.Position -= cam.Front * cameraSpeed * (float)e.Time; // Backwards
        }
        if (KeyboardState.IsKeyDown(Keys.A))
        {
            cam.Position -= cam.Right * cameraSpeed * (float)e.Time; // Left
        }
        if (KeyboardState.IsKeyDown(Keys.D))
        {
            cam.Position += cam.Right * cameraSpeed * (float)e.Time; // Right
        }
        if (KeyboardState.IsKeyDown(Keys.Space))
        {
            cam.Position += cam.Up * cameraSpeed * (float)e.Time; // Up
        }
        if (KeyboardState.IsKeyDown(Keys.LeftShift))
        {
            cam.Position -= cam.Up * cameraSpeed * (float)e.Time; // Down
        }

        // Get the mouse state
        var mouse = MouseState;

        if (firstMove) // This bool variable is initially set to true.
        {
            lastPos = new Vector2(mouse.X, mouse.Y);
            firstMove = false;
        }
        else
        {
            // Calculate the offset of the mouse position
            var deltaX = mouse.X - lastPos.X;
            var deltaY = mouse.Y - lastPos.Y;
            lastPos = new Vector2(mouse.X, mouse.Y);

            // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
            cam.Yaw += deltaX * sensitivity;
            cam.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
        }

        base.OnUpdateFrame(e);
    }

    float time;
    protected override void OnRenderFrame(FrameEventArgs args)
    {

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.BindVertexArray(VertexArrayObject);

        tex.Use(TextureUnit.Texture0);

        shader.Use();

        shader.SetMatrix4("model", model);
        shader.SetMatrix4("view", cam.GetViewMatrix());
        shader.SetMatrix4("projection", cam.GetProjectionMatrix());

        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        SwapBuffers();

        base.OnRenderFrame(args);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
        base.OnResize(e);
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0f, 0f, 0f, 1.0f);

        GL.Enable(EnableCap.DepthTest);

        // bind Vertex Array Object
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);

        // copy our vertices array in a buffer for OpenGL to use
        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        shader = new Shader("OpenGLTest/shader.vert", "OpenGLTest/shader.frag");
        shader.Use();

        var vertexLocation = GL.GetAttribLocation(shader.Handle, "aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3);

        tex = new("OpenGLTest/pepe.jpg");
        tex.Use();
       
        shader.SetInt("texture0", 0);

        model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0f));

        cam = new(Vector3.Zero, Size.X / (float)Size.Y);
        CursorGrabbed = true;
    }

    protected override void OnUnload()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        // Delete all the resources.
        GL.DeleteBuffer(VertexBufferObject);
        GL.DeleteVertexArray(VertexArrayObject);

        base.OnUnload();

        shader.Dispose();
        base.OnUnload();
    }
}
*/
