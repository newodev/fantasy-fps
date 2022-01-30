using CSharp_ECS.ECSCore;
using CSharp_ECS;



Region r = new Region();

JobSystem s = new TestSystem();
s.SetRegion(r);

Random rand = new Random();

r.SpawnEntity(new A());
r.ResolveBuffers();
QueryResult q = r.Query(new HashSet<Type> { typeof(A) });
q.GetComponent<B>(0);
/*

for (int i = 0; i < 100000; i++)
{
    r.SpawnEntity(new A() { lol = rand.Next(15) });
}

bool loop = true;
while (loop)
{
    r.Archetypes[0].ResolveBuffers();
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

*/