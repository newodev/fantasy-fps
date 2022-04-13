using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

static class Time
{
    public static float DeltaTime { get; private set; }

    public static void UpdateTime (float dt)
    {
        DeltaTime = dt;
    }
}
