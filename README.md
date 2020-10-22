(DEVELOP) C# Compiler Settings For Unity
===

**NOTE: This branch is for development purposes only.**  
**To use a released package, see [Releases page](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/releases) or [default branch](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity).**


## How to develop this package

1. Fork the repository and create your branch from `develop`.
2. Open the project and enable `Csc Settings > Develop Mode`  
![](https://user-images.githubusercontent.com/12690315/95061815-10b7aa80-0737-11eb-9e3f-1b33c4f0ddbc.png)
3. Develop the package
  * CSharpCompilerSettings:
    * `Assets/CSharpCompilerSettings`
    * This assembly is included in the package.
    * Publish as dll automatically to `Packages/CSharpCompilerSettings/Plugins`
  * CSharpCompilerSettings.Dev:
    * `Assets/CSharpCompilerSettings/Dev`
    * This assembly is **not** included in the package.
  * CSharpCompilerSettings.Editor:
    * `Packages/CSharpCompilerSettings/Editor`
    * This assembly is included in the package.
4. Click `Window > Generals > Test Runner` to test
5. Commit with [Angular Commit Message Conventions](https://gist.github.com/stephenparish/9941e89d80e2bc58a153)
6. Create pull request

For details, see [CONTRIBUTING](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/blob/upm/CONTRIBUTING.md) and [CODE_OF_CONDUCT](https://github.com/mob-sakai/CSharpCompilerSettingsForUnity/blob/upm/CODE_OF_CONDUCT.md).


## How to release this package

When you push to `preview`, `master` or `v1.x` branch, this package is automatically released by GitHub Action.

* Update version in `package.json` 
* Update CHANGELOG.md
* Commit documents and push
* Update and tag upm branch
* Release on GitHub
* ~~Publish npm registory~~

Alternatively, you can release it manually with the following command:

```bash
$ node run release -- --no-ci
```
