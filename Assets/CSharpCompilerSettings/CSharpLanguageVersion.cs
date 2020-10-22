namespace Coffee.CSharpCompilerSettings
{
    internal enum CSharpLanguageVersion
    {
        CSharp7 = 700,
        CSharp7_1 = 701,
        CSharp7_2 = 702,
        CSharp7_3 = 703,
        CSharp8 = 800,
        CSharp9 = 900,
        Preview = int.MaxValue - 1,
        Latest = int.MaxValue,
    }
}
