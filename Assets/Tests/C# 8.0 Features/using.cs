/*
# "pattern-based using" and "using declarations"

The language will add two new capabilities around the `using` statement in order to make resource
management simpler: `using` should recognize a disposable pattern in addition to `IDisposable` and add a `using`
declaration to the language.
*/
#if CUSTOM_COMPILE
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

namespace CSharp_8_Features
{
    public readonly struct ReadOnlyStructDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }

    public ref struct RefDisposable
    {
        public void Dispose()
        {
        }
    }

    public class Cs8_Using
    {
        [Test]
        public void Test()
        {
            using var a = new ReadOnlyStructDisposable();
            using var b = new ReadOnlyStructDisposable();

            using (new RefDisposable())
            {
            }
        }
    }
}
#endif
