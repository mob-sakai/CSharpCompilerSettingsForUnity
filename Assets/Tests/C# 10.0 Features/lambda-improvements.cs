/*
# Lambda Improvements
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/lambda-improvements

Allow lambdas with attributes. Allow lambdas with explicit return type. Infer a natural delegate type for lambdas and method groups.
*/
#if CUSTOM_COMPILE
using System;

namespace CSharp_10_Features
{
    class HttpPostAttribute : Attribute { public HttpPostAttribute(string root) { } }
    class FromBodyAttribute : Attribute { public FromBodyAttribute() { } }

    public class Cs10_LambdaImprovements
    {
        void MapAction(Func<string, string> action) { }

        [Test]
        public void Test()
        {
            MapAction([HttpPost("/")] ([FromBodyAttribute] todo) => todo);
            MapAction([HttpPost("/")] (todo) => todo);
            MapAction(string (todo) => todo);
            MapAction(todo => todo);
        }
    }
}
#endif