# Installations

Since psburn is a cross platform tool, its binaries are distributed for windows, linux and macos. Binaries are compressed inside a *zip* and *tarball* file. You first need to choose a building envoirnment mentioned below. Then you can install psburn from binary archive to your system.

## Building Envoirnments

Building envoirnments are necessary to build a executable out a file using psburn. Choose one such envoirnment from python and c#. python envoirnment is recommended on linux and macos and c# on windows. 

=== "Python"

	[python3.6+](https://www.python.org/downloads/) and [pip](https://pip.pypa.io/en/stable/installing/) should be installed, then install pyinstaller.

	```bash
	pip install pyinstaller
	```

=== "C#"

	Windows does't require any C# envoirnment as it comes integrated with .NET Framework but on linux and macos a compatible version of [mono](https://www.mono-project.com/download/stable/) should be installed.

## Download

[Windows :material-microsoft-windows:](https://github.com/clitic/psburn/releases/download/v1.1.3/psburn.1.1.3.win-x64.exe){ .md-button .md-button--primary }

## Binary Archives

Binary archives installations steps on linux and macos.

=== "Linux"

	```bash
	# Download the psburn '.tar.gz' archive
	curl -L -o /tmp/psburn.tar.gz https://github.com/clitic/psburn/releases/download/v1.1.3/psburn.1.1.3.linux-x64.tar.gz

	# Create the target folder where psburn will be placed
	sudo mkdir -p /opt/clitic/psburn

	# Expand psburn to the target folder
	sudo tar zxf /tmp/psburn.tar.gz -C /opt/clitic/psburn

	# Set execute permissions
	sudo chmod +x /opt/clitic/psburn/psburn

	# Create the symbolic link that points to psburn
	sudo ln -s /opt/clitic/psburn/psburn /usr/bin/psburn

	# Uninstall binary archive
	# sudo rm -rf /usr/bin/psburn/psburn /opt/clitic/psburn
	```

=== "MacOS"

	```bash
	# Download the psburn '.tar.gz' archive
	curl -L -o /tmp/psburn.tar.gz https://github.com/clitic/psburn/releases/download/v1.1.3/psburn.1.1.3.osx-x64.tar.gz

	# Create the target folder where psburn will be placed
	sudo mkdir -p /usr/local/clitic/psburn

	# Expand psburn to the target folder
	sudo tar zxf /tmp/psburn.tar.gz -C /usr/local/clitic/psburn

	# Set execute permissions
	sudo chmod +x /usr/local/clitic/psburn/psburn

	# Create the symbolic link that points to psburn
	sudo ln -s /usr/local/clitic/psburn/psburn /usr/local/bin/psburn

	# Uninstall binary archive
	# sudo rm -rf /usr/local/bin/psburn /usr/local/clitic/psburn
	```
