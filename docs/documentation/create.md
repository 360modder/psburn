# Create

create command is used to generate native c# code to bind it with powershell script.

| Arguments/Options  | Definations                                                                                           |
|--------------------|-------------------------------------------------------------------------------------------------------|
| psscript           | It is required argument which should stores path of reference powershell script.                      |
| -e, --experimental | This option removes double qoutes by single qoutes to work with *--no-extract* feature.               |
| --execution-policy | Stores execution policy for the powershell script.                                                    |
| --script-name      | Change the file name at assembly.                                                                     |
| --script-version   | Change the file version at assembly.                                                                  |
| --blockcat         | User would not be able to extract powershell script if this option is enabled.                        |
| --self-contained   | Tells script that when I compile this program I will give you pwsh.zip which has powershell binaries. |
| --onedir           | Tells script that when I compile this program I want it to be in one directorary instead of one file. |
| --embed-resources  | Tells script that when I compile this program I will give you a resources zipfile.                    |
| --no-extract       | Enables script to be runned from powershell command mode.                                             |
| -o, --output       | Path to save .cs file.                                                                                |
| -v, --verbose      | Enables verbose logs and outputs.                                                                     |

## Shell

```shell
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
