## [1.1.1-preview.1](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/v1.1.0...v1.1.1-preview.1) (2020-10-13)


### Bug Fixes

* change language version for 8 & 9 to 8.0 & 9.0 ([2ca4892](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/2ca48927cfcbacd4ac26e38148893334fc008171))
* change the external exe path ([0bc2b46](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0bc2b46777fd758a569aa2c339c77a87fff5c955))

# [1.1.0](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/v1.0.1...v1.1.0) (2020-10-06)


### Bug Fixes

* add apply/revert button in project settings provider ([3f86ab6](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/3f86ab6eb4fe3a342def7413929e13175bc0b016))
* ignore development asmdef for inspector ([e49c85e](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/e49c85e36a08a5627ff340e8ee767014ec908704))
* modify csproj with each C# compiler settings ([602af0f](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/602af0fe8b157b26decc261d3eee1c07b06e28ce))
* namespace typo ([e57107b](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/e57107b5b2b37453d121f0fb49fde593b773a3ef))
* support csproj symbols ([0786ecd](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0786ecdd009f96131dda00efaeaabfced41087f8))
* typo ([9366513](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/9366513dd68c340ca870d66284d778cf62d829f0))


### Features

* add app to change assembly name ([ba38c8c](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/ba38c8ceb625d6b972ef5b5552c9ad716b12249e))
* add compiling options (deterministic and optimize) for CSharpCompilerSettings.dll ([701bb3b](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/701bb3b73b19eb115588e76fb70bc48f9124ff41))
* add inspector header for asmdef to configure C# compiler settings ([e3a7117](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/e3a711711df1e964a9f18a336b4a001a6afcb015))
* add setting option to enable debug log for compiling ([1911a82](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/1911a82a8a7559987d902d4017436f86b944f528))
* add support for Microsoft.Net.Compilers.Toolset and similar packages ([3de5ec9](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/3de5ec94b002764d42af1b57d135fdaf7a484156))
* configure the C# compiler for each *.asmdef file ([735e810](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/735e810bc99d7659f0c1f9ccab2a004f20795f55))
* modify the scripting define symbols on compiling ([74b1669](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/74b1669c4f6d425665e0ad72bea8bdfd65c9e139))
* publish assembly as dll ([79b47b4](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/79b47b4abcc8d7c6a33df7fad82d09391ada77cb))
* recompile on reload csc settings assemlby ([b4f79b0](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/b4f79b02e5b09638deefa4c9ef516751a782ad08))
* **dev:** add menu to recompile ([2fff7f9](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/2fff7f9b6deb97395d3899af7ae0f704cf742d1b))
* **Dev:** Automatically change asmdef and dll references for editor assembly ([9b2452a](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/9b2452a7626e62a024f6a3ecb4a52995d4dc0e3b))

## [1.0.1](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/v1.0.0...v1.0.1) (2020-09-17)


### Bug Fixes

* Not working on Windows ([b0d4eba](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/b0d4ebacf6d940b14aecf6d79ef7fdb4ebddcaa3)), closes [#1](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/1)

# 1.0.0 (2020-09-11)


### Features

* add scripting symbols for C# language ([a6dca8e](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/a6dca8efdfc45debc8878349619f85b18179cd22))
* change the C# compiler (csc) used in project ([8f10c0d](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/8f10c0d2735a3bcd7fbfb1e72fe8c55caee37e91))
* project settings for the C# compiler package name, version and C# language version ([c7aec4d](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/c7aec4d6d40c2a85bb53a643509487a6e10e0503))

# [1.0.0-preview.2](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/v1.0.0-preview.1...v1.0.0-preview.2) (2020-09-11)


### Reverts

* Revert "feat: support .Net 3.5" ([5f7cdec](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/5f7cdecfd743a89c09f36ea3946576d4d93d9d2d))

# 1.0.0-preview.1 (2020-09-11)


### Features

* add scripting symbols for C# language ([a6dca8e](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/a6dca8efdfc45debc8878349619f85b18179cd22))
* change the C# compiler (csc) used in project ([8f10c0d](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/8f10c0d2735a3bcd7fbfb1e72fe8c55caee37e91))
* project settings for the C# compiler package name, version and C# language version ([c7aec4d](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/c7aec4d6d40c2a85bb53a643509487a6e10e0503))
* support .Net 3.5 ([d2c423e](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/d2c423e4a04706dd0801291202bb98c27b85bec2))
