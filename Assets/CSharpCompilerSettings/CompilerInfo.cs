namespace Coffee.CSharpCompilerSettings
{
    internal readonly struct CompilerInfo
    {
        public readonly CompilerRuntime Runtime;
        public readonly string Path;

        public CompilerInfo(CompilerRuntime runtime, string path)
        {
            Runtime = runtime;
            Path = path;
        }
    }
}
