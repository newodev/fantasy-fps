using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    public class ECSWorld
    {
        // TODO: determine networking transport to use

        private List<JobSystem> systems;
        private Region region;

        private void ECSInit()
        {
            // Add all systems
        }
    }
}
