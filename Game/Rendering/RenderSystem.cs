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
        region.SpawnEntity(new List<IComponent>() { new Transform() { Position = new Vector3(0f, -1.5f, 0f), Scale = new Vector3(5f, 1f, 5f), Rotation = new Vector3(0f) }, new RenderableComponent() { RenderableID = 998 } });
        region.SpawnEntity(new List<IComponent>() { new Transform() { Position = new Vector3(0f, 0f, 0f), Scale = new Vector3(1f), Rotation = new Vector3(0f) }, new RenderableComponent() { RenderableID = 999 }, new InputComponent() });
        region.SpawnEntity(new List<IComponent>() { new Transform() { Position = new Vector3(0f, 1f, 0f), Scale = new Vector3(0.8f), Rotation = new Vector3(0f) }, new RenderableComponent() { RenderableID = 999 }, new InputComponent() });
        region.SpawnEntity(new List<IComponent>() { new Transform() { Position = new Vector3(0f, 0f, -2f), Scale = new Vector3(1f), Rotation = Vector3.Zero }, new Camera() { AspectRatio = Settings.AspectRatio, FarPlane = 100f, NearPlane = 0.01f, FieldOfView = 90f } });
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

        region.Query((ComponentArray < Transform > t, ComponentArray < InputComponent> i) =>
        {
            Parallel.For(0, t.Count, (i) =>
            {
                Transform tt = t[i];
        
                if (Input.GetKeyHeld(InputAction.Forward) > 0)
                    tt.Position.Z += 2 * Time.DeltaTime;
                if (Input.GetKeyHeld(InputAction.Backward) > 0)
                    tt.Position.Z -= 2 * Time.DeltaTime;
                if (Input.GetKeyHeld(InputAction.Left) > 0)
                    tt.Position.X += 2 * Time.DeltaTime;
                if (Input.GetKeyHeld(InputAction.Right) > 0)
                    tt.Position.X -= 2 * Time.DeltaTime;
        
                tt.Rotation.Y += MathHelper.DegreesToRadians(Input.MouseDelta.X);
                tt.Rotation.X -= MathHelper.DegreesToRadians(Input.MouseDelta.Y);
        
                t[i] = tt;
            });
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
