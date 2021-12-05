#if CUSTOM_COMPILE
using NUnit.Framework;

namespace SourceGenerator
{
    public class Unit
    {
        [Test]
        public void UnitToString()
        {
            var userId = new UserId(1234);
            Assert.AreEqual("UserId(1234)", userId.ToString());
        }

        [Test]
        public void AsPrimitive()
        {
            var userId = new UserId(1234);
            Assert.AreEqual(1234, userId.AsPrimitive());
        }
    }

    [UnitGenerator.UnitOf(typeof(int))]
    public readonly partial struct UserId { }
}
#endif
