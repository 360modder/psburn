# Gui Applications

<p align="center">
  <img src="https://raw.githubusercontent.com/360modder/quickstart-ps-gui/master/images/gui.gif">
</p>

Create a windows gui application with [ModernWpf](https://github.com/Kinnara/ModernWpf) and Powershell. Checkout the sample [release](https://github.com/360modder/quickstart-ps-gui/releases/download/v1.0/quickstart-ps-gui.1.0.zip).

## First Run

First clone this repository.

```bash
git clone https://github.com/360modder/quickstart-ps-gui.git
```

Write your code inside **src** directoray and run the gui app by running this command.

```bash
powershell -ExecutionPolicy Bypass -File "src\MainWindow.ps1"
```

To compile the gui, install [psburn](https://github.com/360modder/psburn) and run **build.bat**. gui will compiled under dist directory.

## Upgrading Libraries

Upgrade [ModernWpf](https://github.com/Kinnara/ModernWpf) library from it's nuget package inside **src/net45** directory. System.ValueTuple.dll can be updated from .NET Framework standard libarary.
