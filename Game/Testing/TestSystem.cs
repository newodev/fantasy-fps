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
        // Floor
        region.SpawnEntity(EntityFactory.New(2).Transform(new(0f, -1.5f, 0f), new(0f, 0f, 0f), new(5f, 1f, 5f)).Renderable(998).End());

        // Center cube
        region.SpawnEntity(EntityFactory.New(3).Transform(new(0f, 0f, 0f), new(0f, 0f, 0f), new(1f)).Renderable(999).Input().End());

        // Camera
        region.SpawnEntity(EntityFactory.New(2).Transform(new(0f, 2f, -2f), new(MathHelper.PiOver4, 0f, 0f), new(1f)).Camera(Settings.AspectRatio, 0.01f, 100f, 90f).End());

        // Point Lights
        region.SpawnEntity(EntityFactory.New(3).Transform(new(0f, 3f, 0f), new(0f, 0f, 0f), new(0.3f)).Renderable(1).PointLight(Color.White).End());
        region.SpawnEntity(EntityFactory.New(3).Transform(new(-3f, 1f, 0f), new(0f, 0f, 0f), new(0.3f)).Renderable(1).PointLight(Color.Red).End());
        region.SpawnEntity(EntityFactory.New(3).Transform(new(3f, 1f, -1f), new(0f, 0f, 0f), new(0.3f)).Renderable(1).PointLight(Color.Blue).End());
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
