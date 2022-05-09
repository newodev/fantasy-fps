using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS;

public abstract class Initialiser
{
    public abstract JobSystem[] InitSystems();
    public abstract void InitEntities(Region region);
}
