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

        // Creates a new entity in this archetype with the specified component objects
        public void SpawnEntity(List<object> components)
        {
            Contents.Add(new Entity() { Id = EntityCount });
            for (int i = 0; i < components.Count; i++)
            {
                Contents.Add(components[i]);
            }
            EntityCount++;
        }

        // TODO: Add a destroy function

        // Check if this archetype matches the query
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
    struct Entity
    {
        public int Id;
    }
    struct A
    {
        public int lol;
    }
}
