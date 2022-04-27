using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.ECSCore;
using Game.InputDevices;
using OpenTK.Mathematics;
using Game.Lighting;
using System.Drawing;

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
        region.SpawnEntity(new IComponent[] { new Transform() { Position = new Vector3(0f, -1.5f, 0f), Scale = new Vector3(5f, 1f, 5f), Rotation = new Vector3(0f) }, new RenderableComponent() { RenderableID = 998 } });
        region.SpawnEntity(new IComponent[] { new Transform() { Position = new Vector3(0f, 0f, 0f), Scale = new Vector3(1f), Rotation = new Vector3(0f) }, new RenderableComponent() { RenderableID = 999 }, new InputComponent() });
        region.SpawnEntity(new IComponent[] { new Transform() { Position = new Vector3(0f, 1f, -2f), Scale = new Vector3(1f), Rotation = new(MathHelper.PiOver4, 0f, 0f) }, new Camera() { AspectRatio = Settings.AspectRatio, FarPlane = 100f, NearPlane = 0.01f, FieldOfView = 90f } });
        region.SpawnEntity(new IComponent[] { new Transform() { Position = new Vector3(0f, 4f, 0f) }, new PointLight() { LightColor = Color.White } });
    }

    public override void Update()
    {
        Vector3 rot;
        region.Query((Query<Transform> t, Query<Camera> cam) =>
        {
            Transform camPos = t[0];
            Camera mainCam = cam[0];

            mainCam.AspectRatio = Settings.AspectRatio;

            cam[0] = mainCam;

            Renderer.UpdateCamera(camPos, mainCam);
        });

        region.Query((Query<Transform> t, Query<InputComponent> i) =>
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
        
                tt.Rotation.Y += MathHelper.DegreesToRadians(Input.MouseDelta.X) * 0.3f;
                tt.Rotation.X -= MathHelper.DegreesToRadians(Input.MouseDelta.Y) * 0.3f;
        
                t[i] = tt;
            });
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
