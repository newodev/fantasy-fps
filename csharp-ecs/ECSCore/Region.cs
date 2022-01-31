﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.ECSCore.Exceptions;

namespace CSharp_ECS.ECSCore
{
    sealed class Region
    {
        public List<ArchetypeCollection> Archetypes = new List<ArchetypeCollection>();

        // Finds the archetype in this region that matches the key
        public ArchetypeCollection? FindArchetypeFromKey(byte key)
        {
            for (int i = 0; i < Archetypes.Count; i++)
            {
                if (Archetypes[i].Key == key)
                    return Archetypes[i];
            }

            return null;
        }

        // Generates a query for a set of components
        public QueryResult Query(HashSet<Type> query)
        {
            // Query all archetypes that match this set
            List<ArchetypeCollection> subset = Archetypes.Where(x => x.Contains(query)).ToList();

            return new QueryResult(subset);
        }
        
        // Generates a query for a single component
        public QueryResult Query<T>() where T : IComponent
        {
            HashSet<Type> q = new HashSet<Type> { typeof(T) };

            return Query(q);
        }

        // Resolves all the Spawn/Destroy buffers within each ArchetypeCollection
        public void ResolveBuffers()
        {
            for (int i = 0; i < Archetypes.Count; i++)
            {
                Archetypes[i].ResolveBuffers();
            }
        }

        // TODO: Add DestroyEntity function
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
        public void SpawnEntity(List<IComponent> components)
        {
            // TODO: this is allocating multiple lists... and a complex expression.... Look to simplify

            // Sort components by type name to match with an archetype
            components.Sort((x, y) => x.GetType().FullName.CompareTo(y.GetType().FullName));

            List<Type> key = new List<Type>();
            foreach (var component in components)
            {
                key.Add(component.GetType());
            }

            // Find an archetype that matches this new entity
            List<ArchetypeCollection> a = Archetypes.Where(x => x.Archetype.SequenceEqual(key)).ToList();
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
                a.First().SpawnEntity(components);
            }
        }

        // Overload for convenience with a single component
        public void SpawnEntity(IComponent component)
        {
            SpawnEntity(new List<IComponent> { component });
        }
    }
}
