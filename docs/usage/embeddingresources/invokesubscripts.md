# Invoke Sub Scripts

You can also invoke multiple sub scripts from a single executable by using **$Executable** in your powershell script which stores the path for powershell executable.

## Test

Create directory structure like this.

```
app.ps1			# main psscript
resources/
	setup.ps1	# sub script 1
	clean.ps1	# sub script 2
```

Contents of scripts.

=== "app.ps1"

	```ps1
	function resource_path([string]$resourcefile) {
	    Join-Path $PSScriptTempRoot $resourcefile
	}

	Write-Output "Launching Up..."

	$resourcefileSetup = resource_path("resources/setup.ps1")
	$resourcefileClean = resource_path("resources/clean.ps1")

	. $resourcefileSetup -Id "1.2.3"
	. $resourcefileClean
	```

=== "setup.ps1"

	```ps1
	param($Id)

	Write-Output "$Id Setting Up..."
	```

=== "clean.ps1"

	```ps1
	Write-Output "Cleaning Up..."
	```

Now compress this resources directory to a zip file and build the executable.

=== "Python"

	```bash
	psburn cross app.ps1 -o app.py
	psburn cbuild app.py app.ps1 -r resources.zip --no-prompt
	```

=== "C#"

	```bash
	psburn create app.ps1 -o app.cs --embed-resources
	psburn build app.cs app.ps1 -r resources.zip
	```

Now if you run compiled executable, you will see this output.

```
Launching Up...
1.2.3 Setting Up...
Cleaning Up...
```