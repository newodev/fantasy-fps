using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.Exceptions;

internal class ECSException : Exception
{
    public ECSException(string error) : base("ECSException: " + error) { }
}
