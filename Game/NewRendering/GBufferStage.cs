using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Game.Rendering;
using Game.Resources;

namespace Game.NewRendering;

class GBufferStage : RenderStage
{
    private DrawBuffersEnum[] layers = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3 };
    private int buffer;
    private Shader shader;
    private int posLayer, normLayer, colorLayer, detailLayer;

    // TODO: batch all models, this could be a single large vert array
    private Dictionary<int, Model> models;
    private Dictionary<int, RenderObject> entities;

    int VAO, VBO;

    public GBufferStage(Dictionary<int, Model> m, Dictionary<int, RenderObject> r)
    {
        models = m;
        entities = r;

        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

        shader = new("Resources/Shaders/Standard/Deferred/gbuffer.vert", "Resources/Shaders/Standard/Deferred/gbuffer.frag");
        shader.Use();
        shader.InitialiseAttribute("aPosition", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 0);
        shader.InitialiseAttribute("aNormal", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 3 * sizeof(float));
        shader.InitialiseAttribute("aTexCoord", 2, VertexAttribPointerType.Float, false, 14 * sizeof(float), 6 * sizeof(float));
        shader.InitialiseAttribute("aTangent", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 8 * sizeof(float));
        shader.InitialiseAttribute("aBitangent", 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 11 * sizeof(float));

        GL.GenFramebuffers(1, out buffer);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, buffer);

        posLayer = GenTarget(0, PixelInternalFormat.Rgba16f);
        normLayer = GenTarget(1, PixelInternalFormat.Rgba16f);
        colorLayer = GenTarget(2, PixelInternalFormat.Rgba);
        detailLayer = GenTarget(3, PixelInternalFormat.Rgba);

        GL.DrawBuffers(4, layers);

        // Depth buffer
        int depthBuffer;
        GL.GenRenderbuffers(1, out depthBuffer);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, Settings.Width, Settings.Height);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            Console.WriteLine("Buffer didn't complete");

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public int[] GetLayers()
    {
        return new int[] { posLayer, normLayer, colorLayer, detailLayer };
    }

    public override void Render()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        shader.Use();
        shader.SetInt("material.albedoMap", 0);
        shader.SetInt("material.roughnessMap", 1);
        shader.SetInt("material.metallicMap", 2);
        shader.SetInt("material.aoMap", 3);
        shader.SetInt("material.normalMap", 4);

        Matrix4 view = Mathm.GetViewMatrix(camTransform);
        shader.SetMatrix4("view", view);
        Matrix4 projection = Mathm.GetProjectionMatrix(camera);
        shader.SetMatrix4("projection", projection);

        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities.ElementAt(i);
            int modelID = entity.Value.modelID;
            Transform t = entity.Value.transform;

            Model m = models[modelID];
            m.Use(VAO, VBO);

            Matrix4 model = Mathm.Transform(t);
            shader.SetMatrix4("model", model);
            Matrix3 normal = new(Matrix4.Transpose(Matrix4.Invert(model)));
            shader.SetMatrix3("normalMat", normal);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
        GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        GL.DrawBuffer(DrawBufferMode.Back);
        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, buffer);
        GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
        GL.BlitFramebuffer(0, 0, Settings.Width, Settings.Height, 0, 0, Settings.Width, Settings.Height,
                          ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
    }
}
