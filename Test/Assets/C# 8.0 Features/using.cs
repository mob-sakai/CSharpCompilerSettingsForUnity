/*
# "pattern-based using" and "using declarations"

The language will add two new capabilities around the `using` statement in order to make resource
management simpler: `using` should recognize a disposable pattern in addition to `IDisposable` and add a `using`
declaration to the language.
*/

namespace CSharp_8_Features.Using
{
    using System;

    readonly struct DeferredMessage : IDisposable
    {
        private readonly string _message;
        public DeferredMessage(string message) => _message = message;
        public void Dispose() => Console.WriteLine(_message);
    }

    ref struct RefDisposable
    {
        public void Dispose()
        {
        }
    }

    class Program
    {
        static void Main()
        {
            // using var で、変数のスコープに紐づいた using になる。
            // スコープを抜けるときに Dispose が呼ばれる。
            using var a = new DeferredMessage("a");
            using var b = new DeferredMessage("b");

            Console.WriteLine("c");

            // c, b, a の順でメッセージが表示される
            using (new RefDisposable())
            {
            }
        }
    }
}
