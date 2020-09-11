/*
# Lambda discard parameters

Allow discards (`_`) to be used as parameters of lambdas and anonymous methods.
For example:
- lambdas: `(_, _) => 0`, `(int _, int _) => 0`
- anonymous methods: `delegate(int _, int _) { return 0; }`
*/

namespace CSharp_9_Features.LambdaDiscardParameters
{
    using System;

    public class Program
    {
        private static event Action<object, int> onSomeThing;

        public static void M()
        {
            onSomeThing += (_, _) => { };
        }
    }
}
