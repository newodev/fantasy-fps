using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Game.NewRendering;

class GBufferStage : RenderStage
{
    private DrawBuffersEnum[] layers = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3 };
    private int buffer;

    public override void Init()
    {
        GL.GenFramebuffers(1, out buffer);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, buffer);
        int posLayer, normLayer, colorLayer, detailLayer;

        posLayer = GenTarget(0, PixelInternalFormat.Rgba16f);
        normLayer = GenTarget(1, PixelInternalFormat.Rgba16f);
        colorLayer = GenTarget(2, PixelInternalFormat.Rgba);
        detailLayer = GenTarget(3, PixelInternalFormat.Rgba);


        GL.DrawBuffers(4, layers);
    }
    public override void Render()
    {

    }
}
