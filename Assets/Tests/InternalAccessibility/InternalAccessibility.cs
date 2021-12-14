using NUnit.Framework;
using InternalAssembly;

public class InternalAccessibility
{
    [Test]
    public void Test()
    {
#if UNITY_2019_2_OR_NEWER
        InternalAssembly.PublicClass.InternalStaticMethod();
#endif
    }
}
