using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Game.Rendering;

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
        Matrix4 rotation = Matrix4.CreateRotationX(t.Rotation.X) * Matrix4.CreateRotationY(t.Rotation.Y) * Matrix4.CreateRotationZ(t.Rotation.Z);
        Matrix4 translation = Matrix4.CreateTranslation(t.Position);

        return scale * rotation * translation;  
    }

    // Gets the vector pointing to the right of the transform
    public static Vector3 Right(Transform t)
    {
        return Quaternion.FromEulerAngles(t.Rotation) * Vector3.UnitX;
    }
    // Gets the vector pointing above the transform
    public static Vector3 Up(Transform t)
    {
        Vector3 result = Quaternion.FromEulerAngles(t.Rotation) * Vector3.UnitY;
        return result;
    }
    // Gets the vector pointing ahead of the transform
    public static Vector3 Front(Transform t)
    {
        Vector3 result = Quaternion.FromEulerAngles(t.Rotation) * Vector3.UnitZ;
        return result;
    }

    public static Matrix4 GetViewMatrix(Transform t)
    {
        Matrix4 result = Matrix4.LookAt(t.Position, t.Position + Front(t), Up(t));
        return result;
    }

    public static Matrix4 GetProjectionMatrix(Camera cam)
    {
        return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(cam.FieldOfView), cam.AspectRatio, cam.NearPlane, cam.FarPlane);
    }
}
