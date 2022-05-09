using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.Exceptions;

internal class ECSArchetypeException : ECSException
{
    public ECSArchetypeException(Type t, byte key, string function) : base($"Archetype of key {key} does not contain component type {t.Name} (function {function} )") { }
}
