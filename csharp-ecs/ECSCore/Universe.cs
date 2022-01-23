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
        // TODO: An entity's ID should include its archetype as the high bits.
        private static int EntityID = 0;
        public static int NextID()
        {
            EntityID++;
            return EntityID;
        }
        private static List<Region> Regions;
    }
}
