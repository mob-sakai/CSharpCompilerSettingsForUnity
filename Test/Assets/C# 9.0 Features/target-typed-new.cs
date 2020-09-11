/*
# Target-typed `new` expressions

Do not require type specification for constructors when the type is known.
*/

namespace CSharp_9_Features.TargetTypedNew
{
    using System.Collections.Generic;

    public class Program
    {
        public static void M()
        {
            Dictionary<int, Dictionary<string, object>> _observations = new ();
        }
    }
}
