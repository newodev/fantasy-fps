using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS;

public interface IComponent
{
    public int Id { get; set; }
}

// Test component implementation
struct A : IComponent
{
    public int Id { get; set; }
    public int lol;
}

struct B : IComponent
{
    public int Id { get; set; }
    public int lol;
}
