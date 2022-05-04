using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS;
using Game.InputDevices;
using OpenTK.Mathematics;
using Game.Lighting;
using System.Drawing;

namespace Game.Rendering;

// TODO: Frustrum culling
class RenderSystem : JobSystem
{
    private Renderer Renderer;
    public RenderSystem(Renderer r)
    {
        Renderer = r;
    }

    public override void Update()
    {
        region.Query((Query<Transform> t, Query<Camera> cam) =>
        {
            Transform camPos = t[0];
            Camera mainCam = cam[0];

            mainCam.AspectRatio = Settings.AspectRatio;

            cam[0] = mainCam;

            Renderer.UpdateCamera(camPos, mainCam);
        });

        region.Query((Query<Transform> t, Query<RenderableComponent> r) =>
        {
            Parallel.For(0, t.Count, (i) =>
            {
                Renderer.AddObject(t[i].Id, t[i], r[i].RenderableID);
            });
        });

        region.Query((Query<Transform> t, Query<DirectionalLight> l) =>
        {
            for(int i = 0; i < t.Count; i++)
            {
                Renderer.Light.AddDirectional(l[i], t[i]);
            }
        });

        region.Query((Query<Transform> t, Query<PointLight> l) =>
        {
            for (int i = 0; i < t.Count; i++)
            {
                Renderer.Light.AddPoint(l[i], t[i]);
            }
        });
    }
}
