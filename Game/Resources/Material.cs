using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Game.Rendering;

namespace Game.Resources;

class Material
{
    public Texture Texture { get; private set; }
    internal Material(Texture t)
    {
        Texture = t;
    }

    public void Use()
    {
        Texture.Use(TextureUnit.Texture0);
    }
}
