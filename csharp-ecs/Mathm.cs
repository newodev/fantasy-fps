using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS
{
    static class Mathm
    {
        // Used to get the adjacent elements of a grid
        public static readonly IntPoint[] Adjacents = new IntPoint[] { 
            new IntPoint(1, 0), new IntPoint(1, 1), 
            new IntPoint(0, 1), new IntPoint(-1, 1), 
            new IntPoint(-1, 0), new IntPoint(-1, -1), 
            new IntPoint(0, -1), new IntPoint(1, -1) 
        };

    }
}
