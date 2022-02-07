using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.SpatialIndexing
{
    class Grid
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
            IntPoint node = FindNode(p);
            nodes[node.X, node.Y].entities.Add(new EntityFootprint() { ID = element, Position = p });
        }

        public List<int> Query(Point center, float radius)
        {
            throw new NotImplementedException();
        }

        public void Update(int element, Point p)
        {
            // Check to see if element is still in same node
            IntPoint nodePoint = FindNode(p);
            GridNode node = nodes[nodePoint.X, nodePoint.Y];
            // If it is in the same node, update it and return
            if (node.UpdateEntity(element, p))
            {
                return;
            }

            // Entity is not in same node, search adjacents
            IntPoint adjacentNodePoint = FindEntityNodeInAdjacents(element, nodePoint);
            if (adjacentNodePoint != new IntPoint(-1, -1))
            {
                GridNode adjacentNode = nodes[adjacentNodePoint.X, adjacentNodePoint.Y];
                // TODO: add a function to do this cleanly
                node.entities.RemoveAt(node.Find(element));
                // TODO: this as well
                adjacentNode.entities.Add(new EntityFootprint() { ID = element, Position = p });
            }
        }

        public IntPoint FindEntityNodeInAdjacents(int entity, IntPoint centerPoint)
        {
            for (int i = 0; i < Mathm.Adjacents.Length; i++)
            {
                IntPoint searchPoint = Mathm.Adjacents[i] + centerPoint;
                if (nodes[searchPoint.X, searchPoint.Y].Find(entity) != -1)
                {
                    return searchPoint;
                }
            }
            return new IntPoint(-1, -1);
        }

        // Translates a point in world space to the node coordinate.
        public IntPoint FindNode(Point p)
        {
            return new IntPoint((int)(p.X / NodeWidth), (int)(p.Y / NodeWidth));
        }
    }

    internal class GridNode
    {
        // TODO: Sort this regularly so that we can binary search
        public List<EntityFootprint> entities = new List<EntityFootprint>();

        // TODO: Swap to binary search
        public int Find(int entity)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].ID == entity)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool UpdateEntity(int entity, Point p)
        {
            int index = Find(entity);
            // If this entity exists, update its position
            if (index != -1)
            {
                entities[index] = new EntityFootprint() { ID = entity, Position = p };
                return true;
            }

            return false;
        }
    }

    struct EntityFootprint
    {
        public int ID;
        public Point Position;
    }
}
