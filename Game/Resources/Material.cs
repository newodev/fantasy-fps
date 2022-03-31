using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Texture.Use();
    }
}
