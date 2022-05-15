using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Game.Resources;

namespace Game.NewRendering;

class Model
{
    public float[] Vertices { get; internal set; }
    public Material Material { get; internal set; }

    public Model(float[] vert, Material m)
        => (Vertices, Material) = (vert, m);

    public void Use(int VAO, int VBO)
    {
        Material.Use();
        GL.BindVertexArray(VAO);

        // Copy vertices to a GPU buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.DynamicDraw);
    }
}
