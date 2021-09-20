# PsBurn - Powershell Script Packager

<p align="center">
  <img src="docs/images/favicon.png" width="200"  height="200"/>
</p>

<p align="center">
  <a href="https://github.com/360modder/psburn/releases">
    <img src="https://img.shields.io/github/downloads/360modder/psburn/total.svg" alt="Total Downloads"/>
  </a>
  <a href="https://github.com/360modder/psburn/releases">
    <img src="https://img.shields.io/github/release/360modder/psburn.svg" alt="Latest Version"/>
  </a>
</p>

psburn is a tool to package powershell scripts into executables by binding it with a c# or python program.

Vist psburn's [website](https://360modder.github.io/psburn/) for more details.

## Installations

First setup a [building envoirnment](https://360modder.github.io/psburn/gettingstarted/installations/#building-envoirnments) then you can download and install a psburn from binary archive  for any of the following platforms.

You can download the psburn binary archives for Windows, Linux and MacOS.

| Supported Platform | Download                                                                                             | How to Install                  |
|--------------------|------------------------------------------------------------------------------------------------------|---------------------------------|
| Windows            | [64-bit](https://github.com/360modder/psburn/releases/download/v1.1.3/psburn.1.1.3.win-x64.zip)      | [Instructions][binary-archives] |
| Linux              | [64-bit](https://github.com/360modder/psburn/releases/download/v1.1.3/psburn.1.1.3.linux-x64.tar.gz) | [Instructions][binary-archives] |
| MacOS              | [64-bit](https://github.com/360modder/psburn/releases/download/v1.1.3/psburn.1.1.3.osx-x64.tar.gz)   | [Instructions][binary-archives] |

To install a specific version, visit [releases](https://github.com/360modder/psburn/releases).

[binary-archives]: https://360modder.github.io/psburn/gettingstarted/installations/#binary-archives
[packages]: https://360modder.github.io/psburn/gettingstarted/installations/#packages

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

Your powershell script will be packaged under dist folder of working directory. [Learn More](https://360modder.github.io/psburn/gettingstarted/packagepsscripts/)

## Quick Links

- [Package Powershell Scripts](https://360modder.github.io/psburn/gettingstarted/packagepsscripts/)
- [Argparse Integration](https://360modder.github.io/psburn/gettingstarted/argparseintegration/)
- [Creating Self Contained Executable](https://360modder.github.io/psburn/usage/creatingselfcontainedexecutable/)
- [CLI API Documentation](https://360modder.github.io/psburn/documentation/create/)

## Building From Source

You can build psburn release binaries from source by following the given instructions.

Requires*

- [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet/3.1)
- [python3.6+](https://www.python.org/downloads/)

Clone the psburn github repository.

```bash
git clone https://github.com/360modder/psburn.git
```

Now execute release.sh from psburn/release

```bash
cd psburn/release && chmod +x release.sh && ./release.sh
```
