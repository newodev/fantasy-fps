using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Game;

class Texture
{
    public int Handle { get; private set; }
    
    public Texture(string path)
    {
		Handle = GL.GenTexture();
		Use();

		Image<Rgba32> image = Image.Load<Rgba32>(path);
		
		// Flip image vertically as OpenGL reads from bottom to top
		image.Mutate(x => x.Flip(FlipMode.Vertical));

		// Convert to array of color bytes
		var pixels = new List<byte>(4 * image.Width * image.Height);

		image.ProcessPixelRows(pixelAccessor =>
		{
			for (int y = 0; y < image.Height; y++)
			{
				Span<Rgba32> row = pixelAccessor.GetRowSpan(y);

				for (int x = 0; x < image.Width; x++)
				{
					pixels.Add(row[x].R);
					pixels.Add(row[x].G);
					pixels.Add(row[x].B);
					pixels.Add(row[x].A);
				}
			}
		});

		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());
		GL.GenerateTextureMipmap(Handle);
	}

	public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
		GL.ActiveTexture(unit);
		GL.BindTexture(TextureTarget.Texture2D, Handle);
	}
}
