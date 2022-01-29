using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    static class IDRegistry
    {
        // Map of each Archetype's footprint to its Key
        private static Dictionary<List<Type>, byte> ArchetypeKeys = new Dictionary<List<Type>, byte>();

        // The key of the newest Archetype to be created. The next archetype will have this+1.
        private static byte HighestKey = 0;

        // Map of the highest ID currently available in each archetype. If this exceeds the maximum, it is rolled to -1 and only freedIDs are used
        private static Dictionary<byte, int> highestID = new Dictionary<byte, int>();
        // All the IDs that are now freed due to destroyed entities available in the archetype. This allows a new entity to take an old one's ID.
        private static Dictionary<byte, List<int>> freedIDs = new Dictionary<byte, List<int>>();

        public static byte GetArchetypeKey(List<Type> key)
        {
            // Search for an existing key if the archetype already exists in the Universe
            // (IDs are unique even across regions)
            KeyValuePair<List<Type>, byte> match = ArchetypeKeys.Where(x => x.Key.SequenceEqual(key)).FirstOrDefault();
            if (match.Key != null)
            {
                return match.Value;
            }

            // If there is no existing key, generate a new one and inilitialise
            // The key of this archetype is one higher than the previous
            HighestKey++;

            // Register the key
            ArchetypeKeys.Add(key, HighestKey);
            // Initialise ID lists for the new Archetype
            freedIDs.Add(HighestKey, new List<int>());
            highestID.Add(HighestKey, 0);
            return HighestKey;
        }

        public static byte GetArchetypeKeyFromID(int entityID)
        {
            int shiftedID = entityID >> 24;
            byte key = (byte)shiftedID;

            return key;
        }

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
                // A = key byte, E = local id byte
                // AEEE
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
