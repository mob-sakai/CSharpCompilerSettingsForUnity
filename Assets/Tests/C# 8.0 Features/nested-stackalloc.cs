/*
# stack allocation

We modify the section [*Stack allocation*](https://github.com/dotnet/csharplang/blob/master/spec/unsafe-code.md#stack-allocation) of the C# language specification to relax the places when a `stackalloc` expression may appear.
*/
#if CUSTOM_COMPILE
using System;
using NUnit.Framework;

namespace CSharp_8_Features
{
    public class Cs8_StackAllocation
    {
        [Test]
        public void ToSpan()
        {
            Span<int> numbers = stackalloc[] {1, 2, 3, 4, 5, 6};
            var actual = numbers.IndexOfAny(stackalloc[] {2, 4, 6, 8});
            var expected = 1;

            Assert.AreEqual(expected, actual);
        }

        static int M(Span<byte> buf) => 0;

        [Test]
        public void ToStackAlloc()
        {
            static int M(Span<int> buf) => 0;

            var actual = M(stackalloc[] {2, 4, 6, 8});
            var expected = 0;

            Assert.AreEqual(expected, actual);
        }
    }
}
#endif
