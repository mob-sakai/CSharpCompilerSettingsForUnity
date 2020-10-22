using NUnit.Framework;
using UnityEngine;

namespace ModifySymbols
{
    public class Tests
    {
        [Test]
        public void Define_CSHARP_7_OR_NEWER()
        {
#if CSHARP_7_OR_NEWER
            Debug.Log("CSHARP_7_OR_NEWER is defined.");
#else
            Assert.That(false, "CSHARP_7_OR_NEWER is not defined.");
#endif
        }

        [Test]
        public void Define_CSHARP_7_1_OR_NEWER()
        {
#if CSHARP_7_1_OR_NEWER
            Debug.Log("CSHARP_7_1_OR_NEWER is defined.");
#else
            Assert.That(false, "CSHARP_7_1_OR_NEWER is not defined.");
#endif
        }

        [Test]
        public void Define_CSHARP_7_2_OR_NEWER()
        {
#if CSHARP_7_2_OR_NEWER
            Debug.Log("CSHARP_7_2_OR_NEWER is defined.");
#else
            Assert.That(false, "CSHARP_7_2_OR_NEWER is not defined.");
#endif
        }

        [Test]
        public void Define_CSHARP_7_3_OR_NEWER()
        {
#if CSHARP_7_3_OR_NEWER
            Debug.Log("CSHARP_7_3_OR_NEWER is defined.");
#else
            Assert.That(false, "CSHARP_7_3_OR_NEWER is not defined.");
#endif
        }

        [Test]
        public void Define_CSHARP_8_OR_NEWER()
        {
#if CSHARP_8_OR_NEWER
            Debug.Log("CSHARP_8_OR_NEWER is defined.");
#else
            Assert.That(false, "CSHARP_8_OR_NEWER is not defined.");
#endif
        }

        [Test]
        public void Define_CSHARP_9_OR_NEWER()
        {
#if CSHARP_9_OR_NEWER
            Debug.Log("CSHARP_9_OR_NEWER is defined.");
#else
            Assert.That(false, "CSHARP_9_OR_NEWER is not defined.");
#endif
        }

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
}
