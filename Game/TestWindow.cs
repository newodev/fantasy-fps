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
using OpenTK.Mathematics;

namespace Game;

// Being used to learn OpenGL based on the tutorial on the OpenTK.net website.
class TestWindow : GameWindow
{
    Shader shader;

    float[] vertices = {
         0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
         0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
    };

    uint[] indices = {  // note that we start from 0!
        0, 1, 3,   // first triangle
        1, 2, 3    // second triangle
    };

    int VertexBufferObject;
    int VertexArrayObject;
    int ElementBufferObject;
    Texture tex;
    Texture tex2;
    Matrix4 model;
    Matrix4 view;
    Matrix4 projection;
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

    float time;
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        time += 20f * (float)args.Time;
        model = Matrix4.Identity * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(time));

        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.BindVertexArray(VertexArrayObject);

        tex.Use(TextureUnit.Texture0);
        tex2.Use(TextureUnit.Texture1);

        shader.Use();

        shader.SetMatrix4("model", model);
        shader.SetMatrix4("view", view);
        shader.SetMatrix4("projection", projection);

        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

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
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        // bind Vertex Array Object
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);

        // copy our vertices array in a buffer for OpenGL to use
        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        shader = new Shader("shader.vert", "shader.frag");
        shader.Use();

        var vertexLocation = GL.GetAttribLocation(shader.Handle, "aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        tex = new("pepe.jpg");
        tex.Use();
        tex2 = new("peeposad.jpg");
        tex.Use(TextureUnit.Texture1);
       
        shader.SetInt("texture1", 0);
        shader.SetInt("texture2", 1);

        model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0f));
        view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
        projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / Size.Y, 0.1f, 100.0f);
    }

    protected override void OnUnload()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        // Delete all the resources.
        GL.DeleteBuffer(VertexBufferObject);
        GL.DeleteVertexArray(VertexArrayObject);
        GL.DeleteBuffer(ElementBufferObject);

        base.OnUnload();

        shader.Dispose();
        base.OnUnload();
    }
}

