C# Compiler Settings For Unity
===

Change the C# compiler (csc) used on your Unity project, as you like!

[![](https://img.shields.io/npm/v/com.coffee.csharp-compiler-settings?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coffee.csharp-compiler-settings/)
[![](https://img.shields.io/github/v/release/mob-sakai/CSharpCompilerSettingsForUnity?include_prereleases)](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/CSharpCompilerSettingsForUnity.svg)](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/releases)
![](https://img.shields.io/badge/unity-2018.3%20or%20later-green.svg)
[![](https://img.shields.io/github/license/mob-sakai/CSharpCompilerSettingsForUnity.svg)](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/blob/upm/LICENSE.txt)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)

<< [Description](#description) | [Installation](#installation) | [Usage](#usage) | [Contributing](#contributing) >>

<br><br><br><br>

## Description

A good news! [Unity 2020.2.0a12 or later now supports C# 8.0!](https://forum.unity.com/threads/unity-c-8-support.663757/page-3#post-5811175)

Many developers (including you!) have been eagerly awaiting support for C# 8.0.  
C# 8.0 includes [some features and some useful syntax-sugars](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8):

* Readonly members
* Default interface methods
* Pattern matching enhancements:
  * Switch expressions
  * Property patterns
  * Tuple patterns
  * Positional patterns
* Using declarations
* Static local functions
* Disposable ref structs
* Nullable reference types
* Asynchronous streams
* Asynchronous disposable
* Indices and ranges
* Null-coalescing assignment
* Unmanaged constructed types
* Stackalloc in nested expressions
* Enhancement of interpolated verbatim strings

However, unfortunately, [there are no plans to backport to Unity 2020.1 or earlier...](https://forum.unity.com/threads/unity-c-8-support.663757/page-5#post-6269856)

<br>

This package changes the C# compiler (csc) used on your Unity project, to support C# 8.0.  
Let's enjoy C# 8.0 features with your Unity project!

![](https://user-images.githubusercontent.com/12690315/95178488-7456dc00-07fa-11eb-8489-63d6af311ed0.png)
![](https://user-images.githubusercontent.com/12690315/95178483-728d1880-07fa-11eb-89e6-c29d98e2ab02.png)

### Features

* Easy to use.
  * This package is out of the box!
* Change the C# compiler (csc) used on your Unity project.
  * Change the nuget package name.
    * **[Microsoft.Net.Compilers][]: Official compiler (default, run on Unity built-in mono)**
    * [Microsoft.Net.Compilers.Toolset][]: Official compiler (run on dotnet)
      * Resolve the [issue #2](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/2)
    * [OpenSesame.Net.Compilers][]: Allows access to internals/privates in other assemblies (run on Unity built-in mono)
    * [OpenSesame.Net.Compilers.Toolset][]: Allows access to internals/privates in other assemblies (run on dotnet)
      * Resolve the [issue #2](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/2)
    * Or, your custom nuget package
  * Change the nuget package version.
    * 3.4.0: C# 8.0 Supported.
    * **3.5.0: C# 8.0 Supported. (default in Unity 2020.2.0)**
    * 3.6.0: C# 8.0 Supported.
    * 3.7.0: C# 8.0 Supported.
    * 3.8.0 (preview): C# 9.0 Supported.
    * For other versions, see the nuget package page above.
  * Change the C# language version.
    * 7.0
    * 7.1
    * 7.2
    * 7.3
    * **8.0 (latest, default in Unity 2020.2.0)**
    * 9.0 (preview)
* Add the scripting define symbols based on language version on compiling.
  * e.g. `CSHARP_7_3_OR_LATER`, `CSHARP_8_OR_LATER`, `CSHARP_9_OR_LATER`
* Change the C# compiler settings for each `*.asmdef` file.
  * Portability: The assembly works even in the projects that do not have this package installed.
    * The best option when distributing as a package.
  * Publish as Dll: Published dll works without this package.
  * Modify the scripting define symbols for each `*.asmdef` file.
    * Add/Remove specific symbols separated with semicolons (`';'`) or commas (`','`).
    * The symbols starting with `'!'` will be removed.
    * e.g. `SYMBOL_TO_ADD;!SYMBOL_TO_REMOVE;...`
* Modify `langversion` property in *.csproj file.
* If `dotnet` is required, install it automatically.
* `CompilerType.BuiltIn` compiler option to disable this plugin.
* `Enable Logging` option to display compilation log.

[OpenSesame.Net.Compilers]: https://www.nuget.org/packages/OpenSesame.Net.Compilers
[OpenSesame.Net.Compilers.Toolset]: https://www.nuget.org/packages/OpenSesame.Net.Compilers.Toolset
[Microsoft.Net.Compilers]: https://www.nuget.org/packages/Microsoft.Net.Compilers
[Microsoft.Net.Compilers.Toolset]: https://www.nuget.org/packages/Microsoft.Net.Compilers.Toolset

### Feature plans

* Add a dropdown menu to select version.
* Verify the selected pakcage name and version.
* Show package description.

### NOTE: Please do so at your own risk!

https://forum.unity.com/threads/unity-c-8-support.663757/page-2#post-5738029

> Unity doesn't support using your own C# compiler.
> While this is possible, it is not something I would recommend, as we've not tested customer C# compiler versions with Unity.

<br><br><br><br>

## Installation

### Requirement

* Unity 2018.3 or later

### via OpenUPM

This package is available on [OpenUPM](https://openupm.com).  
After installing [openupm-cli](https://github.com/openupm/openupm-cli), run the following command
```
openupm add com.coffee.csharp-compiler-settings
```

### via Package Manager

Find the `manifest.json` file in the `Packages` directory in your project and edit it to look like this:
```
{
  "dependencies": {
    "com.coffee.csharp-compiler-settings": "https://github.com/mob-sakai/CSharpCompilerSettingsForUnity.git",
    ...
  },
}
```
To update the package, change suffix `#{version}` to the target version.

* e.g. `"com.coffee.csharp-compiler-settings": "https://github.com/mob-sakai/CSharpCompilerSettingsForUnity.git#1.0.0",`

Or, use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension) to install and update the package.

<br><br><br><br>

## Usage

### Configure C# compiler settings for the project

1. Open project setting window. (`Edit > Project Settings`)  
![](https://user-images.githubusercontent.com/12690315/92742741-e3d3da00-f3ba-11ea-8314-4cabd88c1b2c.png)
2. Select `C# Compiler` tab
3. Set `Compiler Type` to `Custom Package`, to use custom compiler package.  
![](https://user-images.githubusercontent.com/12690315/95178488-7456dc00-07fa-11eb-8489-63d6af311ed0.png)
3. Input `Package Name`, `Package Version`, `Language Version` for compilation.
   * See [features](#features) section.
4. Press `Apply` button to save settings.
5. It will automatically request a recompilation.  
The selected nuget package will be used for compilation.
6. Enjoy!

The project setting asset for C# Compiler will be saved in `ProjectSettings/CSharpCompilerSettings.asset`.

```json
{
    "m_UseDefaultCompiler": false,
    "m_Version": 110,
    "m_CompilerType": 1,
    "m_PackageName": "Microsoft.Net.Compilers.Toolset",
    "m_PackageVersion": "3.8.0-3.final",
    "m_LanguageVersion": 2147483646,
    "m_EnableLogging": true,
    "m_ModifySymbols": ""
}
```

<br><br>

### Configure C# compiler settings for `*.asmodef` file

1. Select a `*.asmodef` file
2. Turn on `Enable C# Compilier Settings` to configure.  
![](https://user-images.githubusercontent.com/12690315/95178483-728d1880-07fa-11eb-89e6-c29d98e2ab02.png)
3. Set `Compiler Type` to `Custom Package`, to use custom compiler package.
4. Input `Package Name`, `Package Version`, `Language Version` and `Modify Symbols` for compilation.
   * See [features](#features) section.
5. Press `Apply` button to save settings.

**Reload:** Reload C# compiler settings dll for the assembly.  
**Publish as dll:** Publish the assembly as dll to the parent directory.

<br><br>

### For C# 8.0 features

[C# 8.0 features](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8)  
[samples](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/tree/develop/Assets/C%23%208.0%20Features)

If you want to use the C# 8.0 features, set it up as follows:

* Package Name: **Microsoft.Net.Compilers**
* Package Version: **3.5.0** or later
* Language Version: **latest** or `CSharp_8`

Some features required external library.

* Async stream -> [UniTask v2](https://github.com/Cysharp/UniTask)
  * Install to project.
* Indices and ranges -> [IndexRange](https://www.nuget.org/packages/IndexRange/)
  * Download nuget package and extract it.
  * Import `lib/netstandard2.0/IndexRange.dll` to project.
* Stackalloc in nested expressions -> [System.Memory](https://www.nuget.org/packages/System.Memory/)
  * Download nuget package and extract it.
  * Import `lib/netstandard2.0/System.Memory.dll` to project.

**NOTE:** Default interface methods feature is not available. It requires `.Net Standard 2.1`.

<br><br>

### For C# 9.0 features (preview)

[C# 9.0 features](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9)  
[samples](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/tree/develop/Assets/C%23%209.0%20Features)

If you want to use the C# 9.0 features, set it up as follows:

* Package Name: **Microsoft.Net.Compilers**
* Package Version: **3.8.0-2.final** or later
* Language Version: **preview**

**NOTE:** Some features is not available. It requires `.Net 5`.

<br><br><br><br>

## Contributing

### Issues

Issues are very valuable to this project.

- Ideas are a valuable source of contributions others can make
- Problems show where this project is lacking
- With a question you show where contributors can improve the user experience

### Pull Requests

Pull requests are, a great way to get your ideas into this repository.  
See [CONTRIBUTING.md](/../../blob/develop/CONTRIBUTING.md) and [develop](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/tree/develop) branch.

### Support

This is an open source project that I am developing in my spare time.  
If you like it, please support me.  
With your support, I can spend more time on development. :)

[![](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/mob_sakai?)  
[![](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)

<br><br><br><br>

## License

* MIT

## Author

[mob-sakai](https://github.com/mob-sakai)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai) 

## See Also

* GitHub page : https://github.com/mob-sakai/CSharpCompilerSettingsForUnity
* Releases : https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/releases
* Issue tracker : https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues
* Change log : https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/blob/upm/CHANGELOG.md
