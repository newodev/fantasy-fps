using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Game.Rendering;

namespace Game.NewRendering;

class GBufferStage : RenderStage
{
    private DrawBuffersEnum[] layers = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3 };
    private int buffer;
    private Shader shader;

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


        int depthBuffer;
        GL.GenRenderbuffers(1, out depthBuffer);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, Settings.Width, Settings.Height);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            Console.WriteLine("Buffer didn't complete");

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        shader = new("Resources/Shaders/Standard/Opaque/shader.vert", "Resources/Shaders/Standard/Deferred/gbuffer.frag");
        
    }
    public override void Render()
    {


    }
}
