using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Rendering;
using OpenTK.Graphics.OpenGL;

namespace Game.NewRendering;

abstract class RenderStage
{
    public Camera camera;
    public Transform camTransform;
    public abstract void Render();

    protected int GenTarget(int i, PixelInternalFormat type)
    {
        int tex;
        GL.GenTextures(1, out tex);
        GL.BindTexture(TextureTarget.Texture2D, tex);
        GL.TexImage2D(TextureTarget.Texture2D, 0, type, Settings.Width, Settings.Height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, tex, 0);

        return tex;
    }
}
