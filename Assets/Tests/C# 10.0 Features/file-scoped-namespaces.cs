/*
# File Scoped Namespaces
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/file-scoped-namespaces

Allow a simpler format for the common case of file containing only one namespace in it.
*/
#if CUSTOM_COMPILE
namespace CSharp_10_Features;

class Cs10_FileScopedNamespaces
{
    [Test]
    public void Test()
    {
        Assert.AreEqual(GetType().Namespace, nameof(CSharp_10_Features));
    }
}
#endif