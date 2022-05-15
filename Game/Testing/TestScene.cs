using CSharp_ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Rendering;
using Game.NewRendering;
using System.Drawing;
using OpenTK.Mathematics;

namespace Game;

class TestScene : Initialiser
{
    private Game.NewRendering.Renderer r;

    public void Rend(Game.NewRendering.Renderer renderer)
    {
        r = renderer;
    }
    public override JobSystem[] InitSystems()
    {
        return new JobSystem[] { new TestSystem(), new RenderSystem(r) };
    }

    public override void InitEntities(Region region)
    {
        // Floor
        region.SpawnEntity(EntityFactory.New(2).Transform(new(0f, -1.5f, 0f), new(0f, 0f, 0f), new(5f, 1f, 5f)).Renderable(998).End());

        // Center cube
        region.SpawnEntity(EntityFactory.New(3).Transform(new(0f, 0f, 0f), new(0f, 0f, 0f), new(1f)).Renderable(999).Input().End());

        // Camera
        region.SpawnEntity(EntityFactory.New(2).Transform(new(0f, 2f, -2f), new(MathHelper.PiOver4, 0f, 0f), new(1f)).Camera(Settings.AspectRatio, 0.01f, 100f, 90f).End());

        // Point Lights
        region.SpawnEntity(EntityFactory.New(2).Transform(new(0f, 3f, 0f), new(0f, 0f, 0f), new(0.3f)).PointLight(Color.White).End());
        region.SpawnEntity(EntityFactory.New(2).Transform(new(-3f, 1f, 0f), new(0f, 0f, 0f), new(0.3f)).PointLight(Color.Red).End());
        region.SpawnEntity(EntityFactory.New(2).Transform(new(3f, 1f, -1f), new(0f, 0f, 0f), new(0.3f)).PointLight(Color.Blue).End());
    }

}
