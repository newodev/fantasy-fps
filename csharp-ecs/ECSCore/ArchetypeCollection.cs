using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CSharp_ECS;

// TODO: Integrate with queries etc
internal class ArchetypeCollection
{
    // The key generated on a per-archetype basis
    public readonly byte Key;

    private GenericComponentArray[] ComponentArrays;

    public Type[] ComponentTypes;

    // Buffer of entities that will be destroyed at EoF (end of frame)
    // The buffer of entities to spawn is stored in each component array
    private List<int> EntitiesToDestroy = new();

    public int EntityCount { get => ComponentArrays[0].Count; }

    internal ArchetypeCollection(Type[] types, byte key)
    {
        Key = key;
        ComponentTypes = types;
        ComponentArrays = new GenericComponentArray[types.Length];
        for (int i = 0; i < types.Length; i++)
        {
            ComponentArrays[i] = GenericComponentArray.FromComponentType(types[i]);
        }
    }

    internal GenericComponentArray GetSegmentFromType(Type t)
    {
        int index = -1;
        for (int i = 0; i < ComponentTypes.Length; i++)
        {
            if (ComponentTypes[i].Equals(t))
            {
                index = i;
                break;
            }
        }

        return ComponentArrays[index];
    }

    internal GenericComponentArray GetSegment(int i)
    {
        return ComponentArrays[i];
    }

    public void DestroyEntityByID(int entityID)
    {
        EntitiesToDestroy.Add(entityID);
    }

    public bool Contains(HashSet<Type> query)
    {
        if (query == null)
            throw new ArgumentNullException("query");

        if (query.IsSubsetOf(ComponentTypes))
            return true;
        else
            return false;
    }

    public void Update()
    {
        ClearDestroyBuffer();
        ClearSpawnBuffer();
    }

    public void SpawnEntity(Span<IComponent> components)
    {
        int id = IDRegistry.GetNewID(Key);

        // This assumes that the order of components given matches the order of ComponentArrays
        for (int i = 0; i < components.Length; i++)
        {
            components[i].Id = id;
            ComponentArrays[i].AddToSpawnBuffer(components[i]);
        }
    }

    public void DestroyEntity(int entityID)
    {
        EntitiesToDestroy.Add(entityID);
    }

    public void ClearSpawnBuffer()
    {
        for (int i = 0; i < ComponentArrays.Length; i++)
        {
            ComponentArrays[i].ClearSpawnBuffer();
        }
    }

    private void ClearDestroyBuffer()
    {
        for (int i = 0; i < ComponentArrays.Length; i++)
        {
            ComponentArrays[i].ClearDestroyBuffer(EntitiesToDestroy);
        }
        EntitiesToDestroy.Clear();
    }
}
