using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    /// <summary>
    /// A Universe describes the entire existence of the game world. It then divides the universe spatially into Regions. Each server unit can work on at least one Region
    /// </summary>
    static class Universe
    {
        public static readonly int MAX_ENTITIES_PER_ARCHETYPE = 10000;
        private static List<Region> Regions;
    }
}
