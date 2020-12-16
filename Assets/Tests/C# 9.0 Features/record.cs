/*
# Records

Record types are reference types, similar to a class declaration. It is an error for a record to provide
a `record_base` `argument_list` if the `record_declaration` does not contain a `parameter_list`.
At most one partial type declaration of a partial record may provide a `parameter_list`.
It is an error for a `parameter_list` to be empty.
*/

using NUnit.Framework;

namespace CSharp_9_Features
{
    // NOTE: Primary constructor needs .Net 5+
    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/records#primary-constructor
    // public record Person(string FirstName, string LastName);

    public record Person
    {
        public string LastName { get; }
        public string FirstName { get; set; }
        public Person(string first, string last) => (FirstName, LastName) = (first, last);

        public void Deconstruct(out string first, out string last)
        {
            first = FirstName;
            last = LastName;
        }
    }

    internal partial class Tests
    {
        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public static void Record(string firstName, string lastName)
        {
            var p = new Person(firstName, lastName);

            Assert.AreEqual(p.FirstName, firstName);
            Assert.AreEqual(p.LastName, lastName);
        }

        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public static void Record_ToString(string firstName, string lastName)
        {
            var p = new Person(firstName, lastName);

            Assert.AreEqual($"Person {{ LastName = {lastName}, FirstName = {firstName} }}", p.ToString());
        }

        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public static void Record_Deconstruct(string firstName, string lastName)
        {
            var p = new Person(firstName, lastName);
            var (first, last) = p;

            Assert.AreEqual(firstName, first);
            Assert.AreEqual(lastName, last);
        }

        [TestCase("Foo", "Bar")]
        [TestCase("HogeHoge", "FugaFuga")]
        public static void Record_With(string firstName, string lastName)
        {
            var p = new Person(firstName, lastName);
            var (first, last) = p with { FirstName = "Void"};

            Assert.AreEqual("Void", first);
            Assert.AreEqual(lastName, last);
        }
    }
}
