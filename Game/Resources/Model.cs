using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game.Resources;

class Model
{
    public float[] Vertices { get; internal set; }

    public int VBO { get; private set; }
    public int VAO { get; private set; }

    public void Use()
    {
        // Copy vertices to a GPU buffer
        VBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

        // Use a VAO to draw the vertices
        VAO = GL.GenVertexArray();
        GL.BindVertexArray(VAO);
    }

    public void Unload()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        GL.DeleteBuffer(VBO);
        GL.DeleteVertexArray(VAO);
    }
}
