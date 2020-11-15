namespace Coffee.CSharpCompilerSettings
{
    internal readonly struct CompilerFilter
    {
        public readonly CompilerRuntime Runtime;
        public readonly string RelatedPath;

        public CompilerFilter(CompilerRuntime runtime, string relatedPath)
        {
            Runtime = runtime;
            RelatedPath = relatedPath;
        }
    }
}
