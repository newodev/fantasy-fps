using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    sealed class Region
    {
        public QueryResult Query(HashSet<Type> query)
        {
            // Query all archetypes that match this set
            List<Archetype> subset = Archetypes.Where(x => query.IsSubsetOf(x.Key)).ToList();

            return new QueryResult(subset);
        }
        public QueryResult Query<T>()
        {
            HashSet<Type> q = new HashSet<Type> { typeof(T) };

            return Query(q);
        }

        public List<Archetype> Archetypes = new List<Archetype>();

        public void SpawnEntity(List<object> components)
        {
            // Sort components by type name to match with an archetype
            components.Sort((x, y) => x.GetType().FullName.CompareTo(y.GetType().FullName));

            List<Type> key = new List<Type>();
            foreach (var component in components)
            {
                key.Add(component.GetType());
            }

            List<Archetype> a = Archetypes.Where(x => x.Key.SequenceEqual(key)).ToList();
            // If this entity doesn't match an archetype, create a new one to match it
            if (a.Count() == 0)
            {
                Archetype newArchetype = new Archetype(key);
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
