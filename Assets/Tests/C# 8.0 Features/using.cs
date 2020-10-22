/*
# "pattern-based using" and "using declarations"

The language will add two new capabilities around the `using` statement in order to make resource
management simpler: `using` should recognize a disposable pattern in addition to `IDisposable` and add a `using`
declaration to the language.
*/

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

namespace CSharp_8_Features
{
    readonly struct DeferredMessage : IDisposable
    {
        private readonly string _message;
        public DeferredMessage(string message) => _message = message;
        public void Dispose() => Debug.Log(_message);
    }

    ref struct RefDisposable
    {
        public void Dispose()
        {
        }
    }

    internal partial class Tests
    {
        [Test]
        public static void Using_Test()
        {
            // log order: c -> b -> a
            LogAssert.Expect(LogType.Log, "c");
            LogAssert.Expect(LogType.Log, "b");
            LogAssert.Expect(LogType.Log, "a");

            using var a = new DeferredMessage("a");
            using var b = new DeferredMessage("b");

            Debug.Log("c");

            using (new RefDisposable())
            {
            }
        }
    }
}
