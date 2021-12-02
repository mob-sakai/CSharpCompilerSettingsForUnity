/*
# Record structs
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/record-structs

Record struct types are value types, like other struct types.
*/
#if CUSTOM_COMPILE

using System.Collections.Generic;
using NUnit.Framework;

namespace CSharp_10_Features
{
    public class Cs10_RecordStructs
    {
        record struct Person(string FirstName, string LastName){}

        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public void Construct(string firstName, string lastName)
        {
            var p = new Person(firstName, lastName);
            p.FirstName = firstName;

            Assert.AreEqual(p.FirstName, firstName);
            Assert.AreEqual(p.LastName, lastName);
        }

        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public void ToString(string firstName, string lastName)
        {
            var p = new Person(firstName, lastName);

            Assert.AreEqual($"Person {{ FirstName = {firstName}, LastName = {lastName} }}", p.ToString());
        }

        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public void Deconstruct(string firstName, string lastName)
        {
            var p = new Person(firstName, lastName);
            var (first, last) = p;

            Assert.AreEqual(firstName, first);
            Assert.AreEqual(lastName, last);
        }

        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public void With(string firstName, string lastName)
        {
            var p = new Person(firstName, lastName);
            var (first, last) = p with { FirstName = "Void"};

            Assert.AreEqual("Void", first);
            Assert.AreEqual(lastName, last);
        }
    }
}
#endif
