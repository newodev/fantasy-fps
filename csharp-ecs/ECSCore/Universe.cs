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
        private static List<Region> Regions;

        private static Dictionary<List<Type>, byte> ArchetypeKeys = new Dictionary<List<Type>, byte>();

        private static byte NextKey = 0;
        public static byte GetArchetypeKey(List<Type> key)
        {
            KeyValuePair<List<Type>, byte> match = ArchetypeKeys.Where(x => x.Key.SequenceEqual(key)).FirstOrDefault();

            if (match.Key != null)
            {
                return match.Value;
            }

            NextKey++;
            ArchetypeKeys.Add(key, NextKey);
            return NextKey;
        }
    }
}
