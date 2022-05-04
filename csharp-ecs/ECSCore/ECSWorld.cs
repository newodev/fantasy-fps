﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS;

public class ECSWorld
{
    // All systems run by this game instance
    private List<JobSystem> systems = new();
    // The region owned/observed by this server/client
    private Region region = new();

    public ECSWorld(List<JobSystem> s)
    {
        systems = s;
        for (int i = 0; i < systems.Count; i++)
        {
            systems[i].SetRegion(region);
        }
    }

    public void Init()
    {
        for (int i = 0; i < systems.Count; i++)
        {
            systems[i].Init();
        }
        region.ResolveBuffers();
    }

    public void Update()
    {
        for (int i = 0; i < systems.Count; i++)
        {
            systems[i].Update();
        }

        region.ResolveBuffers();
    }

    public void FrameUpdate()
    {
        for (int i = 0; i < systems.Count; i++)
        {
            systems[i].FrameUpdate();
        }
    }

}
