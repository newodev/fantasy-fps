using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore;

public class ECSWorld
{
    // All systems run by this game instance
    private List<JobSystem> systems;
    // All regions owned/observed by this server/client
    private List<Region> regions;

    private void ECSInit()
    {
        // Add all systems
    }
}
