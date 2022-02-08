using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.SpatialIndexing
{
    // TODO: Add limits from 0, 0 to cellsX, cellsY
    class Grid
    {
        int GridWidth;
        int GridHeight;
        int CellWidth;
        GridNode[,] nodes;

        public Grid(int gridWidth, int gridHeight, int cellWidth)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            CellWidth = cellWidth;
            nodes = new GridNode[gridWidth, gridHeight];

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    nodes[x, y] = new GridNode();
                }
            }
        }
        public void Delete(int element, Point approximatePosition)
        {
            // Check to see if element is in the approximate node
            IntPoint nodePoint = FindNode(approximatePosition);
            GridNode node = nodes[nodePoint.X, nodePoint.Y];
            // If it is in that node, update it and return
            if (node.Remove(element))
            {
                return;
            }

            // Entity is not in approximate node, search adjacents
            IntPoint adjacentNodePoint = FindEntityNodeInAdjacents(element, nodePoint);
            if (adjacentNodePoint != new IntPoint(-1, -1))
            {
                GridNode adjacentNode = nodes[adjacentNodePoint.X, adjacentNodePoint.Y];

                node.Remove(element);

                return;
            }
        }

        public void Insert(int element, Point p)
        {
            IntPoint nodePoint = FindNode(p);
            GridNode node = nodes[nodePoint.X, nodePoint.Y];
            node.Add(element, p);
        }

        public List<int> Query(Point center, float radius)
        {
            List<int> result = new List<int>();
            int bottom = (int)((center.Y - radius) / CellWidth);
            int top = (int)((center.Y + radius) / CellWidth) + 1;
            int left = (int)((center.X - radius) / CellWidth);
            int right = (int)((center.X + radius) / CellWidth) + 1;

            if (bottom < 0 || left < 0)
                return null;
            if (right >= GridWidth || top >= GridHeight)
                return null;

            for (int y = bottom; y < top; y++)
            {
                for (int x = left; x < right; x++)
                {
                    GridNode node = nodes[x, y];
                    int[] ids = new int[node.entities.Count];
                    for (int i = 0; i < ids.Length; i++)
                    {
                        ids[i] = node.entities[i].ID;
                    }
                    result.AddRange(ids);
                }
            }

            return result;
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
                // Remove from old node, add to new
                node.Remove(element);
                adjacentNode.Add(element, p );

                return;
            }
        }

        public IntPoint FindEntityNodeInAdjacents(int entity, IntPoint centerPoint)
        {
            for (int i = 0; i < Mathm.Adjacents.Length; i++)
            {
                IntPoint searchPoint = Mathm.Adjacents[i] + centerPoint;
                GridNode searchNode = nodes[searchPoint.X, searchPoint.Y];
                if (searchNode.Find(entity) != -1)
                {
                    return searchPoint;
                }
            }
            return new IntPoint(-1, -1);
        }

        // Translates a point in world space to the node coordinate.
        public IntPoint FindNode(Point p)
        {
            return new IntPoint((int)Math.Floor(p.X / CellWidth), (int)Math.Floor(p.Y / CellWidth));
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

        // Returns true and removes an entity if it is in this node.
        public bool Remove(int entity)
        {
            int index = Find(entity);
            if (index != -1)
            {
                entities.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void Add(int entity, Point p)
        {
            EntityFootprint footprint = new EntityFootprint() { ID = entity, Position = p };
            entities.Add(footprint);
        }

        // Returns true and updates it with a new position if it is in this node.
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
