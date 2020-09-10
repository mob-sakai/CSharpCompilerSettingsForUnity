/*
NOTE: UniTask or .NET Standard 2.1 is required.

# async streams

C# has support for iterator methods and async methods, but no support for a method that is both an iterator and an async method.
We should rectify this by allowing for `await` to be used in a new form of `async` iterator, one that returns an `IAsyncEnumerable<T>` or `IAsyncEnumerator<T>` rather than an `IEnumerable<T>` or `IEnumerator<T>`, with `IAsyncEnumerable<T>` consumable in a new `await foreach`.
An `IAsyncDisposable` interface is also used to enable asynchronous cleanup.
*/

#if !NET_LEGACY

using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

namespace CSharp_8_Features.AsyncStreams
{
    class Program
    {
        static async UniTask Main()
        {
            await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate())
            {
                Console.WriteLine("Update");
            }
        }
    }
}
#endif
