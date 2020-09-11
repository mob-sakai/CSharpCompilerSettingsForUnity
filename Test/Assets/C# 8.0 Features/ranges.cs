/*
NOTE: IndexRange(https://www.nuget.org/packages/IndexRange/) or .Net Standard 2.1 is required.

# ranges

This feature is about delivering two new operators that allow constructing `System.Index` and `System.Range` objects, and using them to index/slice collections at runtime.
*/
#if !NET_LEGACY

using System;

namespace CSharp_8_Features.Ranges
{
    class Program
    {
        static void Main()
        {
            int[] array = new[] {1, 2, 3, 4, 5, 6};

            // For IndexRange:
            var slice = array.AsSpan()[1..^1];

            // For .Net Standard 2.1:
            // var slice = array[1..^1];
        }
    }
}
#endif
