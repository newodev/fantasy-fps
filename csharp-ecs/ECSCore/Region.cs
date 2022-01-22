using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    sealed class Region
    {
        /// <summary>
        /// Generate a query for a set of components
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public QueryResult Query(HashSet<Type> query)
        {
            // Query all archetypes that match this set
            List<ArchetypeCollection> subset = Archetypes.Where(x => query.IsSubsetOf(x.Archetype)).ToList();

            return new QueryResult(subset);
        }
        /// <summary>
        /// Generate a query for a single component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public QueryResult Query<T>() where T : IComponent
        {
            HashSet<Type> q = new HashSet<Type> { typeof(T) };

            return Query(q);
        }

        public List<ArchetypeCollection> Archetypes = new List<ArchetypeCollection>();


        // TODO: Add DestroyEntity function
        public void SpawnEntity(List<IComponent> components)
        {
            // Sort components by type name to match with an archetype
            components.Sort((x, y) => x.GetType().FullName.CompareTo(y.GetType().FullName));

            List<Type> key = new List<Type>();
            foreach (var component in components)
            {
                key.Add(component.GetType());
            }

            List<ArchetypeCollection> a = Archetypes.Where(x => x.Archetype.SequenceEqual(key)).ToList();
            // If this entity doesn't match an archetype, create a new one to match it
            if (a.Count() == 0)
            {
                ArchetypeCollection newArchetype = new ArchetypeCollection(key);
                Archetypes.Add(newArchetype);
                newArchetype.SpawnEntity(components);
            }
            // Else add the entity to its matching archetype
            else
            {
                a.First().SpawnEntity(components);
            }
        }

        public void SpawnEntity(object component)
        {
            SpawnEntity(new List<object> { component });
        }
    }
}
