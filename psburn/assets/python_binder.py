import os
import sys
import argparse
import platform
import subprocess


class DataParser:
	
	def __init__(self, ParsedArguments):
		self.ParsedArguments = ParsedArguments

	def ParsePostionalArguments(self):
		Parameters = []

		for Argument in self.ParsedArguments:
			Variable = Argument.split(",")[0];
			Required = Argument.split(",")[2];

			if Required == "true":
				Parameters.append(Variable)

		return Parameters

	def ParseOptionalArguments(self):
		Parameters = []

		for Argument in self.ParsedArguments:
			Variable = Argument.split(",")[0];
			Required = Argument.split(",")[2];

			if Required == "false":
				Parameters.append(Variable)

		return Parameters

	def ParseArgumentsTypesDictionary(self):
		ArgumentsTypes = {}

		for Argument in self.ParsedArguments:
			Variable = Argument.split(",")[0];
			Type = Argument.split(",")[1];

			ArgumentsTypes[Variable] = Type

		return ArgumentsTypes

	def ParseArgumentsBoolsDictionary(self):
		ArgumentsBools = {}

		for Argument in self.ParsedArguments:
			Variable = Argument.split(",")[0];
			Value = Argument.split(",")[3];

			ArgumentsBools[Variable] = Value

		return ArgumentsBools

	def ParseArgumentsAliasDictionary(self):
		ArgumentsAlias = {}

		for Argument in self.ParsedArguments:
			Variable = Argument.split(",")[0];
			Alias = Argument.split(",")[4];

			if Alias == "":
				continue

			ArgumentsAlias[Variable] = Alias

		return ArgumentsAlias

	def ParseArgumentsHelpDictionary(self):
		ArgumentsHelp = {}

		for Argument in self.ParsedArguments:
			Variable = Argument.split(",")[0];
			Help = Argument.split(",")[5];

			ArgumentsHelp[Variable] = Help

		return ArgumentsHelp

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
DefaultArgumentsCode = [] # will come
HelpArgumentsTexts = [] # will come
PSScriptFile = "binding_psscript.ps1" # will come

with open(resource_path(PSScriptFile), encoding="utf-8") as f:
	PSScriptFile = f.read()

# Pre insertion for default code
PSEmbedString = "\n"

for DefaultCodeLine in DefaultArgumentsCode:
	PSEmbedString += DefaultCodeLine + "\n"

# Semi processed data parsing 
ParseParametersData = DataParser(ParsedParameters)
PostionalArguments = ParseParametersData.ParsePostionalArguments()
OptionalArguments = ParseParametersData.ParseOptionalArguments()
ArgumentsType = ParseParametersData.ParseArgumentsTypesDictionary()
ArgumentsBools = ParseParametersData.ParseArgumentsBoolsDictionary()
ArgumentsAlias = ParseParametersData.ParseArgumentsAliasDictionary()
ArgumentsHelp = ParseParametersData.ParseArgumentsHelpDictionary()

# Argument parser integration
ArgumentParser = argparse.ArgumentParser(description=ProgramDescription)
ArgumentParser.add_argument("--cat", dest="cat", action="store_true", help="instead of running cat powershell script into console (default: false)")

for PostionalArg in PostionalArguments:
	HelpForArgument = ArgumentsHelp[PostionalArg]
	ArgumentParser.add_argument(PostionalArg, help=HelpForArgument)

for OptionalArg in OptionalArguments:
	DestForArgument = OptionalArg.replace("-", "")
	HelpForArgument = ArgumentsHelp[OptionalArg]

	if OptionalArg in ArgumentsAlias.keys():
		if ArgumentsType[OptionalArg] == "bool":
			BoolSwitch = "store_true" if ArgumentsBools[OptionalArg] == "false" else "store_false"
			ArgumentParser.add_argument(f"-{ArgumentsAlias[OptionalArg]}", f"--{OptionalArg}", dest=DestForArgument, action=BoolSwitch, help=HelpForArgument)
		else:
			ArgumentParser.add_argument(f"-{ArgumentsAlias[OptionalArg]}", f"--{OptionalArg}", dest=DestForArgument, help=HelpForArgument)

	else:
		if ArgumentsType[OptionalArg] == "bool":
			BoolSwitch = "store_true" if ArgumentsBools[OptionalArg] == "false" else "store_false"
			ArgumentParser.add_argument(f"--{OptionalArg}", dest=DestForArgument, action=BoolSwitch, help=HelpForArgument)
		else:
			ArgumentParser.add_argument(f"--{OptionalArg}", dest=DestForArgument, help=HelpForArgument)

ParsedArgparseArguments = ArgumentParser.parse_args()

# print powershell script if --cat argument is supplied
if ParsedArgparseArguments.cat:
	if BlockCat:
		print("error: cat is blocked during runtime")
		sys.exit(1)
	else:
		print(PSScriptFile)
		sys.exit(0)

# Post code generation for supplied arguments
ParsedArgparseArgumentsDictionary = {}

for Argument, Value in ParsedArgparseArguments._get_kwargs():
	ParsedArgparseArgumentsDictionary[Argument] = Value

AllArguments = " ".join(sys.argv)

def PowershellCodeGenerator(Arguments, DestForArgument, ArgumentsType=ArgumentsType, ParsedArgparseArgumentsDictionary=ParsedArgparseArgumentsDictionary):
	PSEmbedString = ""

	if ArgumentsType[Arguments] == "bool":
		BoolSwitch = "$true" if ParsedArgparseArgumentsDictionary[DestForArgument] else "$false"
		PSEmbedString += f"${DestForArgument} = {BoolSwitch}\n"

	elif ArgumentsType[Arguments] == "float":
		PSEmbedString += f"${DestForArgument} = {ParsedArgparseArgumentsDictionary[DestForArgument]}\n"

	elif ArgumentsType[Arguments] == "string":
		PSEmbedString += f"${DestForArgument} = '{ParsedArgparseArgumentsDictionary[DestForArgument]}'\n"

	return PSEmbedString

for PostionalArg in PostionalArguments:
	DestForArgument = PostionalArg.replace("-", "")
	PSEmbedString += PowershellCodeGenerator(PostionalArg, DestForArgument)

for OptionalArg in OptionalArguments:
	DestForArgument = OptionalArg.replace("-", "")

	if OptionalArg in ArgumentsAlias.keys():
		if f"--{OptionalArg}" in AllArguments or f"-{ArgumentsAlias[OptionalArg]}" in AllArguments:
			PSEmbedString += PowershellCodeGenerator(OptionalArg, DestForArgument)

	else:
		if f"--{OptionalArg}" in AllArguments:
			PSEmbedString += PowershellCodeGenerator(OptionalArg, DestForArgument)


# Unzip essentials to temporay directory
IsWindows = platform.system().lower().startswith("win")
StorageDirectory = resource_path("")
PSScriptRoot = os.path.split(sys.executable)[0]

# Determining the path of powershell executable
OneFile = True if OneDir == False else False
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
