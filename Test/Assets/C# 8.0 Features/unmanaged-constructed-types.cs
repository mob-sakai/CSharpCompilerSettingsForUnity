/*
# unmanaged constructed types

In C# 8.0, we extended the concept of an *unmanaged* type to include constructed (generic) types.
This is a placeholder for its specification.
*/

using System.Collections.Generic;

namespace CSharp_8_Features.UnmanagedConstructedTypes
{
    class Program
    {
        unsafe static void Main()
        {
            var kv = new KeyValuePair<int, int>(1, 2);
            KeyValuePair<int, int>* pkv = &kv;
        }
    }
}
