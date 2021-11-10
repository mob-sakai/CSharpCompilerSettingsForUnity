/*
# alternative interpolated verbatim strings

In C# 8.0 we added a feature that permits an interpolated verbatim string to be introduced with the characters `@$"` or the characters `$@"`.  This is a placeholder for its specification.
*/
#if CUSTOM_COMPILE
using NUnit.Framework;

namespace CSharp_8_Features
{
    public class Cs8_AlternativeInterpolatedVerbatim
    {
        [Test]
        public void DollerAt()
        {
            var x = 123456;
            var y = 987654;
            var actual = $@"
verbatim (here) string
{x}, {y}, {x:n},
";
            var expected = $@"
verbatim (here) string
123456, 987654, 123,456.00,
";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AtDoller()
        {
            var x = 123456;
            var y = 987654;
            var actual = @$"
verbatim (here) string
{x}, {y}, {x:n},
";
            var expected = @"
verbatim (here) string
123456, 987654, 123,456.00,
";
            Assert.AreEqual(expected, actual);
        }
    }
}
#endif
