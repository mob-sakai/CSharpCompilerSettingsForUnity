/*
# Native-sized integers

Language support for a native-sized signed and unsigned integer types.
The motivation is for interop scenarios and for low-level libraries.

*/

using NUnit.Framework;

namespace CSharp_9_Features
{
    internal partial class Tests
    {
        [Test]
        public static void NativeSizedIntegers_Test_1()
        {
            nint x = 3;
            nint y = 4;

            var actual = x + y;
            var expected = (nint) 7;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void NativeSizedIntegers_Test_2()
        {
            nint x = 3;
            nint y = 4;

            var actual = (int) (x + y);
            var expected = 7;

            Assert.AreEqual(expected, actual);
        }
    }
}
