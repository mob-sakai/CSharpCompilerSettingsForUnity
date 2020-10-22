/*
# static local functions

Support local functions that disallow capturing state from the enclosing scope.
*/

using NUnit.Framework;

namespace CSharp_8_Features
{
    internal partial class Tests
    {
        [Test]
        public static void StaticLocalFunctions_Test()
        {
            // a local function without closure (static) -> OK.
            static int ToDouble(int x) => x * 2;

            // a local function with closure (static) -> NG.
            // int a = 2;
            // static int ToDouble(int x) => x * a;

            var actual = ToDouble(100);
            var expected = 200;

            Assert.AreEqual(expected, actual);
        }
    }
}
