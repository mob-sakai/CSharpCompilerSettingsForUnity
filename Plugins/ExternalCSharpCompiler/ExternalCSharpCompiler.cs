using System;
using System.IO;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Coffee.CSharpCompilerSettings
{
    internal class ExternalCSharpCompiler
    {
        [InitializeOnLoadMethod]
        static void InitializeOnLoad()
        {
            var coreAssemblyName = typeof(ExternalCSharpCompiler).Assembly.GetName().Name;
            if (coreAssemblyName == "ExternalCSharpCompiler_") return;

            Console.WriteLine("[ExternalCSharpCompiler] Initialize CSharpCompilerSettings before first compilation: " + typeof(ExternalCSharpCompiler));

            var filepath = "Temp/" + typeof(ExternalCSharpCompiler).Assembly.GetName().Name + ".loaded";
            if (File.Exists(filepath)) return;
            File.WriteAllText(filepath, "");

            var cscTypes = AssetDatabase.FindAssets("t:DefaultAsset CSharpCompilerSettings")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(x => x.EndsWith(".dll"))
                .Select(Assembly.LoadFrom)
                .SelectMany(x => x.GetTypes())
                .ToArray();

            var initializeOnLoadTypes = cscTypes
                .Where(x => 0 < x.GetCustomAttributes(typeof(InitializeOnLoadAttribute), false).Length);

            var initializeOnLoadMethods = cscTypes
                .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(x => 0 < x.GetCustomAttributes(typeof(InitializeOnLoadMethodAttribute), false).Length);

            foreach (var type in initializeOnLoadTypes)
            {
                try
                {
                    Console.WriteLine("  > InitializeOnLoadAttribute: " + type);
                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
            }

            foreach (var method in initializeOnLoadMethods)
            {
                try
                {
                    Console.WriteLine("  > InitializeOnLoadMethodAttribute: " + method);
                    method.Invoke(null, new object[0]);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
            }
        }
    }
}
