# PsBurn - Powershell Compiler

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

psburn is cross platform tool to compile dynamic powershell scripts into platform specific executables by encapsulating it inside a c# or python program.

# Installation

You can download and install a psburn package for any of the following platforms.

| Supported Platform | Download                                                                                        | How to Install             |
|--------------------|-------------------------------------------------------------------------------------------------|----------------------------|
| Windows (x64)      | [.exe](https://github.com/360modder/psburn/releases/download/v1.0.0/psburn.1.0.0.win-x64.exe)   | [Instructions][in-windows] |
| Debian             | [.deb](https://github.com/360modder/psburn/releases/download/v1.0.0/psburn.1.0.0.linux-x64.deb) | [Instructions][in-debian]  |
| Red Hat            | [.rpm](https://github.com/360modder/psburn/releases/download/v1.0.0/psburn.1.0.0.linux-x64.rpm) | [Instructions][in-rh]      |

You can also download the psburn binary archives for Windows, Linux and MacOS.

| Supported Platform | Download                                                                                                                                                                                        | How to Install             |
|--------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------|
| Windows            | [32-bit](https://github.com/360modder/psburn/releases/download/v1.0.0/psburn.1.0.0.win-x86.zip)/[64-bit](https://github.com/360modder/psburn/releases/download/v1.0.0/psburn.1.0.0.win-x64.zip) | [Instructions][in-windows] |
| Linux              | [64-bit](https://github.com/360modder/psburn/releases/download/v1.0.0/psburn.1.0.0.linux-x64.tar.gz)                                                                                            | [Instructions][in-linux]   |
| MacOS              | [64-bit](https://github.com/360modder/psburn/releases/download/v1.0.0/psburn.1.0.0.osx-x64.tar.gz)                                                                                              | [Instructions][in-macos]   |

To install a specific version, visit [releases](https://github.com/360modder/psburn/releases).

# Building From Source

You can build psburn release binaries from source by following the given instructions.

Requires*
- ubuntu-18.04
- python3.6+

Clone the psburn github repository.

```bash
git clone https://github.com/360modder/psburn.git
```

Now execute release.sh from psburn/release

```bash
cd psburn/release
chmod +x release.sh && ./release.sh
```
