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
 *  - Window
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
