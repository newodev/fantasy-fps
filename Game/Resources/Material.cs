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
    public Texture Albedo { get; private set; }
    public Texture Normal { get; private set; }
    public Texture Roughness { get; private set; }
    public Texture Metallic { get; private set; }
    public Texture AO { get; private set; }



    internal Material(Texture a, Texture r, Texture m, Texture ao, Texture n)
        => (Albedo, Roughness, Metallic, AO, Normal) = (a, r, m, ao, n);

    public void Use()
    {
        Albedo.Use(TextureUnit.Texture0);
        Metallic.Use(TextureUnit.Texture1);
        Roughness.Use(TextureUnit.Texture2);
        AO.Use(TextureUnit.Texture3);
        Normal.Use(TextureUnit.Texture4);
    }
}
