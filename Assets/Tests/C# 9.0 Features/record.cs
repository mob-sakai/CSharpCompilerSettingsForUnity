/*
[NOT SUPPORTED] >> .Net 5 is required.

# Records

Record types are reference types, similar to a class declaration. It is an error for a record to provide
a `record_base` `argument_list` if the `record_declaration` does not contain a `parameter_list`.
At most one partial type declaration of a partial record may provide a `parameter_list`.
It is an error for a `parameter_list` to be empty.
*/

#if NET_5
namespace CSharp_9_Features.Records
{
    public record Person
    {
        public string LastName { get; }
        public string FirstName { get; }
        public Person(string first, string last) => (FirstName, LastName) = (first, last);
    }
}
#endif
