/*
[NOT SUPPORTED] >> .Net Standard 2.1 is required.

# default interface methods

Add support for _virtual extension methods_ - methods in interfaces with concrete implementations. A class or struct that implements such an interface is required to have a single _most specific_ implementation for the interface method, either implemented by the class or struct, or inherited from its base classes or interfaces. Virtual extension methods enable an API author to add methods to an interface in future versions without breaking source or binary compatibility with existing implementations of that interface.
These are similar to Java's ["Default Methods"](http://docs.oracle.com/javase/tutorial/java/IandI/defaultmethods.html).
(Based on the likely implementation technique) this feature requires corresponding support in the CLI/CLR. Programs that take advantage of this feature cannot run on earlier versions of the platform.
*/

#if NET_STANDARD_2_1

using System;

namespace CSharp_8_Features.DefaultInterfaceMethod
{
    interface I
    {
        void X();

        // default interface methods.
        void Y()
        {
        }
    }

    class A : I
    {
        // implement only X() -> OK.
        public void X()
        {
        }
    }

    class B : I
    {
        public void X()
        {
        }

        // overwrite Y() -> OK.
        public void Y() => Console.WriteLine("B");
    }

    class Program
    {
        static void Main() => M(new B());
        static void M(I i) => i.Y();
    }
}
#endif
