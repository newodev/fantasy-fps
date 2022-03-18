using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore;

public abstract class JobSystem
{
    protected Region region;
    public void SetRegion(Region r)
    {
        region = r;
    }

    // Called every game tick
    public virtual void Update()
    {

    }
}
