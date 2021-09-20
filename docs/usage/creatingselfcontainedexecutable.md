# Creating Self Contained Executable

psburn can also build self contained executables, means that you need not to install powershell on every system. It uses **powershell core** to do this. For making self contained executables the main recipe is a zip file which contains os specific powershell core binaries.

## Preparing A Zip

You can [download](https://github.com/PowerShell/PowerShell/releases) the latest binary archives of powershell core of your system from official github repository of [powershell](https://github.com/PowerShell/PowerShell).

=== "Windows"

	```ps1
	Invoke-WebRequest https://github.com/PowerShell/PowerShell/releases/download/v7.1.3/PowerShell-7.1.3-win-x64.zip -o PowerShell-7.1.3-win-x64.zip
	```

	After downloading the binary archive you will have to extract all files inside a directory named **pwsh**.

	```ps1
	Expand-Archive -Path PowerShell-7.1.3-win-x64.zip -DestinationPath pwsh
	```

=== "Linux"

	```bash
	wget https://github.com/PowerShell/PowerShell/releases/download/v7.1.3/powershell-7.1.3-linux-x64.tar.gz -O powershell-7.1.3-linux-x64.tar.gz
	```

	After downloading the binary archive you will have to extract all files inside a directory named **pwsh**.

	```bash
	mkdir pwsh && tar -xf powershell-7.1.3-linux-x64.tar.gz -C pwsh
	```

=== "MacOS"

	```bash
	wget https://github.com/PowerShell/PowerShell/releases/download/v7.1.3/powershell-7.1.3-osx-x64.tar.gz -O powershell-7.1.3-osx-x64.tar.gz
	```

	After downloading the binary archive you will have to extract all files inside a folder named **pwsh**.

	```bash
	mkdir pwsh && tar -xf powershell-7.1.3-osx-x64.tar.gz -C pwsh
	```

You should attain this type of directory structure.

```
pwsh/
    *.dll     # dll files.
    *.so      # lib files.
    *.exe     # exe files.
    ...       # other files and folders.
```

Now compress pwsh directory into a zip file.

=== "Windows"

	```ps1
	Compress-Archive -Path pwsh -DestinationPath pwsh.zip
	```

=== "Linux/MacOS"

	```bash
	zip -r pwsh.zip pwsh 
	```

## Building Executable

```bash
psburn create script.ps1 --self-contained
psburn build script.ps1 script.cs -p pwsh.zip
```

!!! note
	Zip compression level can make executable large or small size for **only c#** builds.

!!! note
	Use **--onedir** flag in all commands for faster booting time of packaged scripts.
