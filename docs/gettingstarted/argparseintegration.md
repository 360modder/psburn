# Argparse Integration

psburn can easily integrate argparse to your powershell script. python's argparse is fully matured module to do this stuff but in c# its scratch implemented which is not so well developed.

## Syntax

Integrating argparse is very simple, you just have to implement little syntax in your powershell script. Learn more about syntax and properties from [here](/documentation/argparse/).

```ps1
#@parser {description: "gretting program"}
#@param {variable: "name", alias: "n", required: "true", help: "name to greet"}
#@param {variable: "two", alias: "t", value: "false", type: "bool", required: "false", help: "greet person two times"}

Write-Output "$name you may have a good day"

if ($two) {
	Write-Output "$name you may have a good day second time"
}
```

## Results

Now if you compile and run the compiled executable with **-h** flag, you will see this output.

```bash
greet -h
```
	
=== "Python"

	```
	usage: greet [-h] [--cat] [-t] name

	gretting program

	positional arguments:
	  name        name to greet

	optional arguments:
	  -h, --help  show this help message and exit
	  --cat       instead of running cat powershell script into console (default: false)
	  -t, --two   greet person two times (default: false)
	```

=== "C#"

	```
	usage: greet.exe [-h] [--cat] [-n NAME] [-t]

	gretting program

	postional arguments:
	  -n, --name <NAME>            name to greet

	optional arguments:
	  -t, --two                    greet person two times (default: false)
	  --cat                        instead of running cat powershell script into console (default: false)
	  -h, --help                   show this help message and exit

	examples:
	        greet.exe --cat > greet.ps1
	```