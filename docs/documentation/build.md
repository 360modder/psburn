# Build

build command provides a set of toolchains for building executables with c# and  python with powershell scripts.

## Shell

```bash
psburn build -h
```

```
build
  build an executable from binder file

Usage:
  psburn [options] build <psscript> <binderfile>

Arguments:
  <psscript>    path of powershell script
  <binderfile>  path of binder file

Options:
  -p, --powershell-zip <powershell-zip>  create a powershell core self contained executable [default: no zip]
  -r, --resources-zip <resources-zip>    embed resources from a zip file [default: no zip]
  --onedir                               run powershell executable from root directory for self contained builds
                                         [default: False]
  --icon <icon>                          icon path for executable [default: no icon]
  --no-console                           create executable without a console (gui) [default: False]
  --uac-admin                            request elevation upon application restart (windows specific) [default: False]
  --cscpath <cscpath>                    csc/c# compiler path (C:\Windows\Microsoft.Net\Framework\<version>\csc.exe)
                                         [default: auto detect]
  --merge                                merge dll and exe using ILMerge [default: False]
  --pyinstaller-prompt                   ask for extra pyinstaller arguments [default: False]
  -d, --debug                            don't delete runtime generated files [default: False]
  --verbose                              generate more outputs and logs than usual [default: False]
  -?, -h, --help                         Show help and usage information
```
