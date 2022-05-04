using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS;
using OpenTK.Mathematics;

namespace Game.Lighting;

struct DirectionalLight : IComponent
{
    public int Id { get; set; }

    public float Luminosity;
    public Color LightColor;
}
