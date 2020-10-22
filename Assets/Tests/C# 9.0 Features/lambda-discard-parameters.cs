/*
# Lambda discard parameters

Allow discards (`_`) to be used as parameters of lambdas and anonymous methods.
For example:
- lambdas: `(_, _) => 0`, `(int _, int _) => 0`
- anonymous methods: `delegate(int _, int _) { return 0; }`
*/

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

namespace CSharp_9_Features
{
    internal partial class Tests
    {
        private static Action<object, int> onSomeThing;

        [Test]
        public static void LambdaDiscardParameters()
        {
            onSomeThing = (_, _) => Debug.Log("LambdaDiscardParameters");

            LogAssert.Expect(LogType.Log, "LambdaDiscardParameters");
            onSomeThing(null, 0);
        }
    }
}
