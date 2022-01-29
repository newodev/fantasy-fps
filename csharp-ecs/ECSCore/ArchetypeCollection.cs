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

        // Entity creation/destruction buffers. This is to ensure entity indexes remain the same throughout each frame
        private List<IComponent[]> EntitiesToSpawn = new List<IComponent[]>();
        private List<int> EntitiesToDestroy = new List<int>();

        public ArchetypeCollection(List<Type> archetype, byte key)
        {
            Archetype = archetype;
            Contents = new List<IComponent>();
            Key = key;
        }

        public void ResolveBuffers()
        {
            DestroyBufferedEntities();
            SpawnBufferedEntities();
        }

        // Adds an entity to the buffer to be instantiated at the end of frame
        public void SpawnEntity(List<IComponent> components)
        {
            // This is mini cringe. Ideally make it all run on arrays, as lists have too much memalloc
            EntitiesToSpawn.Add(components.ToArray());
        }

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
            // Get an ID for this new entity
            int id = IDRegistry.GetNewID(Key);
            
            // Console.WriteLine(Convert.ToString(id, 2));

            EntityCount++;

            // Add each of the components to the correct location in the list
            for (int i = 0; i < EntitiesToSpawn[entityIndex].Length; i++)
            {
                // Set the entity ID of each component
                IComponent component = EntitiesToSpawn[entityIndex][i];
                component.Id = id;

                // TODO: Fix, index exceepds list size
                // Insert all components into the collection, starting at the back to simplify the algorithm
                int insertTarget = (Contents.Count - i) * EntityCount;
                Contents.Insert(insertTarget, component);
            }
        }

        // Marks an entity by ID to be destroyed at the end of frame
        public void DestroyEntityByID(int id)
        {
            int index = GetEntityIndexByID(id);
            DestroyEntity(index);
        }
        // Marks an entity to be destroyed at the end of frame
        public void DestroyEntity(int index)
        {
            EntitiesToDestroy.Add(index);
        }

        // Iterates through the destroy buffer and removes all marked entities from the collection
        private void DestroyBufferedEntities()
        {
            for (int i = 0; i < EntitiesToDestroy.Count; i++)
            {
                DestroyBufferedEntity(i);
            }

            EntitiesToDestroy.Clear();
        }

        // Removes an entity from the collection
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
            // Binary search along the first component array in the collection
            // eg. AAAAPPPPZZZZ
            // Searches along the A's for a match
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
}
