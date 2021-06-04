# Build

build is a toolchain for building executables with c# and powershell scripts.

| Arguments/Options    | Definitions                                                           |
|----------------------|-----------------------------------------------------------------------|
| csfile               | Stores path of generated c# file.                                     |
| psscript             | Stores path of powershell script.                                     |
| -d, --debug          | Save some runtime generated files.                                    |
| -p, --powershell-zip | Embeds pwsh.zip to program to make it a self contained app.           |
| -r, --resources-zip  | Embeds a resource zipfile with program.                               |
| --cscpath            | Stores path for c# compiler.                                          |
| --icon               | Stores path to applicable icon.                                       |
| --no-console         | Generate a executable with no console.                                |
| --uac-admin          | Generates executable which request admin rights from user.            |
| --onedir             | When a executable is self contained run pwsh.exe from pwsh/pwsh.exe . |
| -o, --output         | Path to save .exe file.                                               |
| -v, --verbose        | Enables verbose logs and outputs.                                     |
| -?, -h, --help       | Show help and usage information.                                      |

## Shell

```shell
psburn build -h
```

```
build
  build an executable from c# program

Usage:
  psburn [options] build <csfile> <psscript>

Arguments:
  <csfile>    path of c# file
  <psscript>  path of powershell script

Options:
  -d, --debug                            don't delete runtime generated files [default: False]
  -p, --powershell-zip <powershell-zip>  create a self contained executable [default: no zip]
  -r, --resources-zip <resources-zip>    embed resources from a zip file [default: no zip]
  --cscpath <cscpath>                    c# compiler path (C:\Windows\Microsoft.Net\Framework\<version>\csc.exe)
                                         [default: auto detect]
  --icon <icon>                          apply icon to generated executable [default: no icon]
  --no-console                           create executable without a console, this helps for running scripts in
                                         background for gui programs [default: False]
  --uac-admin                            request elevation upon application restart (windows specific) [default: False]
  --onedir                               run powershell from current directory [default: False]
  -o, --output <output>                  path of output files [default: working directory]
  -v, --verbose                          generate more outputs and logs than usual [default: False]
  -?, -h, --help                         Show help and usage information
```
