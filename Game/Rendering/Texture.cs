using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Game.Resources;

namespace Game.Rendering;

class Texture
{
    public int Handle { get; private set; }
    
    public Texture(string path)
    {
		Handle = GL.GenTexture();
		Use();

		Bitmap bitmap = Resource.LoadBitmap(path, true);

		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bitmap.Data);
		GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
	}

	public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
		GL.ActiveTexture(unit);
		GL.BindTexture(TextureTarget.Texture2D, Handle);
	}
}
