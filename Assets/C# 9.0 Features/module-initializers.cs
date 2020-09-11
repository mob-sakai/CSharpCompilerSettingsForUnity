/*
[NOT SUPPORTED] >> .Net 5 is required.

# Module Initializers

Although the .NET platform has a [feature](https://github.com/dotnet/runtime/blob/master/docs/design/specs/Ecma-335-Augments.md#module-initializer) that directly supports writing initialization code for the assembly (technically, the module), it is not exposed in C#.  This is a rather niche scenario, but once you run into it the solutions appear to be pretty painful.  There are reports of [a number of customers](https://www.google.com/search?q=.net+module+constructor+c%23&oq=.net+module+constructor) (inside and outside Microsoft) struggling with the problem, and there are no doubt more undocumented cases.
*/

#if NET_5

using System;
using System.IO;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ModuleInitializerAttribute : Attribute { }
}

namespace CSharp_9_Features.ModuleInitializers
{
    public class Program
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void M()
        {
            File.WriteAllText(DateTime.Now + " ModuleInitializers!", "ModuleInitializers.txt");
        }
    }
}

#endif
