using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore;

class ECSWorld
{
    // All systems run by this game instance
    private List<JobSystem> systems = new();
    // All regions owned/observed by this server/client
    private List<Region> regions = new();

    private void ECSInit()
    {
        // Add all systems
        systems.Add(new TestSystem());
    }

    public void RegisterRegion(Region r)
    {
        regions.Add(r);
    }
}
