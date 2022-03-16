using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using CSharp_ECS.ECSCore;

/*
 * Game:
 *  - ECSClient
 *      - Server Connections (updates ECSWorld based on server states)
 *      - ECSWorld
 *          - Regions of interest to player
 *          - Local prediction systems
 *  - Window
 *  
 * Server Architecture:
 *  - ECSServer
 *  - ECSServer
 *  - ECSServer
 *  - ECSServer
 *      - Client Connections (gathers client input, sends relevant updates to clients)
 *      - ECSWorld
 *          - Regions managed by server
 *          - Game systems
 *  - ECSUniverse
 *      - Holds entire game state
 *      - used to mediate connections between servers that interact
*/
namespace Game
{
    class Game
    {
        private ECSClient client;
        private Window window;

        public Game()
        {
            client = new ECSClient();
            window = new Window();
        }
    }
}
