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


"""
Progam uses many variables, some variables and their meanings are listed below:
1) ExPolicy: Execution policy for powershell.
2) PSScriptFile: Main powershell script need to be runned.
4) BlockCat: Blocks cat featues.
5) PSScriptRoot: Root path of exe and acts as PSScriptRoot.
6) PSEmbedString: Main finally generated powershell script.
7) DefaultArgumentsCode: Auto generated default values of argument.
8) ParsedParameters: Parsed parameters for further parsing.
9) PSScriptName: Name of exe.
10) ProgramDescription: Desciption for program.
11) AllExamples: Examples related to use program.
12) ProgramUsage: Usage for program.
13) StorageDirectory: Temporary directory for storing files.
14) OneDir: Program is going to be hosted in one directory or not.
15) Executable: Path of powershell executable.

Special Annotations:
1) will come: means that data will be pushed here at time of compile.
3) will embed: refers to file which will be embedded during compilation.
"""

# Main script
ExPolicy = "Bypass" # will come
PSScriptFile = "path/to/psscript.ps1" # will embed
PSScriptFile = resource_path(PSScriptFile)

with open(PSScriptFile) as f:
	PSScriptFile = f.read()

# Cat features
BlockCat = False # will come

# Fixing some lost variables and appending default values to defined variables in powershell script
PSScriptRoot = os.path.split(sys.executable)[0]
PSEmbedString = f"$PSScriptRoot = '{PSScriptRoot}'\n\n"

DefaultArgumentsCode = [] # will come

PSEmbedString += "\n"

for DefaultCodeLine in DefaultArgumentsCode:
	PSEmbedString += DefaultCodeLine + "\n"

# Parse parameters from powershell script
ParsedParameters = [] # will come
ParseParametersData = DataParser(ParsedParameters)
PostionalArguments = ParseParametersData.ParsePostionalArguments()
OptionalArguments = ParseParametersData.ParseOptionalArguments()
ArgumentsType = ParseParametersData.ParseArgumentsTypesDictionary()
ArgumentsBools = ParseParametersData.ParseArgumentsBoolsDictionary()
ArgumentsAlias = ParseParametersData.ParseArgumentsAliasDictionary()
ArgumentsHelp = ParseParametersData.ParseArgumentsHelpDictionary()

# Program help related texts generations
FileBaseName = os.path.split(sys.executable)[1].replace(".exe", "")

ProgramDescription = "dynamic embedded powershell script" # will come
ProgramDescription = ProgramDescription.replace("$newline", "\n")
HelpArgumentsTexts = [] # will come
AllExamples = [] # will come
ProgramUsage = "" # will come


# Parser integration
if ProgramUsage == "":
	ArgumentParser = argparse.ArgumentParser(description=ProgramDescription)
else:
	ArgumentParser = argparse.ArgumentParser(description=ProgramDescription, usage=ProgramUsage)

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

# Extended parser integration
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

# Final merge of orginal powershell script with genrated parsed variables
PSEmbedString += "\n" + PSScriptFile

# Cat file features if blocked raise errors
if ParsedArgparseArguments.cat:
	if BlockCat:
		print("error: cat is blocked during runtime")
		sys.exit(1)
	else:
		print(PSEmbedString)
		sys.exit(0)

# Creating unique temporary directory
StorageDirectory = resource_path("")

# Self contained specific checks
OneDir = False # will come
OneFile = True if OneDir == False else False

# Determining the path of powershell executable
IsWindows = platform.system().lower().startswith("win")

Executable = "pwsh.exe" if IsWindows else "pwsh"
Executable = os.path.join(StorageDirectory, "pwsh", Executable) if OneFile else os.path.join(PSScriptRoot, "pwsh", Executable)

if os.path.exists(Executable) is not True:
	Executable = "powershell.exe" if IsWindows else "pwsh"

# Writting a dynamic powershell script to temporary path
TempScriptPath = os.path.join(StorageDirectory, "temp.ps1")
PSEmbedString = f"$PSScriptTempRoot = '{StorageDirectory}'\n" + PSEmbedString;
PSEmbedString = f"$Executable = '{Executable}'\n" + PSEmbedString;

with open(TempScriptPath, "w", encoding="utf-8") as f:
	f.write(PSEmbedString)

# Final call to powershell
subprocess.run([Executable, "-ExecutionPolicy", ExPolicy, "-File", TempScriptPath])
