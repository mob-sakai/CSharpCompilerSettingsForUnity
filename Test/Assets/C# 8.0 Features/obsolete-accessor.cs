/*
# obsolete on property accessor

In C# 8.0, we added support for declaring a property accessor `[Obsolete]`.
This is a placeholder for the specification.
*/

using System;

namespace CSharp_8_Features.ObsoleteOnPropertyAccessor
{
    class A
    {
        public int X { get; [Obsolete] set; }
    }
}
