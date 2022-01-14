using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    class QueryResult
    {
        public int Count;
        Archetype a;

        public T GetComponent<T>(int i) where T : struct
        {
            int offset = a.Key.IndexOf(typeof(T));
            int index = offset + a.EntitySize * i;

            return (T)a.Contents[index];
        }

        // TODO: Ideally this should not require creation of a new struct.
        // Want to be able to edit fields directly through a reference
        public void SetComponent<T>(int i, T val) where T : struct
        {
            int offset = a.Key.IndexOf(typeof(T));
            int index = offset + a.EntitySize * i;

            a.Contents[index] = val;
        }
    }
}
