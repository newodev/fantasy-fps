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
        region.SpawnEntity(new List<IComponent>() { new Transform() { Position = new Vector3(0f, 0f, 3f), Scale = new Vector3(1f), Rotation = Quaternion.FromEulerAngles(1.5f, -1.5f, 0f) }, new Camera() { AspectRatio = Settings.AspectRatio, FarPlane = 100f, NearPlane = 0.01f, FieldOfView = 90f } });
    }

    public override void Update()
    {
        Vector3 rot;
        region.Query((ComponentArray<Transform> t, ComponentArray<Camera> cam) =>
        {
            Transform camPos = t[0];
            Camera mainCam = cam[0];
            rot = camPos.Rotation.ToEulerAngles();
            rot.Y += 0.1f;
            camPos.Rotation = new Quaternion(rot);
            t[0] = camPos;

            Renderer.UpdateCamera(camPos, mainCam);
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
