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
        int iteration = 0;
        public override void Update()
        {
            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Frame: {iteration}");
                region.Query((ComponentCollection<A> a, ComponentCollection<B> b) =>
                {
                    Parallel.For(0, a.Count, (int i) =>
                    {
                        a[i] = new A() { lol = a[i].lol + rand.Next(-1, 2) };
                        int val = (int)Math.Sqrt(iteration * iteration + a[i].lol);
                        b[i] = new B() { lol = val };

                        if (i < 20)
                            Console.WriteLine($"{a[i].Id}: {a[i].lol}, {b[i].lol}           ");
                    });
                });
                iteration++;
            }
          
        }
    }
}
