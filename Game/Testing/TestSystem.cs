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
using Game.Rendering;

namespace Game;

// TODO: Frustrum culling
class TestSystem : JobSystem
{
    public override void Init()
    {
        region.SpawnEntity(EntityFactory.New(2).Transform(new(0f, -1.5f, 0f), new(0f, 0f, 0f), new(5f, 1f, 5f)).Renderable(998).End());
        region.SpawnEntity(EntityFactory.New(3).Transform(new(0f, 0f, 0f), new(0f, 0f, 0f), new(1f)).Renderable(999).Input().End());
        region.SpawnEntity(EntityFactory.New(2).Transform(new(0f, 2f, -2f), new(MathHelper.PiOver4, 0f, 0f), new(1f)).Camera(Settings.AspectRatio, 0.01f, 100f, 90f).End());
        region.SpawnEntity(new IComponent[] { new Transform() { Position = new Vector3(0f, 2f, -2f), Scale = new Vector3(1f), Rotation = new(MathHelper.PiOver4, 0f, 0f) }, new Camera() { AspectRatio = Settings.AspectRatio, FarPlane = 100f, NearPlane = 0.01f, FieldOfView = 90f } });
        region.SpawnEntity(new IComponent[] { new Transform() { Position = new Vector3(0f, 3f, 0f), Scale = new Vector3(0.3f) }, new PointLight() { LightColor = Color.White }, new RenderableComponent() { RenderableID = 1 } });
        region.SpawnEntity(new IComponent[] { new Transform() { Position = new Vector3(-3f, 1f, 0f), Scale = new Vector3(0.3f) }, new PointLight() { LightColor = Color.Red }, new RenderableComponent() { RenderableID = 1 } });
        region.SpawnEntity(new IComponent[] { new Transform() { Position = new Vector3(3f, 1f, -1f), Scale = new Vector3(0.3f) }, new PointLight() { LightColor = Color.Blue }, new RenderableComponent() { RenderableID = 1 } });
    }

    public override void Update()
    {
        region.Query((Query<Transform> t, Query<InputComponent> i) =>
        {
            Parallel.For(0, t.Count, (i) =>
            {
                ref Transform tt = ref t.GetRef(i);

                if (Input.GetKeyHeld(InputAction.Forward) > 0)
                    tt.Position.Z += 2 * Time.DeltaTime;
                if (Input.GetKeyHeld(InputAction.Backward) > 0)
                    tt.Position.Z -= 2 * Time.DeltaTime;
                if (Input.GetKeyHeld(InputAction.Left) > 0)
                    tt.Position.X += 2 * Time.DeltaTime;
                if (Input.GetKeyHeld(InputAction.Right) > 0)
                    tt.Position.X -= 2 * Time.DeltaTime;
                if (Input.GetKeyHeld(InputAction.Secondary) > 0)
                    tt.Rotation.Z += 2 * Time.DeltaTime;
            });
        });
    }
}
