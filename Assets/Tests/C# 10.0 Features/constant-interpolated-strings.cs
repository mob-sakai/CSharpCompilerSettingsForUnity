/*
# Constant Interpolated Strings
　 https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/constant_interpolated_strings

Enables constants to be generated from interpolated strings of type string constant.
*/
#if CUSTOM_COMPILE
namespace CSharp_10_Features
{
    public class Cs10_ConstantInterpolatedStrings
    {
        const string S1 = $"Hello world";
        const string S2 = $"Hello{" "}World";
        const string S3 = $"{S1} Kevin, welcome to the team!";

        [Test]
        public void Test()
        {
            Assert.AreEqual(S3, "Hello world Kevin, welcome to the team!");
        }
    }
}
#endif