/*
# Extended property patterns
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/extended-property-patterns

Allow property subpatterns to reference nested members, for instance.
*/
#if CUSTOM_COMPILE
using NUnit.Framework;

namespace CSharp_10_Features
{
    public class Cs10_ExtendedPropertyPatterns
    {
        struct ParentStruct
        {
            public ChildStruct Child { get; set; }
        }

        struct ChildStruct
        {
            public int Property { get; set; }

        }

        [Test]
        public void Test()
        {
            ParentStruct p = new() { Child = new() { Property = 999 } };

            if (p is ParentStruct { Child.Property: 999 })
            {
            }
            else
            {
                throw new System.NotSupportedException("Extended property patterns (C# 10)");
            }
        }
    }
}
#endif
