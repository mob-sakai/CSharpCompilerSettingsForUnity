/*
# Native-sized integers

Language support for a native-sized signed and unsigned integer types.
The motivation is for interop scenarios and for low-level libraries.

*/
#if CUSTOM_COMPILE

using NUnit.Framework;

namespace CSharp_9_Features
{
    public class Cs9_NativeSizedIntegers
    {
        [Test]
        public void CastToNint()
        {
            nint x = 3;
            nint y = 4;

            var actual = x + y;
            var expected = (nint) 7;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CastToInt()
        {
            nint x = 3;
            nint y = 4;

            var actual = (int) (x + y);
            var expected = 7;

            Assert.AreEqual(expected, actual);
        }
    }
}
#endif
