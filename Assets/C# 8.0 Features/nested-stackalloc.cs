/*
# stack allocation

We modify the section [*Stack allocation*](https://github.com/dotnet/csharplang/blob/master/spec/unsafe-code.md#stack-allocation) of the C# language specification to relax the places when a `stackalloc` expression may appear.
*/

#if !NET_LEGACY
using System;

namespace CSharp_8_Features.StackAllocation
{
    class Program
    {
        static int M(Span<byte> buf) => 0;

        static void M(int len)
        {
            if (stackalloc byte[1] == stackalloc byte[1]) ;
            M(stackalloc byte[1]);
            M(len > 512 ? new byte[len] : stackalloc byte[len]);
        }
    }
}
#endif
