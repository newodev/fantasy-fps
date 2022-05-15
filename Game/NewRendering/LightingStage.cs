using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Game.Rendering;
using Game.Lighting;
using Game.Resources;

namespace Game.NewRendering;

class LightingStage : RenderStage
{
    private Shader shader;
    private int quadVAO;
    private Illumination lights;

    public int gPosition, gNormal, gAlbedo, gDetail;

    public LightingStage(Illumination i)
    {
        lights = i;
        // The lighting stage renders to a screen-size quad. Here we initialise the quad object
        quadVAO = ModelLoader.GenScreenQuad();

        shader = new("Resources/Shaders/Standard/Deferred/lighting.vert", "Resources/Shaders/Standard/Deferred/lighting.frag");
        shader.SetInt("gPosition", 0);
        shader.SetInt("gNormal", 1);
        shader.SetInt("gAlbedo", 2);
        shader.SetInt("gDetail", 3);
    }

    public override void Render()
    {
        // Wipe screen
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        shader.Use();
        // Bind textures from the GBuffer
        BindTexture(TextureUnit.Texture0, gPosition);
        BindTexture(TextureUnit.Texture1, gNormal);
        BindTexture(TextureUnit.Texture2, gAlbedo);
        BindTexture(TextureUnit.Texture3, gDetail);

        // Send all lights to shader
        for (int i = 0; i < lights.PointCount; i++)
        {
            Color lightCol = lights.PointLights[i].LightColor;
            Vector3 colVec = new(lightCol.R, lightCol.G, lightCol.B);

            shader.SetVec3($"pointLights[{i}].position", lights.PointPositions[i].Position);
            shader.SetVec3($"pointLights[{i}].color", colVec);
        }

        shader.SetVec3("viewPos", camTransform.Position);
        shader.SetInt("numPointLight", lights.PointCount);

        // Render quad
        GL.BindVertexArray(quadVAO);
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        GL.BindVertexArray(0);
    }

    public void SetGBuffer(int[] layers)
    {
        gPosition = layers[0];
        gNormal = layers[1];
        gAlbedo = layers[2];
        gDetail = layers[3];
    }

    private void BindTexture(TextureUnit t, int tex)
    {
        GL.ActiveTexture(t);
        GL.BindTexture(TextureTarget.Texture2D, tex);
    }
}
