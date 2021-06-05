# Compiling Powershell Scripts

If you are on *windows* then c# commands works without any dependencies, means you can run those commands without installing any other dependencies. psburn compiles script in one single file but this behaviour can be changed by using **--onedir** flag in all commands.

## Compiling

=== "Python"

	Generate a native code code to bind your powershell scripts with python program.
	
	```shell
	psburn cross path/to/script.ps1
	```

	This command will generate a python file named after the basename of your powershell script. Now build a executable out of this file by running **cbuild** command.

	```shell
	psburn cbuild path/to/generated/script.py path/to/script.ps1 --no-prompt
	```

=== "C#"

	Generate a native code code to bind your powershell scripts with c# program.

	```shell
	psburn create path/to/script.ps1
	```

	This command will generated a c# file named after the basename of your powershell script. Now build a executable out of this file by running **build** command.

	```shell
	psburn build path/to/generated/script.cs path/to/script.ps1
	```

Your powershell script will be compiled under dist folder of working directory. Use [mkbundle](https://www.mono-project.com/docs/tools+libraries/tools/mkbundle/) to bundle exe if you are using mono.

## Access more options with cbuild command

You can access more options like icon, name etc. while build executables with cbuild command, just remove **--no-prompt** flag from your cbuild command. Then you will be prompted to supply extra arguments to pyinstaller. You can check pyinstaller help for available options.

```shell
pyinstaller -h

```

## Merging dll and exe files

If you use *build* command it generates a dll which is requires by executable to run properly, you can merge this dll and exe using ilmerge or ilrepack.

=== "Windows"

	```shell
	ILMerge ICSharpCode.SharpZipLib.dll <file.exe> /o:a.exe
	```

=== "Linux/MacOS"

	```shell
	mono ILRepack.exe ICSharpCode.SharpZipLib.dll <file.exe> /o:a.exe
	```

## Oops got an error while building an executable, what to do ?

!!! error "Error: got nothing in dist"
	You might encounter this problem while you are using relative path of files just replace relative paths with **absolute paths**.

!!! error "Error: csc path not found"
	In linux and macos you will get this error, fix this error by adding **--cscpath csc** flag in build command.
