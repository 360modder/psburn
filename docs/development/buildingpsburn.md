# Building Psburn

## Updating Assets

Before building up psburn, *assets* directory needs to be updated once. This can be done by running *update_assets.ps1*

```bash
powershell -ExecutionPolicy Bypass -File update_assets.ps1
```

Now you can build psburn by running.

```bash
dotnet build
```

## Test

To perform a test just go to *test* directory and run *run.bat*, and to clean files run *clean.bat*

## Release

You can build psburn release binaries from source by following the given instructions.

Requires*

- [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet/3.1)
- [python3.6+](https://www.python.org/downloads/)

Clone the psburn github repository.

```bash
git clone https://github.com/360modder/psburn.git
```

Now execute *release.sh* from *psburn/release*

```bash
cd psburn/release && chmod +x release.sh && ./release.sh
```

To build a setup file, first install [Inno Setup](https://jrsoftware.org/isdl.php). Then execute *build_installer.bat* from *psburn/release*

```bash
cd psburn/release && build_installer.bat
```
