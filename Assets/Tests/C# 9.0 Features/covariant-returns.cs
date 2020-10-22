/*
[NOT SUPPORTED] >> .Net 5 is required.

# Covariant returns

Support _covariant return types_. Specifically, permit the override of a method to declare a more derived return type than the method it overrides, and similarly to permit the override of a read-only property to declare a more derived type. Override declarations appearing in more derived types would be required to provide a return type at least as specific as that appearing in overrides in its base types. Callers of the method or property would statically receive the more refined return type from an invocation.

*/

#if NET_5
namespace CSharp_9_Features.CovariantReturns
{
    class Compilation
    {
        private string options = "";

        public virtual Compilation WithOptions(string options)
        {
            this.options += $"{options},";
            return this;
        }
    }

    class CSharpCompilation : Compilation
    {
        private string csOptions = "";

        public override CSharpCompilation WithOptions(string options)
        {
            base.WithOptions(options);
            this.csOptions += $"{options}[cs],";
            return this;
        }
    }
}
#endif
