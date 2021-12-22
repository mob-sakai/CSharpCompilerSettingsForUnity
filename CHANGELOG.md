# [1.5.0](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/1.4.0...1.5.0) (2021-12-22)


### Bug Fixes

* In Unity 2020.2 or later, some fields will be not deserialized on first compilation (analyzer) ([0a8a7fb](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0a8a7fbe3e66565a1510d06474b6bbeffaf579b1))
* when using VS Code, C# project file is not modified in Unity 2021 or later ([dc97d57](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/dc97d57f17c07dfe1a1bf44f3073de2a10244ba8))


### Features

* add 'CUSTOM_COMPILE' symbol for custom compiler ([745ecd9](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/745ecd99b3081985ebf28230df4baf79271f48ae))
* add custom compiler interface ([2e49e4c](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/2e49e4c1208fac8efa2a72069b78aee0966e3ee8))
* support C# 10 (for *.Net.Compilers.Toolset v4.0.0 or later) ([fb5ac33](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/fb5ac33876681f5900e2118b014893ebff43b75c))
* support Unity 2021.1 or later ([40f4d57](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/40f4d570a3b14220cb2fb77a7897a1e9024d97e0))

# [1.4.0](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/1.3.0...1.4.0) (2021-01-06)


### Bug Fixes

* in 2020.2 or later, no need to recompile ([f815cf2](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/f815cf2c7c270afca6bca019d0fc6eb42b4c4348))
* in Unity 2020.2 or later, some fields in CscSettingsAsset will be not deserialized on first compilation ([1858247](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/18582475232a2d9dbc455dab89c5b7b374f7d110))
* nullable is supported in C[#8](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/8).0 or later ([f16985b](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/f16985b7aaf6017740a9e5449d23402894616059))
* when enabling C# Settings in asmdef inspector, edits are lost ([0a988fa](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0a988faa1dede0bca577caf0f96c179e4d5eedb7))


### Features

* add assembly filter to compile with custom compiler ([acfa0e6](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/acfa0e6117ba3d3e74a15fedea43236d3daf81e3))
* in Unity 2020.2 or later, initialize CSharpCompilerSettings for first compilation ([3924f1a](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/3924f1a1838bed92606ee1b7720e1bf01ec7e530))
* reload all CSharpCompilerSettings_* assembly from project settings window ([9a4b7c5](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/9a4b7c58505fa4f68d6dd32f8c9ec1009c453c5a))
* support .editorconfig for analyzer ([0e1d4c3](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0e1d4c36555e4a004ba11583a421fd2acc82b118))
* support nuget package drawer ([34d1a28](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/34d1a28538c1615248b09b3c4dd7657f2bd77b20))
* support roslyn analyzer ([0b06968](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0b069685563c79f541bf570bb7aff3411f7b851a)), closes [#9](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/9)

# [1.4.0-preview.1](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/1.3.0...1.4.0-preview.1) (2020-12-23)


### Bug Fixes

* in 2020.2 or later, no need to recompile ([f815cf2](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/f815cf2c7c270afca6bca019d0fc6eb42b4c4348))
* in Unity 2020.2 or later, some fields in CscSettingsAsset will be not deserialized on first compilation ([1858247](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/18582475232a2d9dbc455dab89c5b7b374f7d110))
* nullable is supported in C[#8](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/8).0 or later ([f16985b](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/f16985b7aaf6017740a9e5449d23402894616059))
* when enabling C# Settings in asmdef inspector, edits are lost ([0a988fa](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0a988faa1dede0bca577caf0f96c179e4d5eedb7))


### Features

* add assembly filter to compile with custom compiler ([acfa0e6](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/acfa0e6117ba3d3e74a15fedea43236d3daf81e3))
* in Unity 2020.2 or later, initialize CSharpCompilerSettings for first compilation ([3924f1a](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/3924f1a1838bed92606ee1b7720e1bf01ec7e530))
* reload all CSharpCompilerSettings_* assembly from project settings window ([9a4b7c5](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/9a4b7c58505fa4f68d6dd32f8c9ec1009c453c5a))
* support .editorconfig for analyzer ([0e1d4c3](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0e1d4c36555e4a004ba11583a421fd2acc82b118))
* support nuget package drawer ([34d1a28](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/34d1a28538c1615248b09b3c4dd7657f2bd77b20))
* support roslyn analyzer ([0b06968](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0b069685563c79f541bf570bb7aff3411f7b851a)), closes [#9](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/9)

# [1.3.0](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/1.2.0...1.3.0) (2020-11-09)


### Bug Fixes

* support Unity 2020.2 or later ([181ea58](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/181ea586feda9e3a0b8cdc86578ae95c19807123)), closes [#6](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/6) [#7](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/7)
* when enabling C# Settings in asmdef inspector, edits are lost ([0d86d20](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0d86d20032e573771168d25dfc3027162027ce89))


### Features

* support all nullable settings ([789edaf](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/789edaf83d2ecab152b0efd5eb1a03c7fbfbacbd)), closes [#8](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/8)

# [1.2.0](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/1.1.1...1.2.0) (2020-10-23)


### Bug Fixes

* fix release workflow ([073f310](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/073f310220eadf3343b51f9849e1807330867e36))


### Features

* add nullable option ([9bc0b85](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/9bc0b858e27dfb6da561404336389064a35c2cf8)), closes [#5](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/5)

# [1.2.0](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/1.1.1...1.2.0) (2020-10-22)


### Features

* add nullable option ([9bc0b85](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/9bc0b858e27dfb6da561404336389064a35c2cf8)), closes [#5](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/issues/5)

## [1.1.1](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/compare/v1.1.0...v1.1.1) (2020-10-13)


### Bug Fixes

* change language version for 8 & 9 to 8.0 & 9.0 ([2ca4892](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/2ca48927cfcbacd4ac26e38148893334fc008171))
* change the external exe path ([0bc2b46](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/0bc2b46777fd758a569aa2c339c77a87fff5c955))
* log for compile command is incorrect ([b551b82](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/b551b82743e0b759a07f218c6d28ef9917dc2ed9))
* the setting is incorrect for asmdef ([6b16210](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/commit/6b1621042b55cbe6a82138b29b029b1647cc79f7))

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
