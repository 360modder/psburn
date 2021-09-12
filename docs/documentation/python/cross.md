# Cross

create command is used to generate python code to bind it with powershell script.

| Arguments/Options  | Definitions                                                             |
|--------------------|-------------------------------------------------------------------------|
| psscript           | Stores path of powershell script.                                       |
| --execution-policy | Stores execution policy for the powershell script.                      |
| --blockcat         | User would not be able to extract/cat powershell script by using --cat. |
| --onedir           | When a executable is self contained run pwsh.exe from pwsh/pwsh.exe .   |
| -o, --output       | Path to save .py file.                                                  |
| -v, --verbose      | Enables verbose logs and outputs.                                       |
| -?, -h, --help     | Show help and usage information.                                        |

## Shell

```bash
psburn cross -h
```

```
cross
  create a psburn py script

Usage:
  psburn [options] cross <psscript>

Arguments:
  <psscript>  path of powershell script

Options:
  --execution-policy <execution-policy>  execution policy for the powershell script [default: Bypass]
  --blockcat                             block cating of script which prevents script exposure [default: False]
  --onedir                               run powershell from current directory [default: False]
  -o, --output <output>                  path of output files [default: working directory]
  -v, --verbose                          generate more outputs and logs than usual [default: False]
  -?, -h, --help                         Show help and usage information
```