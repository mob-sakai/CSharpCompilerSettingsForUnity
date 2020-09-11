/*
# nullable reference types

Nullable reference types have the same syntax `T?` as the short form of nullable value types, but do not have a corresponding long form.
For the purposes of the specification, the current `nullable_type` production is renamed to `nullable_value_type`, and a `nullable_reference_type` production is added.
*/

namespace CSharp_8_Features.NullableReferenceTypes
{
    class Program
    {
        string NotNull() => "";
        string? MaybeNull() => null;

        int M(string s)
        {
            var s1 = NotNull();
            var s2 = MaybeNull();

            // without null check -> warning.
            return s.Length + s1.Length + s2.Length;
        }
    }
}
