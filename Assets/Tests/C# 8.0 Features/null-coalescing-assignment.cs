/*
# null coalescing assignment

Simplifies a common coding pattern where a variable is assigned a value if it is null.
As part of this proposal, we will also loosen the type requirements on `??` to allow an expression whose type is an unconstrained type parameter to be used on the left-hand side.
*/
#if CUSTOM_COMPILE
using NUnit.Framework;

namespace CSharp_8_Features
{
    public class Cs8_NullCoalescingAssignment
    {
        [Test]
        public void Null()
        {
            string actual = null;
            actual ??= "default string";
            var expected = "default string";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NotNull()
        {
            string actual = "default string";
            actual ??= "other string";
            var expected = "default string";

            Assert.AreEqual(expected, actual);
        }
    }
}
#endif
