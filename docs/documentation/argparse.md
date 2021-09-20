# Argparse

psburn has a feature called argparse integration which allows to integrate argparse to powershell scripts. Also check this [example](/psburn/gettingstarted/argparseintegration/).

!!! note
	All #@ descriptors should be in one line.

## #@parser

Base descriptor for program.

### Syntax

```ps1
#@parser {description: "psburn parser-cli demo"}
```

### Properties

| Key         | Type       |
|-------------|------------|
| description | string     |

## #@param

Arguments and parameter descriptor for program.

### Syntax

```ps1
#@param {variable: "verbose", alias: "v", value: "false", type: "bool", required: "false", help: "enable or disable logging"}
```

### Properties

| Key      | Type           | Values                 |
|----------|----------------|------------------------|
| variable | string         |                        |
| alias    | char or string |                        |
| value    | string         |                        |
| type     | string         | string, double or bool |
| required | string         | true or false          |
| help     | string         |                        |

!!! note
	If **Values** cell is blank it means any value can be assigned.

!!! warning
	hyphens are not supported in **variable** instead use underscore.
