using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Lighting;
using Game.Rendering;

namespace Game.NewRendering;

class Renderer
{
    private Illumination illumination = new();
    private GBufferStage gBuffer;
    private LightingStage lighting;

    public Renderer()
    {
        gBuffer = new();
        lighting = new(illumination);
        lighting.SetGBuffer(gBuffer.GetLayers());
    }

    public void Render()
    {

    }
}
