using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    // A single instance of this class exists on every player's client
    // It handles the networked changes to the ECSWorld and sends input to the server
    public class ECSClient : ECSWorld
    {
        // Some list of connections to servers

        /*
         * This ECSWorld:
         *  a. Contains culled regions of interest to the player
         *  b. Runs predictive systems on them
        */
        private ECSWorld world = new ECSWorld();

        public ECSClient()
        {
            // Initialise some server connections
            // Initialise systems, ECSworld
        }
    }
}
