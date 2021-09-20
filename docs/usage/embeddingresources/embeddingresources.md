# Embedding Resources

psburn can embed resources to your powershell script. You just need to place all files inside one directory and you can access the path to those files by using **$PSScriptRoot** and **$PSScriptTempRoot**.

Suppose you created a directory structure like this.

```
resources/
	message.txt
```

You can access message.txt path by a function.

```ps1
function resource_path([string]$resourcefile) {
    Join-Path $PSScriptTempRoot $resourcefile
}

$message_file = resource_path("resources/message.txt")
Write-Output $message_file
```

For build the executable, first compress resources directory to a zip file and then run the following commands.

```bash
psburn create script.ps1 --embed-resources
psburn build script.ps1 script.cs -r resources.zip
```
