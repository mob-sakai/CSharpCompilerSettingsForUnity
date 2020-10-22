/*
# Pattern-matching changes for C# 9.0

We are considering a small handful of enhancements to pattern-matching for C# 9.0 that have natural synergy and work well to address a number of common programming problems:
- https://github.com/dotnet/csharplang/issues/2925 Type patterns
- https://github.com/dotnet/csharplang/issues/1350 Parenthesized patterns to enforce or emphasize precedence of the new combinators
- https://github.com/dotnet/csharplang/issues/1350 Conjunctive `and` patterns that require both of two different patterns to match;
- https://github.com/dotnet/csharplang/issues/1350 Disjunctive `or` patterns that require either of two different patterns to match;
- https://github.com/dotnet/csharplang/issues/1350 Negated `not` patterns that require a given pattern *not* to match; and
- https://github.com/dotnet/csharplang/issues/812 Relational patterns that require the input value to be less than, less than or equal to, etc a given constant.
*/

using NUnit.Framework;

namespace CSharp_9_Features
{
    public static class Extensions
    {
        public static bool IsLetter(this char c) => c is  >= 'a' and <= 'z' or >= 'A' and <= 'Z';
        public static bool IsNotNull<T>(this T t) => t is not null;
    }

    internal partial class Tests
    {
        [Test]
        public static void PatternMatching_AndOr_1()
        {
            var actual = $"'#' is letter ? {'#'.IsLetter()}";
            var expected = $"'#' is letter ? False";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void PatternMatching_AndOr_2()
        {
            var actual = $"'c' is letter ? {'c'.IsLetter()}";
            var expected = $"'c' is letter ? True";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void PatternMatching_Not_1()
        {
            object obj = null;
            var actual = $"obj is not null ? {obj.IsNotNull()}";
            var expected = $"obj is not null ? False";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void PatternMatching_Not_2()
        {
            object obj = "";
            var actual = $"obj is not null ? {obj.IsNotNull()}";
            var expected = $"obj is not null ? True";

            Assert.AreEqual(expected, actual);
        }
    }
}
