/*
# Target-typed `new` expressions

Do not require type specification for constructors when the type is known.
*/
#if CUSTOM_COMPILE

using System.Collections.Generic;
using NUnit.Framework;

namespace CSharp_9_Features
{
    public class Cs9_TargetTypedNew
    {
        [Test]
        public void Test()
        {
            Dictionary<int, Dictionary<string, object>> obj = new ();

            Assert.IsNotNull(obj);
        }
    }
}
#endif
