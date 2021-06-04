# Embedding Resources

psburn can embed resources to your powershell script. You just need to place all files inside one directory and you can access the path to those files by using **$PSScriptRoot** or **$PSScriptTempRoot**.

Suppose you created a directory structure like this and message.txt is the resource file whose path you want.

```
resources/
	message.txt
```

Now compress this directory to a zip file and build the executable.

=== "Python"

	```shell
	psburn cross path/to/script.ps1
	psburn cbuild path/to/generated/script.py path/to/script.ps1 -r resources.zip
	```

=== "C#"

	```shell
	psburn create path/to/script.ps1 --embed-resources
	psburn build path/to/generated/script.cs path/to/script.ps1 -r resources.zip
	```

You can access this path by a simple function.

```ps1
function resource_path([string]$resourcefile) {
    Join-Path $PSScriptTempRoot $resourcefile
}

$message_file = resource_path("resources/message.txt")
Write-Output $message_file
```
