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

            // TODO: Finish
            return new QueryResult();
        }

        public List<Archetype> Archetypes = new List<Archetype>();

    }
}
