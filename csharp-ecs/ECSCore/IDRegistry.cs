using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    static class IDRegistry
    {
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
            freedIDs.Add(NextKey, new List<int>());
            highestID.Add(NextKey, 0);
            return NextKey;
        }

        // Map of the highest ID currently available in each archetype. If this exceeds the maximum, it is rolled to -1 and only freedIDs are used
        private static Dictionary<byte, int> highestID = new Dictionary<byte, int>();
        // All the IDs that are now freed due to destroyed entities available in the archetype
        private static Dictionary<byte, List<int>> freedIDs = new Dictionary<byte, List<int>>();

        // Generates a new id for an entity, or grabs the first freed ID
        public static int GetNewID(byte archKey)
        {
            int newID = 0;
            if (freedIDs[archKey].Count > 0)
            {
                // Get the first freed ID
                newID = freedIDs[archKey][0];
                freedIDs[archKey].RemoveAt(0);
            }
            else
            {
                // Bitwise-generated IDs containing archetype key and entity ID within the archetype
                int shiftedKey = archKey << 24;
                newID = shiftedKey | highestID[archKey];

                highestID[archKey]++;
            }

            return newID;
        }

        // Frees an ID. Called when an entity is destroyed
        public static void FreeID(int id, byte key)
        {
            freedIDs[key].Add(id);
        }
    }
}
