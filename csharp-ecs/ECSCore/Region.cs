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
            return new QueryResult();
        }
    }
}
