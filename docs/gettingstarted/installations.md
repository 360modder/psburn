# Installations

Since psburn is a cross platform tool, its binaries are distributed for windows, linux and macos. Binaries are compressed inside a *zip* and *tarball* file. You first need to choose a building envoirnment mentioned below. Then you can install psburn from binary archive to your system.

## Building Envoirnments

Building envoirnments are necessary to build a executable out a file using psburn. Choose one such envoirnment from python or c#. python envoirnment is recommended on linux and macos and c# on windows. 

=== "Python"

	[python3.6+](https://www.python.org/downloads/) and [pip](https://pip.pypa.io/en/stable/installing/) should be installed, then install pyinstaller.

	```bash
	pip install pyinstaller
	```

=== "C#"

	Windows does't require any C# envoirnment as it comes integrated with .NET Framework but on linux and macos a compatible version of [mono](https://www.mono-project.com/download/stable/) should be installed.

## Binary Archives

Binary archives installations steps on windows, linux and macos.

=== "Windows"

	```ps1
	# Download the zipped binary
	Invoke-WebRequest https://github.com/360modder/psburn/releases/download/v1.1.3/psburn.1.1.3.win-x64.zip -o psburn.1.1.3.win-x64.zip

	# Extract zipped binary
	Expand-Archive psburn.1.1.3.win-x64.zip psburn.1.1.3.win-x64

	# Setup path envoirnment variables
	$env:Path = "psburn.1.1.3.win-x64;$env:Path"
	```

=== "Linux"

	```bash
	# Download the psburn '.tar.gz' archive
	curl -L -o /tmp/psburn.tar.gz https://github.com/360modder/psburn/releases/download/v1.1.3/psburn.1.1.3.linux-x64.tar.gz

	# Create the target folder where psburn will be placed
	sudo mkdir -p /opt/360modder/psburn

	# Expand psburn to the target folder
	sudo tar zxf /tmp/psburn.tar.gz -C /opt/360modder/psburn

	# Set execute permissions
	sudo chmod +x /opt/360modder/psburn/psburn

	# Create the symbolic link that points to psburn
	sudo ln -s /opt/360modder/psburn/psburn /usr/bin/psburn

	# Uninstall binary archive
	# sudo rm -rf /usr/bin/psburn/psburn /opt/360modder/psburn
	```

=== "MacOS"

	```bash
	# Download the psburn '.tar.gz' archive
	curl -L -o /tmp/psburn.tar.gz https://github.com/360modder/psburn/releases/download/v1.1.3/psburn.1.1.3.osx-x64.tar.gz

	# Create the target folder where psburn will be placed
	sudo mkdir -p /usr/local/360modder/psburn

	# Expand psburn to the target folder
	sudo tar zxf /tmp/psburn.tar.gz -C /usr/local/360modder/psburn

	# Set execute permissions
	sudo chmod +x /usr/local/360modder/psburn/psburn

	# Create the symbolic link that points to psburn
	sudo ln -s /usr/local/360modder/psburn/psburn /usr/local/bin/psburn

	# Uninstall binary archive
	# sudo rm -rf /usr/local/bin/psburn /usr/local/360modder/psburn
	```
