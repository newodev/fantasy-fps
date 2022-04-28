using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Game.Resources;


public struct Vertex
{
    public Vector3 Position;
    public Vector2 UV;
    public Vector3 Normal;
    public Vector3 Tangent;
    public Vector3 Bitangent;

    public Vertex(Vector3 pos, Vector2 uv, Vector3 norm) =>
        (Position, UV, Normal, Tangent, Bitangent) = (pos, uv, norm, Vector3.Zero, Vector3.Zero);
    public Vertex(float px, float py, float pz, float nx, float ny, float nz, float u, float v) =>
        (Position, Normal, UV, Tangent, Bitangent) = (new Vector3(px, py, pz), new Vector3(nx, ny, nz), new Vector2(u, v), Vector3.Zero, Vector3.Zero);
}

public struct Triangle
{
    public Vertex V1;
    public Vertex V2;
    public Vertex V3;

    public Triangle(Vertex v1, Vertex v2, Vertex v3) =>
        (V1, V2, V3) = (v1, v2, v3);
}

static class ModelLoader
{
    public static Model LoadCube()
    {
        Model cube = new();

        int vecSize = 14;

        Triangle[] tris =
        {
            new(
                new(-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f),
                new(0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f),
                new(0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f)
             ),
            new(
                new( 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f),
                new(-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f),
                new(-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f)
             ),
            new(
                new(-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f),
                new( 0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f),
                new( 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f)
             ),                                                           
            new(                                                         
                new( 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f),
                new(-0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f),
                new(-0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f)
             ),                                                           
            new(                                                         
                new(-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f),
                new(-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f),
                new(-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f)
             ),                                                          
            new(                                                         
                new(-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f),
                new(-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f),
                new(-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f)
             ),                                                           
             new(                                                        
                 new(0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f),
                 new(0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f),
                 new(0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f)
              ),                                                          
             new(                                                        
                 new(0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f),
                 new(0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f),
                 new(0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f)
              ),                                                          
            new(                                                         
                new(-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f),
                new( 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f),
                new( 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f)
             ),                                                           
            new(                                                         
                new( 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f),
                new(-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f),
                new(-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f)
             ),                                                           
            new(                                                         
                new(-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f),
                new( 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f),
                new( 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f)
             ),
            new(
                new( 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f),
                new(-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f),
                new(-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f)
             )
        };

        for (int i = 0; i < tris.Length; i++)
        {
            tris[i] = CalculateTangents(tris[i]);
        }

        float[] vertices = new float[vecSize * tris.Length * 3];
        int vPointer = 0;
        for (int i = 0; i < tris.Length; i++)
        {
            vPointer = InsertVertex(vertices, vPointer, tris[i].V1);
            vPointer = InsertVertex(vertices, vPointer, tris[i].V2);
            vPointer = InsertVertex(vertices, vPointer, tris[i].V3);
        }
        cube.Vertices = vertices;
        for (int i = 3; i < vertices.Length; i+= 14 * 6)
        {
            Console.WriteLine($"norm: {vertices[i]}, {vertices[i + 1]}, {vertices[i + 2]}");
        }
        return cube;
    }

    private static int InsertVertex(float[] vertices, int index, Vertex v)
    {
        (vertices[index], vertices[index + 1], vertices[index + 2]) = (v.Position.X, v.Position.Y, v.Position.Z);
        (vertices[index + 3],  vertices[index + 4],  vertices[index + 5])  = (v.Normal.X,     v.Normal.Y,    v.Normal.Z);
        (vertices[index + 6], vertices[index + 7]) = (v.UV.X, v.UV.Y);
        (vertices[index + 8],  vertices[index + 9],  vertices[index + 10]) = (v.Tangent.X,   v.Tangent.Y,   v.Tangent.Z);
        (vertices[index + 11], vertices[index + 12], vertices[index + 13]) = (v.Bitangent.X, v.Bitangent.Y, v.Bitangent.Z);

        return index + 14;
    }

    private static Triangle CalculateTangents(Triangle t)
    {
        Vector3 edge1 = t.V2.Position - t.V1.Position;
        Vector3 edge2 = t.V3.Position - t.V1.Position;
        Vector2 deltaUV1 = t.V2.UV - t.V1.UV;
        Vector2 deltaUV2 = t.V3.UV - t.V1.UV;

        Vector3 tangent, bitangent;
        float f = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);

        tangent.X = f * (deltaUV2.Y * edge1.X - deltaUV1.Y * edge2.X);
        tangent.Y = f * (deltaUV2.Y * edge1.Y - deltaUV1.Y * edge2.Y);
        tangent.Z = f * (deltaUV2.Y * edge1.Z - deltaUV1.Y * edge2.Z);

        bitangent.X = f * (-deltaUV2.X * edge1.X + deltaUV1.X * edge2.X);
        bitangent.Y = f * (-deltaUV2.X * edge1.Y + deltaUV1.X * edge2.Y);
        bitangent.Z = f * (-deltaUV2.X * edge1.Z + deltaUV1.X * edge2.Z);

        t.V1.Tangent = t.V2.Tangent = t.V3.Tangent = tangent;
        t.V1.Bitangent = t.V2.Bitangent = t.V3.Bitangent = bitangent;
        return t;
    }
}
