/*
[NOT SUPPORTED] >> .Net 6.0 is required.

# Caller Argument Expression
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/caller-argument-expression

Allow developers to capture the expressions passed to a method, to enable better error messages in diagnostic/testing APIs and reduce keystrokes.
*/
#if NET_6
#if CUSTOM_COMPILE
using System.Runtime.CompilerServices;
namespace CSharp_10_Features
{
    public class Cs10_CallerArgumentExpression
    {
        private static readonly int[] array = new[] { 1 };

        [Test]
        public void Test()
        {
            Assert.AreEqual(Expression(array != null), "array != null");
            Assert.AreEqual(Expression(array.Length == 1), "array.Length == 1");
        }

        public string Expression<T>(T condition, [CallerArgumentExpression("condition")] string? expression = null)
        {
            return expression;
        }
    }
}
#endif
#endif