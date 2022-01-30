# PsBurn - Powershell Script Packager

<p align="center">
  <img src="docs/images/favicon.png" width="200"  height="200"/>
</p>

<p align="center">
  <a href="https://github.com/clitic/psburn/releases">
    <img src="https://img.shields.io/github/downloads/clitic/psburn/total.svg" alt="Total Downloads"/>
  </a>
  <a href="https://github.com/clitic/psburn/releases">
    <img src="https://img.shields.io/github/release/clitic/psburn.svg" alt="Latest Version"/>
  </a>
</p>

psburn is a tool to package powershell scripts into executables by binding it with a c# or python program.

Visit psburn's [website](https://clitic.github.io/psburn/) for more details.

## Installations

First setup a [building envoirnment](https://clitic.github.io/psburn/gettingstarted/installations/#building-envoirnments) then you can download and install psburn for any of the following platform.

You can download the psburn for Windows, Linux and MacOS.

| Supported Platform | Download                                                                                             | How to Install                  |
|--------------------|------------------------------------------------------------------------------------------------------|---------------------------------|
| Windows            | [64-bit](https://github.com/clitic/psburn/releases/download/v1.1.3/psburn.1.1.3.win-x64.exe)      |                                 |
| Linux              | [64-bit](https://github.com/clitic/psburn/releases/download/v1.1.3/psburn.1.1.3.linux-x64.tar.gz) | [Instructions][binary-archives] |
| MacOS              | [64-bit](https://github.com/clitic/psburn/releases/download/v1.1.3/psburn.1.1.3.osx-x64.tar.gz)   | [Instructions][binary-archives] |

To install a specific version, visit [releases](https://github.com/clitic/psburn/releases).

[binary-archives]: https://clitic.github.io/psburn/gettingstarted/installations/#binary-archives

## Usage

Following commands are in reference of powershell script named **script.ps1**

- Windows (backend c# --> c# 5 compiler)

```bash
psburn create script.ps1 -o script.cs
psburn build script.ps1 script.cs
```

- Linux/MacOS (backend python --> pyinstaller)

```bash
psburn create script.ps1 --py -o script.py
psburn build script.ps1 script.py
```

Your powershell script will be packaged under dist folder of working directory. [Learn More](https://clitic.github.io/psburn/gettingstarted/packagepsscripts/)

## Quick Links

- [Package Powershell Scripts](https://clitic.github.io/psburn/gettingstarted/packagepsscripts/)
- [Argparse Integration](https://clitic.github.io/psburn/gettingstarted/argparseintegration/)
- [Creating Self Contained Executable](https://clitic.github.io/psburn/usage/creatingselfcontainedexecutable/)
- [CLI API Documentation](https://clitic.github.io/psburn/documentation/create/)
- [Changelog](https://clitic.github.io/psburn/changelog/)

## How does this works ?

- [C# Builds](https://github.com/clitic/psburn/tree/master/psburn_cli_parser/csharp_binder) 
- [Python Builds](https://github.com/clitic/psburn/tree/master/psburn_cli_parser/python_binder)

## Building From Source

You can build psburn release binaries from source by following the given instructions.

Requires*

- [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet/3.1)
- [Inno Setup](https://jrsoftware.org/isdl.php)
- [Git Bash](https://git-scm.com/download)

Clone the psburn github repository.

```bash
git clone https://github.com/clitic/psburn.git
```

Now run `make help` to see available targets.

```bash
$ make help
```

```
make targets:
  test                  run a test for script compile task
  clean                 clean bin and test generated files
  update_assets         update assets file in assets directory
  release               package tarball distribution for linux-x64 and osx-x64
  setup                 create inno setup installer for win-x64
  help                  shows this help message
```
