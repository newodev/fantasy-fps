using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.SpatialIndexing
{
    class Grid : ISpatialIndex
    {
        GridNode[,] nodes = new GridNode[30, 30];
        int NodeWidth = 100;
        public void Delete(int element)
        {
            throw new NotImplementedException();
        }

        public void Insert(int element, Point p)
        {
            // TODO: fix/finish
            Point node = FindNode(p);
            nodes[(int)node.X, (int)node.Y].entities.Add(new EntityFootprint());
        }

        public List<int> Query(AABB box)
        {
            throw new NotImplementedException();
        }

        public void Update(int element, Point p)
        {
            throw new NotImplementedException();
        }

        // Translates a point in world space to the node coordinate.
        public Point FindNode(Point p)
        {
            return new Point((int)(p.X / NodeWidth), (int)(p.Y / NodeWidth));
        }
    }

    internal struct GridNode
    {
        public List<EntityFootprint> entities;
    }

    struct EntityFootprint
    {
        public int ID;
        public List<Type> Archetype;
        public Point Position;
    }
}
