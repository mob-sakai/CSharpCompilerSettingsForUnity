/*
# Global Using Directive
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/parameterless-struct-constructors

Syntax for a using directive is extended with an optional global keyword that can precede the using keyword.
*/
#if CUSTOM_COMPILE
global using System.Collections.Generic;
global using System.Linq;
global using NUnit.Framework;

namespace CSharp_10_Features
{
    public class Cs10_GlobalUsingDirective
    {
        [Test]
        public void DetaultUsing_Linq()
        {
            var list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            Assert.AreEqual(list.Count(x => x % 2 == 0), 5);
        }
    }
}
#endif
