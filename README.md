# PsBurn - Powershell Compiler

<p align="center">
  <img src="docs/images/favicon.png" width="200"  height="200"/>
</p>

psburn is cross platform tool to compile dynamic powershell scripts into platform specific executables by encapsulating it inside a c# or python program.

# Installation

- Ubuntu 18.04/20.04

Add psburn's repository to your system.

```bash
curl -s https://packagecloud.io/install/repositories/360modder/psburn/script.deb.sh | sudo bash
sudo apt-get update
```

Install psburn from psburn's repository.

```bash
sudo apt-get install psburn
```

- For other platforms releasing soon

# Building from source

You can build psburn release binaries from source, follow the following instructions.

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
