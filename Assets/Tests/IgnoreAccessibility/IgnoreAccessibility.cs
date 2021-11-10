using System;
using NUnit.Framework;

namespace IgnoreAccessibility
{
    public class IgnoreAccessibilityContent
    {
        private int number = 999;
    }

    public class IgnoreAccessibilityTest
    {
        [Test]
        public void GetPrivateField()
        {
#if CUSTOM_COMPILE
            Assert.AreEqual(new IgnoreAccessibilityContent().number, 999);
#else
            throw new NotImplementedException();
#endif
        }
    }
}
