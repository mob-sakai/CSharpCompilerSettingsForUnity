/*
# recursive pattern matching and switch

Pattern matching extensions for C# enable many of the benefits of algebraic data types and pattern matching from functional languages, but in a way that smoothly integrates with the feel of the underlying language.
Elements of this approach are inspired by related features in the programming languages [F#](http://www.msr-waypoint.net/pubs/79947/p29-syme.pdf "Extensible Pattern Matching Via a Lightweight Language") and [Scala](https://link.springer.com/content/pdf/10.1007%2F978-3-540-73589-2.pdf "Matching Objects With Patterns, page 273").
*/

using NUnit.Framework;

namespace CSharp_8_Features
{
    internal class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x = 0, int y = 0) => (X, Y) = (x, y);
        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);
    }

    internal partial class Tests
    {
        [Test]
        public static void Patterns_Test_Position()
        {
            var actual = Pattern_1(new Point(1, 0));
            var expected = 4;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Patterns_Test_Property()
        {
            var actual = Pattern_1(new Point(2, 5));
            var expected = 5;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Patterns_Test_Value()
        {
            var actual = Pattern_1(0);
            var expected = 1;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Patterns_Test_Type()
        {
            var actual = Pattern_1(100);
            var expected = 2;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Patterns_Test_Default()
        {
            var actual = Pattern_1("text");
            var expected = 0;

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public static void Patterns_Test_NotNull()
        {
            var actual = Pattern_2(100, 200);
            var expected = 300;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Patterns_Test_NullLeft()
        {
            var actual = Pattern_2(100, null);
            var expected = 1;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Patterns_Test_NullRight()
        {
            var actual = Pattern_2(null, 200);
            var expected = -1;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void Patterns_Test_NullBoth()
        {
            var actual = Pattern_2(null, null);
            var expected = 0;

            Assert.AreEqual(expected, actual);
        }

        private static int Pattern_1(object obj)
            => obj switch
            {
                0 => 1,
                int i => 2,
                Point (1, _) => 4, // position pattern
                Point { X: 2, Y: var y } => y, // property pattern
                _ => 0
            };

        private static int Pattern_2(int? x, int? y)
            => (x, y) switch
            {
                (int i, int j) => i + j,
                ({ }, null) => 1,
                (null, { }) => -1,
                (null, null) => 0
            };
    }
}
