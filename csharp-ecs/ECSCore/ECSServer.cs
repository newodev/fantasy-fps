using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS;

public class ECSServer
{
    private ECSWorld World;
    // Some list of client connections


    public ECSServer()
    {
        World = new ECSWorld(new JobSystem[0]);
    }

    private void NetSend()
    {
        // Send relevant changes to clients
    }
}
