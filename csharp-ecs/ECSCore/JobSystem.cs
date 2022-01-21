using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    abstract class JobSystem
    {
        protected Region region;
        public void SetRegion(Region r)
        {
            region = r;
        }

        /// <summary>
        /// Called every game tick. Tick rate is dynamic.
        /// </summary>
        public virtual void Update()
        {

        }
    }
}
