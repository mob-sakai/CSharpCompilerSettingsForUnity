/*
# Improved Definite Assignment Analysis
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/improved-definite-assignment

Definite assignment analysis as specified has a few gaps which have caused users inconvenience.
*/
#if CUSTOM_COMPILE
using NUnit.Framework;
#nullable enable

namespace CSharp_10_Features
{
    public class Cs10_ImprovedDefiniteAssignmentAnalysis
    {
        public void Main()
        {
            C c = new C();
            if (c?.M(out object obj3) == true)
            {
                obj3.ToString(); // undesired error
            }

            if (c?.M(out object obj4) ?? false)
            {
                obj4.ToString(); // undesired error
            }
            if (c != null ? c.M(out object obj) : false)
            {
                obj.ToString(); // undesired error
            }
        }
    }

    public class C
    {
        public bool M(out object obj)
        {
            obj = new object();
            return true;
        }
    }
}
#endif