/*
# alternative interpolated verbatim strings

In C# 8.0 we added a feature that permits an interpolated verbatim string to be introduced with the characters `@$"` or the characters `$@"`.  This is a placeholder for its specification.
*/

namespace CSharp_8_Features.AlternativeInterpolatedVerbatimStrings
{
    class Program
    {
        public void X()
        {
            var x = 123456;
            var y = 987654;
            var verbatim1 = $@"
verbatim (here) string
{x}, {y}, {x:c}, {x:n}
";

            var verbatim2 = @$"
verbatim (here) string
{x}, {y}, {x:c}, {x:n}
";
        }
    }
}
// #endif
