C# Compiler Settings For Unity
===

Change the C# compiler (csc) used in your Unity project, as you like!

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

Change the C# compiler (csc) used in your Unity project, as you like!

![](https://user-images.githubusercontent.com/12690315/92738843-44611800-f3b7-11ea-9412-be528547d0dd.png)

### Features

* Change the C# compiler (csc) used in your Unity project.
  * Change the nuget package name.
    * **[Microsoft.Net.Compilers][]: Official compiler (default)**
    * [OpenSesame.Net.Compilers][]: Allows access to internals/privates in other assemblies.
  * Change the nuget package version.
    * 3.4.0: C# 8.0 Supported.
    * **3.5.0: C# 8.0 Supported. (default)**
    * 3.6.0: C# 8.0 Supported.
    * 3.7.0: C# 8.0 Supported.
    * 3.8.0 (preview): C# 9.0 Supported.
    * For other versions, see the nuget package page above.
  * Change the C# language version.
    * 7.0
    * 7.1
    * 7.2
    * 7.3
    * **8.0 (Latest, default)**
    * 9.0 (Preview)
* Easy to use.
  * This package is out of the box.
* `dotnet` is not required.
* "Use Default Compiler" option.

[OpenSesame.Net.Compilers]: https://www.nuget.org/packages/OpenSesame.Net.Compilers
[Microsoft.Net.Compilers]: https://www.nuget.org/packages/Microsoft.Net.Compilers

<br><br><br><br>

## Installation

#### Requirement

* Unity 2018.3 or later

#### via OpenUPM

This package is available on [OpenUPM](https://openupm.com).  
After installing [openupm-cli](https://github.com/openupm/openupm-cli), run the following command
```
openupm add com.coffee.csharp-compiler-settings
```

#### via Package Manager

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

1. Open project setting window. (`Edit > Project Settings`)  
![](https://user-images.githubusercontent.com/12690315/92742741-e3d3da00-f3ba-11ea-8314-4cabd88c1b2c.png)
1. Select `C# Compiler` tab and turn off `Use Default Compiler` option.  
![](https://user-images.githubusercontent.com/12690315/92738843-44611800-f3b7-11ea-9412-be528547d0dd.png)
1. It will automatically request a recompilation.  
The selected nuget package will be used for compilation.
3. Enjoy!


<br><br><br><br>

## Contributing

### Issues

Issues are very valuable to this project.

- Ideas are a valuable source of contributions others can make
- Problems show where this project is lacking
- With a question you show where contributors can improve the user experience

### Pull Requests

Pull requests are, a great way to get your ideas into this repository.  
See [CONTRIBUTING.md](/../../blob/develop/CONTRIBUTING.md).

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
