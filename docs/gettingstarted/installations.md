# Installations

Since psburn is a cross platform tool, its binaries are distributed for windows, linux and macos. Binaries are compressed inside a *zip* file. You can either click the below download buttons or use curl command to download it from terminal.

## Direct Download

[Windows :material-microsoft-windows:](#){ .md-button .md-button--primary }
[Linux :material-linux:](#){ .md-button .md-button--primary }
[MacOS :material-apple:](#){ .md-button .md-button--primary }

## Command Line Installations

=== "Windows"

	Download the zipped binary.

	```ps1
	Invoke-WebRequest https://github.com/360modder/psburn/releases/download/v1.0.0/psburn-1.0.0-win.zip -o psburn-1.0.0-win.zip
	```
	
	Extract zipped binary and remove zip file.

	```ps1
	Expand-Archive .\psburn-1.0.0-win.zip .
	Remove-Item psburn-1.0.0-win.zip
	```

	Setup temporary envoirnment variables.

	```ps1
	$env:Path = "psburn-1.0.0-win/bin;$env:Path"
	```

=== "Linux"

	Download the zipped binary.

	```shell
	wget https://github.com/360modder/psburn/releases/download/v1.0.0/psburn-1.0.0-linux.zip -O psburn-1.0.0-linux.zip
	```

	Extract zipped binary and remove zip file.

	```shell
	unzip psburn-1.0.0-linux.zip
	rm psburn-1.0.0-linux.zip
	```

	Export temporary path for executables.

	```shell
	export PATH=$PATH/bin:psburn-1.0.0-linux/bin
	```

=== "MacOS"

	Download the zipped binary.

	```shell
	wget https://github.com/360modder/psburn/releases/download/v1.0.0/psburn-1.0.0-osx.zip -O psburn-1.0.0-osx.zip
	```

	Extract zipped binary and remove zip file.

	```shell
	unzip psburn-1.0.0-osx.zip
	rm psburn-1.0.0-osx.zip
	```

	Export temporary path for executables.

	```shell
	export PATH=$PATH/bin:psburn-1.0.0-osx/bin
	```

## Building Envoirnments

Building envoirnments are necessary to build a executable out a file using psburn. Choose one such envoirnment from python or c#.

=== "Python"

	[python3.6+](https://www.python.org/) and [pip](https://pip.pypa.io/en/stable/installing/) should be installed if you are using **cross** command then install pyinstaller.

	```shell
	pip install pyinstaller
	```

=== "C#"

	Windows does't require any C# envoirnment as its come integrated with .NET Framework but on linux and macos a compitible version of [mono](https://www.mono-project.com/download/stable/) should be installed if you are using **create** command.
