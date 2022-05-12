using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.Exceptions;
using System.Reflection;

namespace CSharp_ECS;

public class Region
{
    internal List<ArchetypeCollection> Archetypes = new List<ArchetypeCollection>();

    // Finds the archetype in this region that matches the key
    internal ArchetypeCollection? FindArchetypeFromKey(byte key)
    {
        for (int i = 0; i < Archetypes.Count; i++)
        {
            if (Archetypes[i].Key == key)
                return Archetypes[i];
        }

        return null;
    }

    // Generates a query for the components specified in the delegate args, then calls the delegate
    // Usage: region.Query((ComponentArray<C1> c1s, ComponentArray<C2> c2s) => { loop through collections });
    public void Query(Delegate action)
    {
        ParameterInfo[] parameters = action.Method.GetParameters();
        Type[] componentTypes = new Type[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            if (!parameters[i].ParameterType.IsGenericType || parameters[i].ParameterType.GetGenericTypeDefinition() != QueryFactory.Generic)
                throw new ECSException("Query type " + parameters[i].ParameterType.FullName + " is not a Query");
            componentTypes[i] = parameters[i].ParameterType.GetGenericArguments()[0];
        }

        ArchetypeCollection[] matches = GetMatchingArchetypes(componentTypes.ToHashSet());


        // The array of ComponentCollections the delegate is called with
        object[] args = QueryFactory.ConstructQueries(parameters, matches, this);

        action.DynamicInvoke(args);
    }

    private ArchetypeCollection[] GetMatchingArchetypes(HashSet<Type> query)
    {
        ArchetypeCollection[] subset = Archetypes.Where(x => x.Contains(query)).ToArray();

        return subset;
    }

    // Resolves all the Spawn/Destroy buffers within each ArchetypeCollection
    public void ResolveBuffers()
    {
        for (int i = 0; i < Archetypes.Count; i++)
        {
            Archetypes[i].Update();
        }
    }

    public void DestroyEntity(int entityID)
    {
        byte key = IDRegistry.GetArchetypeKeyFromID(entityID);
        ArchetypeCollection? match = FindArchetypeFromKey(key);

        if (match != null)
        {
            match.DestroyEntityByID(entityID);
        }
        else
        {
            throw new ECSException($"Cannot find archetype with key {key} in function Region.DestroyEntity({entityID})");
        }
    }

    // 'Spawns' an entity by adding its components into the collection
    public void SpawnEntity(IComponent[] components)
    {
        // TODO: this is allocating multiple lists... and a complex expression.... Look to simplify

        // Sort components by type name to match with an archetype
        Array.Sort(components, (x, y) => x.GetType().Name.CompareTo(y.GetType().Name));

        Type[] key = new Type[components.Length];
        for (int i = 0; i < components.Length; i++)
        {
            key[i] = components[i].GetType();
        }

        // Find an archetype that matches this new entity
        List<ArchetypeCollection> a = Archetypes.Where(x => x.ComponentTypes.SequenceEqual(key)).ToList();
        // If this entity doesn't match an archetype, create a new one to match it
        if (a.Count() == 0)
        {
            ArchetypeCollection newArchetype = new ArchetypeCollection(key, IDRegistry.GetArchetypeKey(key));
            Archetypes.Add(newArchetype);
            newArchetype.SpawnEntity(components);
        }
        // Else add the entity to its matching archetype
        else
        {
            a.First().SpawnEntity(new Span<IComponent>(components));
        }
    }

    // Overload for convenience with a single component
    public void SpawnEntity(IComponent component)
    {
        SpawnEntity(new[] { component });
    }
}
