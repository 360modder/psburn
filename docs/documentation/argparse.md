# Argparse

psburn has a feature called argparse integration which allows to integrate argparse to powershell scripts. Also check this [example](/psburn/gettingstarted/argparseintegration/).

!!! note
	All #@ descriptors should be in one line.

## #@parser

Base descriptor for program.

### Syntax

```ps1
#@parser {description: "psburn$newlineparser-cli demo", usage: "usage: test.exe [<commands>]", examples: ["$file_base_name.exe --name $double_qoutes360modder$double_qoutes", "$file_base_name.exe --log"]}
```

### Properties

| Key         | Type       | Variables                       |
|-------------|------------|---------------------------------|
| description | string     | $newline                        |
| usage       | string     |                                 |
| examples    | list/array | $file_base_name, $double_qoutes |

!!! note
	**Variables** can be used inside strings which will be parsed later on.
	Python builds doesn't support Variables.

## #@param

Arguments and parameter descriptor for program.

### Syntax

```ps1
#@param {variable: "verbose", alias: "v", value: "false", type: "bool", required: "false", help: "enable or disable logging"}
```

### Properties

| Key      | Type           | Values                |
|----------|----------------|-----------------------|
| variable | string         |                       |
| alias    | char or string |                       |
| value    | string         |                       |
| type     | string         | string, float or bool |
| required | string         | true or false         |
| help     | string         |                       |

!!! note
	If **Values** cell is blank it means any value can be assigned.

!!! warning
	hyphens are not supported in **variable** instead use underscore.
