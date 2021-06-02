# Invoke Sub Scripts

You can also invoke multiple subscripts from a single executable by using **$Executable** in your powershell script which stores the path for powershell executable.


## Test

Create directoray structure like this.

```
app.ps1			# main psscript
resources/
	setup.ps1	# sub script 1
	clean.ps1	# sub script 2
```

Contents of scripts.

=== "app.ps1"

	```ps1
	Write-Output "Launching Up..."

	# Path Fetching Function
	function resource_path([string]$resourcefile) {
	    Join-Path $PSScriptTempRoot $resourcefile
	}

	# Fetch Paths
	$resourcefileSetup = resource_path("resources/setup.ps1")
	$resourcefileClean = resource_path("resources/clean.ps1")

	# Run Scripts
	Start-Process -FilePath $Executable -ArgumentList "-ExecutionPolicy Bypass -File `"$resourcefileSetup`"" -NoNewWindow -Wait
	Start-Process -FilePath $Executable -ArgumentList "-ExecutionPolicy Bypass -File `"$resourcefileClean`"" -NoNewWindow -Wait
	```

=== "setup.ps1"

	```ps1
	Write-Output "Setting Up..."
	```

=== "clean.ps1"

	```ps1
	Write-Output "Cleaning Up..."
	```

Now compress this resources directory to a zip file and build the executable.

=== "Python"

	```shell
	psburn cross app.ps1 -o app.py
	psburn cbuild app.py app.ps1 -r resources.zip --no-prompt
	```

=== "C#"

	```shell
	psburn create app.ps1 -o app.cs --embed-resources
	psburn build app.cs app.ps1 -r resources.zip
	```

Now if you run compiled executable, you will see this output.

```
Launching Up...
Setting Up...
Cleaning Up...
```