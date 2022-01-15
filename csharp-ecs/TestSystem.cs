using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.ECSCore;

namespace CSharp_ECS
{
    class TestSystem : JobSystem
    {
        public override void Update()
        {
            HashSet<Type> query = new HashSet<Type>();
            query.Add(typeof(A));
            QueryResult q = region.Query(query);

            Parallel.For(0, q.Count, (i) =>
            {
                A a = q.GetComponent<A>(i);

                a.lol *= 2;

                q.SetComponent(i, a);
            });
        }
    }
}
