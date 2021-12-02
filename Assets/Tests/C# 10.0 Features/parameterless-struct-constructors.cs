/*
# Parameterless struct constructors
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/parameterless-struct-constructors

Support parameterless constructors and instance field initializers for struct types.
*/
#if CUSTOM_COMPILE

using System.Collections.Generic;
using NUnit.Framework;

namespace CSharp_10_Features
{
    public class Cs10_ParameterlessStructConstructors
    {
        record struct Person()
        {
            public string FirstName { get; init; } = default;
            public string LastName { get; init; } = default;
        }

        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public void Construct(string firstName, string lastName)
        {
            var p = new Person() { FirstName = firstName, LastName = lastName };

            Assert.AreEqual(p.FirstName, firstName);
            Assert.AreEqual(p.LastName, lastName);
        }

        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public void ToString(string firstName, string lastName)
        {
            var p = new Person() { FirstName = firstName, LastName = lastName };

            Assert.AreEqual($"Person {{ FirstName = {firstName}, LastName = {lastName} }}", p.ToString());
        }

        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public void With(string firstName, string lastName)
        {
            var p = new Person() { FirstName = firstName, LastName = lastName };
            var p2 = p with { FirstName = "Void" };

            Assert.AreEqual(p2.FirstName, "Void");
            Assert.AreEqual(p2.LastName, lastName);
        }
    }
}
#endif
