/*
NOTE: UniTask or .NET Standard 2.1 is required.

# async streams

C# has support for iterator methods and async methods, but no support for a method that is both an iterator and an async method.
We should rectify this by allowing for `await` to be used in a new form of `async` iterator, one that returns an `IAsyncEnumerable<T>` or `IAsyncEnumerator<T>` rather than an `IEnumerable<T>` or `IEnumerator<T>`, with `IAsyncEnumerable<T>` consumable in a new `await foreach`.
An `IAsyncDisposable` interface is also used to enable asynchronous cleanup.
*/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace CSharp_8_Features
{
    public static class TaskExtensions
    {
        public static IEnumerator AsIEnumerator(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                throw task.Exception;
            }
        }
    }

    internal partial class Tests
    {
        [UnityTest]
        public IEnumerator AsyncStreams_Test_1()
        {
            var task = Task.Run(async () =>
            {
                var sw = Stopwatch.StartNew();
                await foreach (var i in CountUpWithDelay())
                {
                    sw.Stop();
                    Debug.Log($"{i}: {sw.ElapsedMilliseconds}ms");
                    Assert.GreaterOrEqual(sw.ElapsedMilliseconds, i * 10);
                    sw.Restart();
                }
            });

            yield return task.AsIEnumerator();
            Assert.That(true);
        }

        private static async IAsyncEnumerable<int> CountUpWithDelay()
        {
            for (int i = 1; i < 5; ++i)
            {
                await Task.Delay(10 * i);
                yield return i;
            }
        }
    }
}
