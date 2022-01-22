using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.SpatialIndexing
{
    internal interface ISpatialIndex
    {
        public List<int> Query(AABB box);
        public void Insert(int element, Point p);
        public void Update(int element, Point p);
        public void Delete(int element);
    }
    struct AABB
    {
        public float X;
        public float Y;

        public float halfDimension;

        public AABB(int x, int y, float h)
        {
            X = x;
            Y = y;
            halfDimension = h;
        }

        public AABB(Point p, float h)
        {
            X = p.X;
            Y = p.Y;
            halfDimension = h;
        }
    }
    struct Point
    {
        public float X;
        public float Y;

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Point operator /(Point a, int b) => new Point(a.X / b, a.Y / b);
    }
}
