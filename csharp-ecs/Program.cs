using CSharp_ECS.ECSCore;
using CSharp_ECS;

Region r = new Region();

JobSystem s = new TestSystem();
s.SetRegion(r);

Random rand = new Random();

for (int i = 0; i < 10000; i++)
{
    r.SpawnEntity(new A() { lol = rand.Next(15) });
}

bool loop = true;
while (loop)
{
    s.Update();

    Console.Clear();
    QueryResult q = r.Query<A>();
    for (int i = 0; i < 15; i++)
    {
        Console.WriteLine(q.GetComponent<A>(i).lol);
    }
    char input = Console.ReadKey(true).KeyChar;
    if (input == 's')
        loop = false;
}