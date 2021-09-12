# CBuild

cbuild is a toolchain for building executables with python and powershell scripts.

| Arguments/Options    | Definitions                                                           |
|----------------------|-----------------------------------------------------------------------|
| pyscript             | Stores path of generated python file.                                 |
| psscript             | Stores path of powershell script.                                     |
| -d, --debug          | Save some runtime generated files.                                    |
| -p, --powershell-zip | Embeds pwsh.zip to program to make it a self contained app.           |
| -r, --resources-zip  | Embeds a resource zipfile with program.                               |
| --onedir             | When a executable is self contained run pwsh.exe from pwsh/pwsh.exe . |
| --no-prompt          | Hides up extra pyinstaller prompts.                                   |
| -o, --output         | Path to save executable file. (currently un-working)                  |
| -v, --verbose        | Enables verbose logs and outputs.                                     |
| -?, -h, --help       | Show help and usage information.                                      |

## Shell

```bash
psburn cbuild -h
```

```
cbuild
  build an executable from python script

Usage:
  psburn [options] cbuild <pyscript> <psscript>

Arguments:
  <pyscript>  path of python script
  <psscript>  path of powershell script

Options:
  -d, --debug                            don't delete runtime generated files [default: False]
  -p, --powershell-zip <powershell-zip>  create a self contained executable [default: no zip]
  -r, --resources-zip <resources-zip>    embed resources from a zip file [default: no zip]
  --onedir                               run powershell from current directory [default: False]
  --no-prompt                            don't ask for any prompts [default: False]
  -o, --output <output>                  path of output files [default: working directory]
  -v, --verbose                          generate more outputs and logs than usual [default: False]
  -?, -h, --help                         Show help and usage information
```
