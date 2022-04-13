using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.ECSCore;
namespace Game.Rendering;

struct RenderableComponent : IComponent
{
    public int Id { get; set; }

    public int RenderableID;
}
