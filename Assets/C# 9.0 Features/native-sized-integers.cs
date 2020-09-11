/*
# Native-sized integers

Language support for a native-sized signed and unsigned integer types.
The motivation is for interop scenarios and for low-level libraries.

*/

namespace CSharp_9_Features.NativeSizedIntegers
{
    public class Program
    {
        public static void M()
        {
            nint x = 3;
            nint.Equals(x, 3);
        }
    }
}
