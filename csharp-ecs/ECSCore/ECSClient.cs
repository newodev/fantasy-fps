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

        private void NetRecieve()
        {
            // Get everything that happened on the server, solve desync
        }
    }
}
