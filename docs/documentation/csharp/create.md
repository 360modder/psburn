# Create

create command is used to generate c# code to bind it with powershell script.

| Arguments/Options  | Definitions                                                               |
|--------------------|---------------------------------------------------------------------------|
| psscript           | Stores path of powershell script.                                         |
| -e, --experimental | Removes double quotes by single quotes to work with --no-extract feature. |
| --execution-policy | Stores execution policy for the powershell script.                        |
| --script-name      | Changes the file name at assembly.                                        |
| --script-version   | Changes the file version at assembly.                                     |
| --blockcat         | User would not be able to extract/cat powershell script by using --cat.   |
| --self-contained   | Tells script that executable is gonna be a self contained app.            |
| --onedir           | When a executable is self contained run pwsh.exe from pwsh/pwsh.exe .     |
| --embed-resources  | Tells script that executable will have embedded resources.                |
| --no-extract       | Enables script to be runned from powershell command mode.                 |
| -o, --output       | Path to save .cs file.                                                    |
| -v, --verbose      | Enables verbose logs and outputs.                                         |
| -?, -h, --help     | Show help and usage information.                                          |

## Shell

```bash
psburn create -h
```

```
create
  create a psburn c# file

Usage:
  psburn [options] create <psscript>

Arguments:
  <psscript>  path of powershell script

Options:
  -e, --experimental                     use experimental features to create script [default: False]
  --execution-policy <execution-policy>  execution policy for the powershell script [default: Bypass]
  --script-name <script-name>            set name of script [default: basename of input file]
  --script-version <script-version>      set version of script [default: 1.0.0]
  --blockcat                             block cating of script which prevents script exposure [default: False]
  --self-contained                       enable this option if you are using --powershell-zip [default: False]
  --onedir                               run powershell from current directory [default: False]
  --embed-resources                      enable this option if you are using --resources-zip [default: False]
  --no-extract                           this option allows to host scripts purely from program instead of extracted
                                         version of them [default: False]
  -o, --output <output>                  path of output files [default: working directory]
  -v, --verbose                          generate more outputs and logs than usual [default: False]
  -?, -h, --help                         Show help and usage information
```
