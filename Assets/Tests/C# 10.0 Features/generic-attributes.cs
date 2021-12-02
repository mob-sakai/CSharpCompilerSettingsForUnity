/*
# Generic Attributes
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/generic-attributes

When generics were introduced in C# 2.0, attribute classes were not allowed to participate. We can make the language more composable by removing (rather, loosening) this restriction.
*/
#if CUSTOM_COMPILE
#if !ENABLE_IL2CPP
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace CSharp_10_Features
{
    public class Attr<T1> : Attribute { }

    public class Cs10_GenericAttributes
    {
        [Attr<int>]
        // [Attr<T>] // error
        void M<T>()
        {

        }
    }
}
#endif
#endif