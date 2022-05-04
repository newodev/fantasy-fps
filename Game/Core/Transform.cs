using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS;
using OpenTK.Mathematics;

namespace Game;

struct Transform : IComponent
{
    public int Id { get; set; }

    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
}
