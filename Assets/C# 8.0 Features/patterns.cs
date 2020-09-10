/*
# recursive pattern matching and switch

Pattern matching extensions for C# enable many of the benefits of algebraic data types and pattern matching from functional languages, but in a way that smoothly integrates with the feel of the underlying language.
Elements of this approach are inspired by related features in the programming languages [F#](http://www.msr-waypoint.net/pubs/79947/p29-syme.pdf "Extensible Pattern Matching Via a Lightweight Language") and [Scala](https://link.springer.com/content/pdf/10.1007%2F978-3-540-73589-2.pdf "Matching Objects With Patterns, page 273").
*/

#if !NET_LEGACY

namespace CSharp_8_Features.Patterns
{
    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x = 0, int y = 0) => (X, Y) = (x, y);
        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
    }

    class Program
    {
        static int M(object obj)
            => obj switch
            {
                0 => 1,
                int i => 2,
                Point (1, _) => 4, // position pattern
                Point { X: 2, Y: var y } => y, // property pattern
                _ => 0
            };

        public int M2(int? x, int? y)
            => (x, y) switch
            {
                (int i, int j) => i.CompareTo(j),
                ({ }, null) => 1,
                (null, { }) => -1,
                (null, null) => 0
            };
    }
}
#endif
