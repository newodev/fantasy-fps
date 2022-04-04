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
        region.SpawnEntity(new List<IComponent>() { new Transform() { Position = new Vector3(), Scale = new Vector3(1f), Rotation = Vector3.Zero }, new RenderableComponent() { RenderableID = 999 } });

        region.SpawnEntity(new List<IComponent>() { new Transform() { Position = new Vector3(1f, 0f, -2f), Scale = new Vector3(1f), Rotation = Vector3.Zero }, new Camera() { AspectRatio = Settings.AspectRatio, FarPlane = 100f, NearPlane = 0.01f, FieldOfView = 90f } });
    }

    public override void Update()
    {
        Vector3 rot;
        region.Query((ComponentArray<Transform> t, ComponentArray<Camera> cam) =>
        {
            Transform camPos = t[0];
            Camera mainCam = cam[0];

            mainCam.AspectRatio = Settings.AspectRatio;

            cam[0] = mainCam;

            Renderer.UpdateCamera(camPos, mainCam);
        });

        region.Query((ComponentArray<Transform> t, ComponentArray<RenderableComponent> r) =>
        {
            Parallel.For(0, t.Count, (i) =>
            {
                Transform tt = t[i];

                tt.Rotation.X = 0f;
                tt.Rotation.Y += MathHelper.DegreesToRadians(Time.DeltaTime * 40f);
                tt.Rotation.Z = 0f;

                t[0] = tt;
                Renderer.AddObject(t[i].Id, t[i], r[i].RenderableID);
            });
        });
    }
}
