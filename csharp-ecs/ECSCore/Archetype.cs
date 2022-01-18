using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    // Stores a collection of entities that exist within a region.
    // All entities stored in an archetype have components that exactly match the Key.
    internal class Archetype
    {
        // The archetype key is the components that define the archetype, eg. Position, Rotation.
        // Note that every entity also has the entity component.
        public List<Type> Key;
        public int EntitySize { get => Key.Count + 1; }

        // An entity is described as an array of components.
        // Begins with Entity, followed by the components listed in Key in alphabetical order.
        public List<object> Contents;
        public int EntityCount = 0;

        public Archetype(List<Type> key)
        {
            Key = key;
            Contents = new List<object>();
        }
        /// <summary>
        /// Creates a new entity in this archetype with the specified component objects
        /// </summary>
        /// <param name="components">Entity's Components</param>
        public void SpawnEntity(List<IComponent> components)
        {
            Contents.Add(new Entity() { Id = EntityCount });
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Id = EntityCount;
                Contents.Add(components[i]);
            }
            EntityCount++;
        }

        public void DestroyEntity(int index)
        {
            for (int i = index * EntitySize; i < i * (EntitySize + 1); i++)
            {
                Contents.RemoveAt(i);
            }
            EntityCount++;
        }

        // TODO: Add a destroy function
        /// <summary>
        /// Check if this archetype matches a query
        /// </summary>
        /// <param name="query">Query to match</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Contains(HashSet<Type> query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            if (query.IsProperSubsetOf(Key))
                return true;
            else
                return false;
        }
    }
    struct Entity : IComponent
    {
        public int Id { set; get; }
    }
    interface IComponent
    {
        public int Id { set; get; }
    }
    struct A : IComponent
    {
        public int Id { set; get; }
        public int lol;
    }
}
