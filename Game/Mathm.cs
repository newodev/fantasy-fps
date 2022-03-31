using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Game;

static class Mathm
{
    // Used to get the adjacent elements of a grid
    public static readonly Vector2i[] Adjacents = new Vector2i[] {
            new Vector2i(1, 0), new Vector2i(1, 1),
            new Vector2i(0, 1), new Vector2i(-1, 1),
            new Vector2i(-1, 0), new Vector2i(-1, -1),
            new Vector2i(0, -1), new Vector2i(1, -1)
        };

    // Turns a Transform component into a transformation matrix used for rendering
    public static Matrix4 Transform(Transform t)
    {
        Matrix4 scale = Matrix4.CreateScale(t.Scale);
        Matrix4 rotation = Matrix4.CreateFromQuaternion(t.Rotation);
        Matrix4 translation = Matrix4.CreateTranslation(t.Position);

        return translation * rotation * scale;  
    }

    // Gets the vector pointing to the right of the transform
    public static Vector3 Right(Transform t)
    {
        return t.Rotation * Vector3.UnitX;
    }
    // Gets the vector pointing above the transform
    public static Vector3 Up(Transform t)
    {
        return t.Rotation * Vector3.UnitY;
    }
    // Gets the vector pointing ahead of the transform
    public static Vector3 Front(Transform t)
    {
        return t.Rotation * Vector3.UnitZ;
    }

    public static Matrix4 GetViewMatrix(Transform t)
    {
        return Matrix4.LookAt(t.Position, t.Position + Front(t), Up(t));
    }

    public static Matrix4 GetProjectionMatrix(Camera cam)
    {
        return Matrix4.CreatePerspectiveFieldOfView(cam.FieldOfView, cam.AspectRatio, cam.NearPlane, cam.FarPlane);
    }
}
