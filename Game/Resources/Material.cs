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
    public Texture Diffuse { get; private set; }
    public Texture Specular { get; private set; }
    public float Shininess;
    internal Material(Texture d, Texture s, float shininess)
    {
        Diffuse = d;
        Specular = s;
        Shininess = shininess;
    }

    public void Use()
    {
        Diffuse.Use(TextureUnit.Texture0);
        Specular.Use(TextureUnit.Texture1);
    }
}
