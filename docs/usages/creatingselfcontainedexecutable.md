# Creating Self Contained Executable

psburn can also build self contained executables, means that you need not to install powershell on every system. For making self contained executables the main recipe is zip file which contains os specific powershell binaries.

## Preparing A Zip

You can [download](https://github.com/PowerShell/PowerShell/releases) the latest binary archives of powershell of your system from official github repository of [powershell](https://github.com/PowerShell/PowerShell).

=== "Windows"

	```ps1
	Invoke-WebRequest https://github.com/PowerShell/PowerShell/releases/download/v7.1.3/PowerShell-7.1.3-win-x64.zip -o PowerShell-7.1.3-win-x64.zip
	```

	After downloading the binary archive you will have to extract all files inside a folder named **pwsh**.

	```ps1
	Expand-Archive -Path PowerShell-7.1.3-win-x64.zip -DestinationPath pwsh
	```

=== "Linux"

	```shell
	wget https://github.com/PowerShell/PowerShell/releases/download/v7.1.3/powershell-7.1.3-linux-x64.tar.gz
	```

	After downloading the binary archive you will have to extract all files inside a folder named **pwsh**.

	```shell
	mkdir pwsh && tar -xf powershell-7.1.3-linux-x64.tar.gz -C pwsh
	```

=== "MacOS"

	```bash
	wget https://github.com/PowerShell/PowerShell/releases/download/v7.1.3/powershell-7.1.3-osx-x64.tar.gz
	```

	After downloading the binary archive you will have to extract all files inside a folder named **pwsh**.

	```shell
	mkdir pwsh && tar -xf powershell-7.1.3-osx-x64.tar.gz -C pwsh
	```

You should attain this type of directory structure.

```
pwsh/
    *.dll     # dll files.
    *.so      # libs files.
    *.exe     # exe files.
    ...       # other files and folders.
```

Now compress this directory into a zip file.

=== "Windows"

	```ps1
	Compress-Archive -Path pwsh -DestinationPath pwsh.zip
	```

=== "Linux/MacOS"

	```shell
	zip -r pwsh.zip pwsh 
	```

## Building Executable

=== "Python"
	
	```shell
	psburn cbuild path/to/generated/script.py path/to/script.ps1 -p pwsh.zip
	```

=== "C#"

	```shell
	psburn build path/to/generated/script.cs path/to/script.ps1 -p pwsh.zip
	```

!!! note
	Use **--onedir** flag for faster booting of compiled scripts.
