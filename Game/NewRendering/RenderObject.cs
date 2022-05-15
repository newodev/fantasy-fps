using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.NewRendering;

struct RenderObject
{
    public int modelID;
    public Transform transform;

    public RenderObject(int model, Transform t)
        => (modelID, transform) = (model, t);
}
