using System;
using Mono.Cecil;

namespace ChangeAssemblyName
{
    class Program
    {
        private static void Main(string[] args)
        {
            var filepath = args[0];
            var name = args[1];

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(filepath, new ReaderParameters()
            {
                AssemblyResolver = new DefaultAssemblyResolver(),
                ReadWrite = true,
            });

            var oldName = assemblyDefinition.Name.Name;
            var mainModule = assemblyDefinition.MainModule;
            mainModule.Name = name;
            assemblyDefinition.Name.Name = name;
            Console.WriteLine($"change assembly name '{oldName}' to '{name}'");

            assemblyDefinition.Write();
        }
    }
}
