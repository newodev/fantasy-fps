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
