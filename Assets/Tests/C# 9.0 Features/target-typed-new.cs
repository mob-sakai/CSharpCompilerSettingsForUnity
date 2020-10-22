/*
# Target-typed `new` expressions

Do not require type specification for constructors when the type is known.
*/

using System.Collections.Generic;
using NUnit.Framework;

namespace CSharp_9_Features
{
    internal partial class Tests
    {
        [Test]
        public static void TargetTypedNew()
        {
            Dictionary<int, Dictionary<string, object>> obj = new ();

            Assert.IsNotNull(obj);
        }
    }
}
