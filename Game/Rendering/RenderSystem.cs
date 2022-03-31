using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.ECSCore;
using OpenTK.Mathematics;

namespace Game.Rendering;

class RenderSystem : JobSystem
{
    private Renderer Renderer;
    public RenderSystem(Renderer r)
    {
        Renderer = r;
    }

    public override void Init()
    {
        region.SpawnEntity(new List<IComponent>() { new Transform() { Position = new Vector3(0f), Scale = new Vector3(1f), Rotation = Quaternion.Identity }, new RenderableComponent() { RenderableID = 999 } });
        region.SpawnEntity(new List<IComponent>() { new Transform() { Position = new Vector3(0f), Scale = new Vector3(1f), Rotation = Quaternion.Identity }, new Camera() { AspectRatio = Settings.AspectRatio, FarPlane = 100f, NearPlane = 100f, FieldOfView = 90f } });
    }

    public override void Update()
    {
        Camera mainCam;
        Transform camPos;
        region.Query((ComponentArray<Transform> t, ComponentArray<Camera> cam) =>
        {
            mainCam = cam[0];
            camPos = t[0];
        });

        region.Query((ComponentArray<Transform> t, ComponentArray<RenderableComponent> r) =>
        {
            Parallel.For(0, t.Count, (i) =>
            {
                Renderer.AddObject(t[i].Id, t[i], r[i].RenderableID);
            });
        });
    }
}
