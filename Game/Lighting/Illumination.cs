using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Lighting;

class Illumination
{
    private const int MaxDirectional = 5;
    public int DirectionalCount { get; set; }
    public DirectionalLight[] Directionals = new DirectionalLight[MaxDirectional];
    public Transform[] DirectionalDirections = new Transform[MaxDirectional];

    private const int MaxPoint = 20;
    public int PointCount { get; set; }
    public PointLight[] PointLights = new PointLight[MaxPoint];
    public Transform[] PointPositions = new Transform[MaxPoint];

    public void NewFrame()
    {
        DirectionalCount = 0;

        PointCount = 0;
    }

    public void AddDirectional(DirectionalLight light, Transform direction)
    {
        DirectionalDirections[DirectionalCount] = direction;
        Directionals[DirectionalCount] = light;
        DirectionalCount++;
    }

    public void AddPoint(PointLight light, Transform position)
    {
        PointLights[PointCount] = light;
        PointPositions[PointCount] = position;
        PointCount++;
    }
}
