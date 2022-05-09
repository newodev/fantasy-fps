using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS;

// A single instance of this class exists on every player's client
// It handles the networked changes to the ECSWorld and sends input to the server
public class ECSClient
{
    // Some list of connections to servers

    /*
     * This ECSWorld:
     *  a. Contains culled regions of interest to the player
     *  b. Runs predictive systems on them
     *  c. Processes user input, which is then sent to the server by this ECSClient instance
     *  d. Data is recieved by this ECSClient regularly and compared with the local simulation to ensure synchronisation
    */
    private ECSWorld world;

    public ECSClient(JobSystem[] systems)
    {
        // Initialise some server connections

        world = new ECSWorld(systems);
    }
    public void Init(Initialiser init)
    {
        world.Init(init);
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
