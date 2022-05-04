using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS;

// -------------- DIAGRAM ---------------
//
//                universe
//          _________|_________
//          |                 |
//       region1           region2
//          |                 |
//      archetypeAB      archetypeB
//      A (id=10001)     B (id=10010)
//      A (id=10011)
//      B (id=10001)
//      B (id=10011)
//
// --------------------------------------
// SPAWN ORDER:
// - region1.Spawn(AB)
// - region2.Spawn(B)
// - region1.Spawn(AB)


// A Universe describes the entire existence of the game world. It then divides the universe spatially into Regions. Each server unit can work on at least one Region.
// It acts as the database used by servers to communicate
static class Universe
{
    private static List<Region> Regions;
}
