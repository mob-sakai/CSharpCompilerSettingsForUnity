using NUnit.Framework;
using UnityEngine;

public class ModifySymbolTests
{
    [Test]
    public void Define_ADD_SYMBOL()
    {
#if ADD_SYMBOL
        Debug.Log("ADD_SYMBOL is defined.");
#else
            Assert.That(false, "ADD_SYMBOL is not defined.");
#endif
    }

    [Test]
    public void Undefine_UNITY_5_3_OR_NEWER()
    {
#if !UNITY_5_3_OR_NEWER
        Debug.Log("UNITY_5_3_OR_NEWER is not defined.");
#else
            Assert.That(false, "UNITY_5_3_OR_NEWER is defined.");
#endif
    }
}
