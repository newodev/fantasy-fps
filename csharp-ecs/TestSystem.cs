using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS;

namespace CSharp_ECS;

class TestSystem : JobSystem
{
    int iteration = 0;
    public override void Update()
    {
        Random rand = new Random();
        Console.SetCursorPosition(0, 1);
        Console.WriteLine($"Frame: {iteration}");
        for (int i = 0; i < 10; i++)
        {
            region.Query((ComponentArray<A> a, ComponentArray<B> b) =>
            {
                Parallel.For(0, a.Count, (int i) =>
                {
                    a[i] = new A() { lol = a[i].lol + rand.Next(-1, 2) };
                    int val = (int)Math.Sqrt(iteration * iteration + a[i].lol);
                    val = (int)Math.Pow(val, 10);
                    val = (int)Math.Pow(val, -10);
                    b[i] = new B() { lol = val };
                });
            });
            iteration++;
        }

    }
}
