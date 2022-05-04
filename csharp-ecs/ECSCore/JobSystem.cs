using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS;

public abstract class JobSystem
{
    protected Region region;

    public void SetRegion(Region r)
    {
        region = r;
    }

    public virtual void Init() { }

    // Called every game tick
    public virtual void Update() { }

    public virtual void FrameUpdate() { }
}
