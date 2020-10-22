/*
[NOT SUPPORTED] >> .Net Standard 2.1 is required.

# default interface methods

Add support for _virtual extension methods_ - methods in interfaces with concrete implementations. A class or struct that implements such an interface is required to have a single _most specific_ implementation for the interface method, either implemented by the class or struct, or inherited from its base classes or interfaces. Virtual extension methods enable an API author to add methods to an interface in future versions without breaking source or binary compatibility with existing implementations of that interface.
These are similar to Java's ["Default Methods"](http://docs.oracle.com/javase/tutorial/java/IandI/defaultmethods.html).
(Based on the likely implementation technique) this feature requires corresponding support in the CLI/CLR. Programs that take advantage of this feature cannot run on earlier versions of the platform.
*/

using NUnit.Framework;

namespace CSharp_8_Features
{
    interface IDefault
    {
#if NET_STANDARD_2_1
        int X() => 0;
#else
        int X();
#endif
    }

    class WithoutImplement : IDefault
    {
#if !NET_STANDARD_2_1
        public int X() => 1;
#endif
    }

    class WithImplement : IDefault
    {
        // overwrite -> OK.
        public int X() => 2;
    }


    internal partial class Tests
    {
        [Test]
        public static void DefaultInterfaceMethod_WithoutImplement()
        {
            var actual = new WithoutImplement().X();
            var expected = 1;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void DefaultInterfaceMethod_WithImplement()
        {
            var actual = new WithImplement().X();
            var expected = 2;

            Assert.AreEqual(expected, actual);
        }
    }
}
