using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore
{
    interface IComponent
    {
        public int Id { set; get; }
    }

    // Test component implementation
    struct A : IComponent
    {
        public int Id { set; get; }
        public int lol;
    }
}
