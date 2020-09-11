/*
# null coalescing assignment

Simplifies a common coding pattern where a variable is assigned a value if it is null.
As part of this proposal, we will also loosen the type requirements on `??` to allow an expression whose type is an unconstrained type parameter to be used on the left-hand side.
*/

using System;

namespace CSharp_8_Features.NullCoalescingAssignment
{
    class Program
    {
        static void M(string s = null)
        {
            s ??= "default string";
            Console.WriteLine(s);
        }
    }
}
