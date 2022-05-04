using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS;

public static class ECS
{
    public static bool EntityHasComponent(Type componentType, int entityID)
    {
        ReadOnlySpan<Type> components = IDRegistry.ComponentTypesFromID(entityID);
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == componentType)
                return true;
        }
        return false;
    }
}
internal static class IDRegistry
{
    // Map of each Archetype's footprint to its Key
    private static Dictionary<byte, Type[]> ArchetypeKeys = new();

    // The key of the newest Archetype to be created. The next archetype will have this+1.
    private static byte HighestKey = 0;

    // Map of the highest ID currently available in each archetype. If this exceeds the maximum, it is rolled to -1 and only freedIDs are used
    private static Dictionary<byte, int> highestID = new();
    // All the IDs that are now freed due to destroyed entities available in the archetype. This allows a new entity to take an old one's ID.
    private static Dictionary<byte, List<int>> freedIDs = new();

    public static ReadOnlySpan<Type> ComponentTypesFromID(int entityID)
    {
        byte key = GetArchetypeKeyFromID(entityID);

        ReadOnlySpan<Type> result = new ReadOnlySpan<Type>(ArchetypeKeys[key]);
        return result;
    }

    public static byte GetArchetypeKey(Type[] key)
    {
        // Search for an existing key if the archetype already exists in the Universe
        // (IDs are unique even across regions)
        KeyValuePair<byte, Type[]> match = ArchetypeKeys.Where(x => x.Value.SequenceEqual(key)).FirstOrDefault();
        if (match.Value != null)
        {
            return match.Key;
        }

        // If there is no existing key, generate a new one and inilitialise
        // The key of this archetype is one higher than the previous
        HighestKey++;

        // Register the key
        ArchetypeKeys.Add(HighestKey, key.ToArray());
        // Initialise ID lists for the new Archetype
        freedIDs.Add(HighestKey, new List<int>());
        highestID.Add(HighestKey, 0);
        return HighestKey;
    }

    // Gets the key byte out of an entity's ID, which can then be used to find the archetype the entity belongs to.
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
