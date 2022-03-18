using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace CSharp_ECS;

static class Mathm
{
    // Used to get the adjacent elements of a grid
    public static readonly Vector2i[] Adjacents = new Vector2i[] {
            new Vector2i(1, 0), new Vector2i(1, 1),
            new Vector2i(0, 1), new Vector2i(-1, 1),
            new Vector2i(-1, 0), new Vector2i(-1, -1),
            new Vector2i(0, -1), new Vector2i(1, -1)
        };
}
