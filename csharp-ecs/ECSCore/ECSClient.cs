﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore;

// A single instance of this class exists on every player's client
// It handles the networked changes to the ECSWorld and sends input to the server
public class ECSClient
{
    // Some list of connections to servers

    /*
     * This ECSWorld:
     *  a. Contains culled regions of interest to the player
     *  b. Runs predictive systems on them
    */
    private ECSWorld world;

    public ECSClient(List<JobSystem> systems)
    {
        // Initialise some server connections

        world = new ECSWorld(systems);
    }
    public void Init()
    {
        world.Init();
    }
    public void Update()
    {
        world.Update();
    }

    public void FrameUpdate()
    {
        world.FrameUpdate();
    }
}
