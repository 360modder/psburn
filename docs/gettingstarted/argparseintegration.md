# Argparse Integration

psburn can easily integrate argparse to your powershell script. python's argparse is fully matured module to do this stuff but in c# its scratch implemented which is similar to python argparse.

## Syntax

Integrating argparse is very simple, you just have to write one line json descriptors in your powershell script. [Learn More](/psburn/documentation/argparse/)

```ps1
#@parser {description: "gretting program"}
#@param {variable: "name", alias: "n", required: "true", help: "name to greet"}
#@param {variable: "twice", alias: "t", value: "false", type: "bool", required: "false", help: "greet person two times"}

Write-Output "$name you may have a good day"

if ($twice) {
	Write-Output "$name you may have a good day second time"
}
```

## Results

Now if you package script and run the compiled executable with **-h** flag, you will see this help message.

```bash
greet -h
```

=== "C#"

	```
	usage: greet [-h] [-t] [--cat] name

	gretting program

	positional arguments:
	name                         name to greet

	optional arguments:
	-t, --twice                  greet person two times [default: false]
	--cat                        instead of running cat powershell script into console [default: false]
	-h, --help                   show this help message and exit
	```

=== "Python"

	```
	usage: greet [-h] [-t] [--cat] name

	gretting program

	positional arguments:
	name         name to greet

	optional arguments:
	-h, --help   show this help message and exit
	-t, --twice  greet person two times [default: false]
	--cat        instead of running cat powershell script into console [default: false]
	```
