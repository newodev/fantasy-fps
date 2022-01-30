﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.ECSCore.Exceptions;

namespace CSharp_ECS.ECSCore
{
    class QueryResult
    {
        // TODO: Remake Querys to work by returning three generic collections, each referencing a single component type of the query.

        public int Count;
        List<ArchetypeCollection> matches;

        public QueryResult(List<ArchetypeCollection> _matches)
        {
            matches = _matches;
            Count = 0;
            foreach (ArchetypeCollection a in matches)
            {
                Count += a.EntityCount;
            }
        }

        /// <summary>
        /// Finds the archetype of an entity that matches the query based on its index out of all matching archetypes
        /// <summary>
        public ArchetypeCollection FindEntityArchetype(int i)
        {
            if (matches.Count == 1)
                return matches[0];
            foreach (ArchetypeCollection a in matches)
            {
                if (i > a.EntityCount)
                {
                    i -= a.EntityCount;
                }
                else
                {
                    return a;
                }
            }
            throw new IndexOutOfRangeException("Index i out of bounds in QueryResult Find(i)");
        }

        /// <summary>
        /// Gets the component of specified type of entity at index i
        /// </summary>
        /// <typeparam name="T">Type of component to get</typeparam>
        /// <param name="i">Entity index (out of all archetypes that match the query)</param>
        /// <returns></returns>
        public T GetComponent<T>(int i) where T : IComponent
        {
            ArchetypeCollection a = FindEntityArchetype(i);
            int index = FindComponentIndex<T>(i, a);

            // Copy the component
            return (T)a.Contents[index];
        }

        // TODO: Ideally this should not require creation of a new struct.
        // Want to be able to edit fields directly through a reference
        public void SetComponent<T>(int i, T val) where T : IComponent
        {
            ArchetypeCollection a = FindEntityArchetype(i);
            int index = FindComponentIndex<T>(i, a);

            // Apply the value changes
            a.Contents[index] = val;
        }

        // Finds the index of a component based on its type and the entity's index
        private int FindComponentIndex<T>(int i, ArchetypeCollection a) where T : IComponent
        {
            Type componentType = typeof(T);
            // Find the location of the component in the collection
            int offset = a.Archetype.IndexOf(componentType) + 1;
            int index = offset * i + a.EntitySize;

            if (offset == 0)
                throw new ECSArchetypeException(componentType, a.Key, $"FindComponentIndex<{componentType.Name}>({i})");

            return index;
        }
    }
}
