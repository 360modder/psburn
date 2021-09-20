# Create

create command is used to generate c# and python binder file for powershell script.

## Shell

```bash
psburn create -h
```

```
create
  create a binder c# or python file

Usage:
  psburn [options] create <psscript>

Arguments:
  <psscript>  path of powershell script

Options:
  -o, --output <output>                  path for output of c# or python file [default: working directory]
  --py                                   generate python binder file instead of c# [default: False]
  --self-contained                       enable this option if you are using --powershell-zip [default: False]
  --embed-resources                      enable this option if you are using --resources-zip [default: False]
  --onedir                               run powershell executable from root directory for self contained builds
                                         [default: False]
  --execution-policy <execution-policy>  script execution policy [default: Bypass]
  --block-cat                            block script cat feature at runtime [default: False]
  --verbose                              generate more outputs and logs than usual [default: False]
  -?, -h, --help                         Show help and usage information
```
