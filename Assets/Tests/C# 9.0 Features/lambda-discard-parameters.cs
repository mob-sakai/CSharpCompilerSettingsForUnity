/*
# Lambda discard parameters

Allow discards (`_`) to be used as parameters of lambdas and anonymous methods.
For example:
- lambdas: `(_, _) => 0`, `(int _, int _) => 0`
- anonymous methods: `delegate(int _, int _) { return 0; }`
*/
#if CUSTOM_COMPILE

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

namespace CSharp_9_Features
{
    public class Cs9_LambdaDiscardParameters
    {
        private static Action<object, int> onSomeThing;

        [Test]
        public void Test()
        {
            onSomeThing = (_, _) => Debug.Log("LambdaDiscardParameters");
            onSomeThing(null, 0);
        }
    }
}
#endif
