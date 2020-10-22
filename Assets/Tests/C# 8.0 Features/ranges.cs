/*
NOTE: IndexRange(https://www.nuget.org/packages/IndexRange/) or .Net Standard 2.1 is required.

# ranges

This feature is about delivering two new operators that allow constructing `System.Index` and `System.Range` objects, and using them to index/slice collections at runtime.
*/

using System;
using NUnit.Framework;

namespace CSharp_8_Features
{
    internal partial class Tests
    {
        [Test]
        public static void Ranges_Test_1()
        {
            var array = new[] {1, 2, 3, 4, 5, 6};
#if NET_STANDARD_2_1
            var actual = array[1..^1];
#else
            var actual = array.AsSpan()[1..^1].ToArray();
#endif
            var expected = new[] {2, 3, 4, 5};

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
