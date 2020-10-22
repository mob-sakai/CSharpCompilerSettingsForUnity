/*
# stack allocation

We modify the section [*Stack allocation*](https://github.com/dotnet/csharplang/blob/master/spec/unsafe-code.md#stack-allocation) of the C# language specification to relax the places when a `stackalloc` expression may appear.
*/

using System;
using NUnit.Framework;

namespace CSharp_8_Features
{
    internal partial class Tests
    {
        [Test]
        public static void StackAllocation_Test_1()
        {
            Span<int> numbers = stackalloc[] {1, 2, 3, 4, 5, 6};
            var actual = numbers.IndexOfAny(stackalloc[] {2, 4, 6, 8});
            var expected = 1;

            Assert.AreEqual(expected, actual);
        }

        static int M(Span<byte> buf) => 0;

        [Test]
        public static void StackAllocation_Test_2()
        {
            static int M(Span<int> buf) => 0;

            var actual = M(stackalloc[] {2, 4, 6, 8});
            var expected = 0;

            Assert.AreEqual(expected, actual);
        }
    }
}
