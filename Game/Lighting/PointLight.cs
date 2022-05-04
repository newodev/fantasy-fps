using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS;
using System.Drawing;

namespace Game.Lighting;

struct PointLight : IComponent
{
    public int Id { get; set; }

    public float Luminosity;
    public Color LightColor;
    public float Distance;
}
