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
        // The key generated on a per-archetype basis
        public readonly byte Key;

        // The archetype is the components that define the collection, eg. Position + Rotation.
        // Note that every entity also has the entity component.
        public List<Type> Archetype;
        public int EntitySize { get => Archetype.Count; }

        // An entity is described as an array of components.
        // Contents are layed out as AAAABBBBCCCC.
        public List<IComponent> Contents;
        public int EntityCount = 0;

        private List<IComponent[]> EntitiesToSpawn = new List<IComponent[]>();
        private List<int> EntitiesToDestroy = new List<int>();

        // Iterates through the spawn buffer and adds them all to the collection
        private void SpawnBufferedEntities()
        {
            for (int i = 0; i < EntitiesToSpawn.Count; i++)
            {
                SpawnBufferedEntity(i);
            }

            EntitiesToSpawn.Clear();
        }

        // Adds a single entity to the collection
        private void SpawnBufferedEntity(int entityIndex)
        {
            int id = IDRegistry.GetNewID(Key);
            Console.WriteLine(Convert.ToString(id, 2));

            EntityCount++;
            for (int i = 0; i < EntitiesToSpawn[entityIndex].Length; i++)
            {
                IComponent component = EntitiesToSpawn[entityIndex][i];
                component.Id = id;

                // Insert all components into the collection, starting at the back to simplify the algorithm
                int insertTarget = (Contents.Count - i) * EntityCount;
                Contents.Insert(insertTarget, component);
            }
        }

        private void DestroyBufferedEntities()
        {
            for (int i = 0; i < EntitiesToDestroy.Count; i++)
            {
                DestroyBufferedEntity(i);
            }

            EntitiesToDestroy.Clear();
        }

        private void DestroyBufferedEntity(int entityIndex)
        {
            int id = Contents[entityIndex].Id;
            IDRegistry.FreeID(id, Key);

            for (int i = 0; i < EntitySize; i++)
            {
                // Destroy all components that compose the entity, starting from the back
                int destroyTarget = (EntitySize - i) * EntityCount + entityIndex;
                EntitiesToDestroy.RemoveAt(destroyTarget);
            }
            EntityCount--;
        }

        public ArchetypeCollection(List<Type> archetype, byte key)
        {
            Archetype = archetype;
            Contents = new List<IComponent>();
            Key = key;
        }

        // TODO: all calls to destroy or spawn entities should be buffered until the end of frame
        // TODO: new keys will glitch out if count exceeeds 2^24. Add a cap or smth
        // TODO: create a list of freed ID's to grab from once an entity is destroyed.
        // Adds an entity to the spawn buffer. Entities are truly spawned once all systems are spooled down.
        public void SpawnEntity(List<IComponent> components)
        {
            // This is mini cringe. Ideally make it all run on arrays, as lists have too much memalloc
            EntitiesToSpawn.Add(components.ToArray());
        }

        // TODO: all calls to destroy or spawn entities should be buffered until the end of frame
        // TODO: fix... currently misses the correct components every time
        // TODO: fix... isnt updated to new AABBCC mem layout
        public void DestroyEntity(int index)
        {
            EntitiesToDestroy.Add(index);
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
            return GetEntityIndexByID(id, 0, EntityCount - 1);
        }

        private int GetEntityIndexByID(int id, int start, int end)
        {
            // Binary search implementation
            int pivot = (start + end) / 2;

            if (Contents[pivot].Id == id)
            {
                return pivot;
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
