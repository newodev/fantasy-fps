using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS;

public class ECSWorld
{
    // All systems run by this game instance
    private JobSystem[] systems;
    // The region owned/observed by this server/client
    private Region region = new();

    public ECSWorld(JobSystem[] s)
    {
        systems = s;
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].SetRegion(region);
        }
    }

    public void Init(Initialiser init)
    {
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Init();
        }
        init.InitEntities(region);
        region.ResolveBuffers();
    }

    public void Update()
    {
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Update();
        }

        region.ResolveBuffers();
    }

    public void FrameUpdate()
    {
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].FrameUpdate();
        }
    }

}
