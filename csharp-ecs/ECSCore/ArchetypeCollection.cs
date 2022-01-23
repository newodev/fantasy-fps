using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    // Stores a collection of entities that exist within a region.
    // All entities stored in an archetype have components that exactly match the Key.
    internal class ArchetypeCollection
    {
        // TODO: components should be arranged as AAAAPPPPZZZZ. This also removes the needs for Entity components.
        // The key generated on a per-archetype basis
        public readonly byte Key;

        // The archetype is the components that define the collection, eg. Position + Rotation.
        // Note that every entity also has the entity component.
        public List<Type> Archetype;
        public int EntitySize { get => Archetype.Count + 1; }

        // An entity is described as an array of components.
        // Begins with Entity, followed by the components listed in Key in alphabetical order.
        public List<IComponent> Contents;
        public int EntityCount = 0;

        public ArchetypeCollection(List<Type> archetype, byte key)
        {
            Archetype = archetype;
            Contents = new List<IComponent>();
            Key = key;
        }

        // TODO: all calls to destroy or spawn entities should be buffered until the end of frame
        // TODO: components should be arranged as AAAAPPPPZZZZ. This also removes the needs for Entity components.
        // TODO: new keys will glitch out if count exceeeds 2^24. Add a cap or smth
        // TODO: create a list of freed ID's to grab from once an entity is destroyed.
        /// <summary>
        /// Creates a new entity in this archetype with the specified component objects
        /// </summary>
        /// <param name="components">Entity's Components</param>
        public void SpawnEntity(List<IComponent> components)
        {
            int id = IDRegistry.GetNewID(Key);
            Console.WriteLine(Convert.ToString(id, 2));
            Contents.Add(new Entity() { Id = id });
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Id = id;
                Contents.Add(components[i]);
            }
            EntityCount++;
        }

        // TODO: all calls to destroy or spawn entities should be buffered until the end of frame
        // TODO: components should be arranged as AAAAPPPPZZZZ. This also removes the needs for Entity components.
        // TODO: fix... currently misses the correct components every time
        public void DestroyEntity(int index)
        {
            for (int i = index * EntitySize; i < index * (EntitySize + 1); i++)
            {
                Contents.RemoveAt(index * EntitySize);
            }
            EntityCount--;
        }
        public void DestroyEntityByID(int id)
        {
            int index = GetEntityIndexByID(id);
            DestroyEntity(index);
        }

        /// <summary>
        /// Check if this archetype matches a query
        /// </summary>
        /// <param name="query">Query to match</param>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Contains(HashSet<Type> query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            if (query.IsProperSubsetOf(Archetype))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Finds an entity's index in this collection based on its ID. 
        /// </summary>
        /// <param name="id">The entity's ID</param>
        public int GetEntityIndexByID(int id)
        {
            return GetEntityIndexByID(id, 0, Contents.Count - 1);
        }

        private int GetEntityIndexByID(int id, int start, int end)
        {
            // Binary search implementation
            int pivot = (start + end) / 2;

            if (Contents[pivot].Id == id)
            {
                // Iterate backwards until we reach the Entity component
                // TODO: components should be arranged as AAAAPPPPZZZZ. This also removes the needs for Entity components.
                bool isEntity = Contents[pivot].GetType() == typeof(Entity);
                while (!isEntity)
                {
                    pivot--;
                    isEntity = Contents[pivot].GetType() == typeof(Entity);
                }
                return pivot / EntitySize;
            }
            else if (Contents[pivot].Id < id)
            {
                return GetEntityIndexByID(id, pivot + 1, end);
            }
            else if (Contents[pivot].Id > id)
            {
                return GetEntityIndexByID(id, start, pivot - 1);
            }
            return -1;
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
