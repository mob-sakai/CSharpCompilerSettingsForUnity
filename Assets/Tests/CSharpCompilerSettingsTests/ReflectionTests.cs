using System.IO;
using Coffee.CSharpCompilerSettings;
using NUnit.Framework;


public class ReflectionTests
{
    public class ReflectionTestClass
    {
        private static string _staticPrivateText = "_staticPrivateText";

        private string _privateText = "";

        public static string StaticText
        {
            get { return _staticPrivateText; }
            set { _staticPrivateText = value; }
        }

        public string Text
        {
            get { return _privateText; }
        }

        private string PropertyText
        {
            get { return _privateText; }
            set { _privateText = value; }
        }

        private string GetText()
        {
            return _privateText;
        }

        private string GetText<T>(T obj)
        {
            return _privateText + "_" + typeof(T).Name + "_" + obj;
        }

        private static string GetStaticText(bool _)
        {
            return _staticPrivateText;
        }

        public ReflectionTestClass(string text)
        {
            _privateText = text;
        }
    }

    [Test]
    public void New()
    {
        var instance = typeof(ReflectionTestClass).New("private") as ReflectionTestClass;
        var expected = "private";
        var actual = instance.Text;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void InstanceGetMember()
    {
        var instance = new ReflectionTestClass("private");
        var expected = "private";
        var actual = instance.Get("_privateText");
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void InstanceGetProperty()
    {
        var instance = new ReflectionTestClass("private");
        var expected = "private";
        var actual = instance.Get("PropertyText");
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void InstanceSetProperty()
    {
        var instance = new ReflectionTestClass("private");
        instance.Set("PropertyText", "changed");
        var expected = "changed";
        var actual = instance.Text;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void InstanceSetMember()
    {
        var instance = new ReflectionTestClass("private");
        var expected = "changed";
        instance.Set("_privateText", "changed");
        var actual = instance.Get("_privateText");
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void InstanceCallMember()
    {
        var instance = new ReflectionTestClass("private");
        var expected = "private";
        var actual = instance.Call("GetText");
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void InstanceCallGenericMember()
    {
        var instance = new ReflectionTestClass("private");
        var expected = "private_Boolean_True";
        var actual = instance.Call(new[] {typeof(bool)}, "GetText", true);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void StaticGetMember()
    {
        ReflectionTestClass.StaticText = "StaticGetMember";
        var expected = "StaticGetMember";
        var actual = typeof(ReflectionTestClass).Get("_staticPrivateText");
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void StaticSetMember()
    {
        var expected = "StaticSetMember";
        typeof(ReflectionTestClass).Set("_staticPrivateText", "StaticSetMember");
        var actual = ReflectionTestClass.StaticText;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void StaticCallMember()
    {
        ReflectionTestClass.StaticText = "StaticCallMember";
        var expected = "StaticCallMember";
        var actual = typeof(ReflectionTestClass).Call("GetStaticText", true);
        Assert.AreEqual(expected, actual);
    }
}
