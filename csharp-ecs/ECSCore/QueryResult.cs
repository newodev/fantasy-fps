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
        List<Archetype> matches;

        public QueryResult(List<Archetype> _matches)
        {
            matches = _matches;
            Count = 0;
            foreach (Archetype a in matches)
            {
                Count += a.EntityCount;
            }
        }

        public Archetype Find(int i)
        {
            if (matches.Count == 1)
                return matches[0];
            foreach (Archetype a in matches)
            {
                if (i > a.EntityCount)
                {
                    i -= a.EntityCount;
                }
                else
                {
                    return a;
                }
            }
            throw new IndexOutOfRangeException("Index i out of bounds in QueryResult Find(i)");
        }

        public T GetComponent<T>(int i) where T : struct
        {
            Archetype a = Find(i);
            int offset = a.Key.IndexOf(typeof(T)) + 1;
            int index = offset + a.EntitySize * i;

            return (T)a.Contents[index];
        }

        // TODO: Ideally this should not require creation of a new struct.
        // Want to be able to edit fields directly through a reference
        public void SetComponent<T>(int i, T val) where T : struct
        {
            Archetype a = Find(i);

            int offset = a.Key.IndexOf(typeof(T)) + 1;
            int index = offset + a.EntitySize * i;

            a.Contents[index] = val;
        }
    }
}
