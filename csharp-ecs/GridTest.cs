using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.SpatialIndexing;
using OpenTK.Mathematics;

namespace CSharp_ECS;

/*
class GridTest
{
    Grid grid;

    public GridTest()
    {
        grid = new Grid(5, 5, 1);

        int id = 0;
        for (float y = 0; y < 5; y++)
        {
            for (float x = 0; x < 5; x++)
            {
                id++;
                grid.Insert(id, new Vector2(x + 0.5f, y + 0.5f));
            }
        }

        List<int> query = grid.Query(new Vector2(1.5f, 1.5f), 1.1f);
        for (int i = 0; i < query.Count; i++)
        {
            Console.WriteLine(query[i]);
        }
    }
}
*/