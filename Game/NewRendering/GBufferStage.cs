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
    private int posLayer, normLayer, colorLayer, detailLayer;

    public GBufferStage()
    {
        GL.GenFramebuffers(1, out buffer);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, buffer);

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
        shader.InitialiseAttribute("aPosition", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 0);
        shader.InitialiseAttribute("aNormal", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 3 * sizeof(float));
        shader.InitialiseAttribute("aTexCoord", 2, VertexAttribPointerType.Float, false, 14 * sizeof(float), 6 * sizeof(float));
        shader.InitialiseAttribute("aTangent", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 8 * sizeof(float));
        shader.InitialiseAttribute("aBitangent", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 11 * sizeof(float));
    }

    public int[] GetLayers()
    {
        return new int[] { posLayer, normLayer, colorLayer, detailLayer };
    }

    public override void Render()
    {
        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities.ElementAt(i);
            int renderableID = entity.Value.RenderableID;
            Transform t = entity.Value.Transform;

            Renderable r = renderables[renderableID];
            r.UseWithTransform(t, CameraPos, CurrentCamera);

            r.Shader.SetInt("numDirLight", Light.DirectionalCount);

            for (int j = 0; j < Light.DirectionalCount; j++)
            {
                UseDirectional(j, r, Light.Directionals[j], Light.DirectionalDirections[j]);
            }

            r.Shader.SetInt("numPointLight", Light.PointCount);

            for (int j = 0; j < Light.PointCount; j++)
            {
                UsePoint(j, r, Light.PointLights[j], Light.PointPositions[j]);
            }

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
    }
}
