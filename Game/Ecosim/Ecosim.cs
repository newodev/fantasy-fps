using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS;

namespace Game.Ecosim;

// A ecosystem/economy simulation used to generate events, encounters, resource rates, etc
// The ecosim runs on a seperate ECS instance
class Ecosim
{
    private EcosimSystem[] systems =
    {

    };
    private ECSWorld ecosimWorld;

    public Ecosim()
    {
        ecosimWorld = new ECSWorld(systems);
    }
}
