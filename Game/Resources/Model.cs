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

    public void Use(int VAO, int VBO)
    {
        // Use a VAO to draw the vertices
        
        GL.BindVertexArray(VAO);

        // Copy vertices to a GPU buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.DynamicDraw);

        GL.BindVertexArray(VAO);
    }
}
