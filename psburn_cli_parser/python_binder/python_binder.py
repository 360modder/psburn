import os
import sys
import argparse
import platform
import subprocess


class ArgumentDataNamespace:

	def __init__(self, ParsedArgumentData: str):
		Data = ParsedArgumentData.split(",")
		self.Argument = Data[0]
		self.Type = Data[1]
		self.Required = Data[2] == "true"
		self.Value = Data[3]
		self.Alias = Data[4]
		self.Help = Data[5]

def resource_path(relative_path):
    """ Get absolute path to resource, works for dev and for PyInstaller """
    try:
        # PyInstaller creates a temp folder and stores path in _MEIPASS
        base_path = sys._MEIPASS
    except Exception:
        base_path = os.path.abspath(".")

    return os.path.join(base_path, relative_path)

	
# will come - means that these variables will be pushed here at time of creation.
ProgramDescription = "dynamic embedded powershell script" # will come
ExPolicy = "Bypass" # will come
BlockCat = False # will come
OneDir = False # will come
ParsedParameters = [] # will come
PSScriptFile = "binding_psscript.ps1" # will come

# Argument parser integration
ArgumentParser = argparse.ArgumentParser(description=ProgramDescription)

for ParsedArgumentData in ParsedParameters:
	Arg = ArgumentDataNamespace(ParsedArgumentData)

	if Arg.Required:
		ArgumentParser.add_argument(Arg.Argument, help=Arg.Help)

	else:
		DestForArgument = Arg.Argument.replace("-", "")
		HelpText = Arg.Help + f" [default: {Arg.Value}]"

		if Arg.Alias != "":
			if Arg.Type == "bool":
				BoolSwitch = "store_true" if Arg.Value == "false" else "store_false"
				ArgumentParser.add_argument(f"-{Arg.Alias}", f"--{Arg.Argument}", dest=DestForArgument, action=BoolSwitch, help=HelpText)
			else:
				ArgumentParser.add_argument(f"-{Arg.Alias}", f"--{Arg.Argument}", dest=DestForArgument, help=HelpText)

		elif Arg.Type == "bool":
			BoolSwitch = "store_true" if Arg.Value == "false" else "store_false"
			ArgumentParser.add_argument(f"--{Arg.Argument}", dest=DestForArgument, action=BoolSwitch, help=HelpText)
		else:
			ArgumentParser.add_argument(f"--{Arg.Argument}", dest=DestForArgument, help=HelpText)

ArgumentParser.add_argument("--cat", dest="cat", action="store_true", help="instead of running cat powershell script into console [default: false]")
ParsedArgparseArguments = ArgumentParser.parse_args()

# print powershell script if --cat argument is supplied
with open(resource_path(PSScriptFile), encoding="utf-8") as f:
	PSScriptFile = f.read()

if ParsedArgparseArguments.cat:
	if BlockCat:
		print("error: cat is blocked during runtime")
		sys.exit(1)
	else:
		print(PSScriptFile)
		sys.exit(0)

# Code generation for supplied arguments
ParsedArgparseArgumentsDictionary = {
    Argument: Value
    for Argument, Value in ParsedArgparseArguments._get_kwargs()
}

PSEmbedString = ""
for ParsedArgumentData in ParsedParameters:
	Arg = ArgumentDataNamespace(ParsedArgumentData)
	if Arg.Type == "bool":
		PSEmbedString += f"${Arg.Argument} = ${str(ParsedArgparseArgumentsDictionary[Arg.Argument]).lower()}\n"
	elif Arg.Type == "double":
		PSEmbedString += f"${Arg.Argument} = {ParsedArgparseArgumentsDictionary[Arg.Argument]}\n"
	elif Arg.Type == "string":
		PSEmbedString += f"${Arg.Argument} = '{ParsedArgparseArgumentsDictionary[Arg.Argument]}'\n"

# Unzip essentials to temporay directory
IsWindows = platform.system().lower().startswith("win")
StorageDirectory = resource_path("")
PSScriptRoot = os.path.split(sys.executable)[0]

# Determining the path of powershell executable
OneFile = not OneDir
Executable = "pwsh.exe" if IsWindows else "pwsh"
Executable = os.path.join(StorageDirectory, "pwsh", Executable) if OneFile else os.path.join(PSScriptRoot, "pwsh", Executable)

if not os.path.exists(Executable):
	Executable = "powershell.exe" if IsWindows else "pwsh"

# Writting a new powershell script to temporary path
PSEmbedString = f"$Executable = '{Executable}'\n" + PSEmbedString
PSEmbedString = f"$PSScriptTempRoot = '{StorageDirectory}'\n" + PSEmbedString
PSEmbedString = f"$PSScriptRoot = '{PSScriptRoot}'\n"  + PSEmbedString
PSEmbedString += "\n" + PSScriptFile

TempScriptPath = os.path.join(StorageDirectory, "temp.ps1")

with open(TempScriptPath, "w", encoding="utf-8") as f:
	f.write(PSEmbedString)

# Final call to powershell
subprocess.run([Executable, "-ExecutionPolicy", ExPolicy, "-File", TempScriptPath])
