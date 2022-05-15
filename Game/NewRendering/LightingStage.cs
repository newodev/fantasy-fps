using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Game.Rendering;

namespace Game.NewRendering;

class LightingStage : RenderStage
{
    private Shader shader;
    public override void Init()
    {
        shader = new("Resources/Shaders/Standard/Deferred/lighting.vert", "Resources/Shaders/Standard/Deferred/lighting.frag");
        shader.SetInt("gPosition", 0);
        shader.SetInt("gNormal", 1);
        shader.SetInt("gAlbedo", 2);
        shader.SetInt("gDetail", 3);
    }
    public override void Render()
    {

    }
}
