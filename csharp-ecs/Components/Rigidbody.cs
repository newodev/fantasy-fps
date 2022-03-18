using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace CSharp_ECS;

struct Rigidbody
{
    public int parentID;
    public Vector3 offsetFromParent;
    public Vector3 velocity;
}
