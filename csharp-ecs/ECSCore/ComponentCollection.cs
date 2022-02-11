using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.ECSCore.Exceptions;

namespace CSharp_ECS.ECSCore
{
    class ComponentCollection<T> where T : IComponent
    {
        public int Count;
        private List<ArchetypeCollection> matches;
        // Represents the base 1 index of this collection's component type in each ArchetypeCollection's archetype
        // eg. ComponentCollection<C>
        //     matches: ABC BCD CDE
        //     offsets: 3   2   1
        private List<int> componentOffsets;
        // An instance of T so that it doesn't have to be repeatedly instantiated to query
        private Type typeInstance;

        public ComponentCollection(List<ArchetypeCollection> _matches)
        {
            matches = _matches;
            Count = 0;
            componentOffsets = new List<int>();
            typeInstance = typeof(T);
            foreach (ArchetypeCollection a in matches)
            {
                Count += a.EntityCount;
                componentOffsets.Add(a.Archetype.IndexOf(typeInstance));
            }
        }

        public T this[int index]
        {
            get => GetComponent(index);
            set => SetComponent(index, value);
        }

        public T GetComponent(int i)
        {
            int match = FindEntityArchetype(i);
            ArchetypeCollection a = matches[match];
            int index = FindComponentIndex(i, match);

            // Copy the component
            return (T)a.Contents[index];
        }

        public void SetComponent(int i, T val)
        {
            int match = FindEntityArchetype(i);
            ArchetypeCollection a = matches[match];
            int index = FindComponentIndex(i, match);

            // Maintain the current component's Id
            val.Id = a.Contents[index].Id;
            // Apply the value changes
            a.Contents[index] = val;
        }


        // Finds the index in matches of the archetype of the entity at i in this collection
        public int FindEntityArchetype(int entityIndex)
        {
            if (matches.Count == 1)
                return 0;
            for (int i = 0; i < matches.Count; i++)
            {
                ArchetypeCollection a = matches[i];
                if (entityIndex > a.EntityCount)
                {
                    entityIndex -= a.EntityCount;
                }
                else
                {
                    return i;
                }
            }
            throw new IndexOutOfRangeException("Index i out of bounds in QueryResult Find(i)");
        }

        // Finds the index of a component based on its type and the entity's index
        private int FindComponentIndex(int i, int match)
        {
            ArchetypeCollection a = matches[match];
            // Find the location of the component in the collection
            int offset = componentOffsets[match];
            int index = offset * a.EntityCount + i;

            if (offset == -1)
                throw new ECSArchetypeException(typeInstance, a.Key, $"FindComponentIndex({i}, {match})");

            return index;
        }
    }
}
