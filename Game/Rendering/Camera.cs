using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS;

namespace Game.Rendering;

struct Camera : IComponent
{
    public int Id { get; set; }

    public float FieldOfView;
    public float AspectRatio;
    public float NearPlane;
    public float FarPlane;
}
