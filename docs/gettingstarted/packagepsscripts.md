# Package Powershell Scripts

If you are on *windows* then c# api works without any dependencies, means you can run commands without installing any other dependencies. To use python api, *pyinstaller* must be installed on your system.

## Packaging

Generate binder file to run powershell scripts with program.

```bash
psburn create script.ps1
```

This command will generated a c# binder file named after the basename of your powershell script. Now build a executable out of this file by running **build** command.

```bash
psburn build script.ps1 script.cs
```

Your powershell script will be packaged under dist folder of working directory. Use [mkbundle](https://www.mono-project.com/docs/tools+libraries/tools/mkbundle/) to bundle exe if you are using mono.

To package scripts using python, just add **--py** flag in *create* command to generate python binder file instead of c#.

```bash
psburn create script.ps1 --py
psburn build script.ps1 script.py
```

## Merging dll and exe files

If you use c# binder file to build a executable then it generates dll which are required by executable to run properly, you can merge this dll and exe using [ilmerge](https://github.com/dotnet/ILMerge) or [ilrepack](https://github.com/gluck/il-repack). You can also use **--merge** flag on windows, which will use ilmerge to merge dll and exe.

```bash
psburn build script.ps1 script.cs --merge
```

=== "Windows"

	```bash
	ILMerge /o:a.exe main_file.exe PsburnCliParser.dll ICSharpCode.SharpZipLib.dll
	```

=== "Linux/MacOS"

	```bash
	mono ILRepack.exe /o:a.exe main_file.exe PsburnCliParser.dll ICSharpCode.SharpZipLib.dll 
	```

## Access more options with build command

You can access more options like icon, name etc. while build executables with build command, just add **--pyinstaller-prompt** flag to your build command. Then you will be prompted to supply extra arguments to pyinstaller. You can check pyinstaller help for available options.

```bash
pyinstaller -h
```

## Oops got an error while building an executable, what to do ?

!!! error "Error: csc path not found"
	In linux and macos you will get this error, fix this error by adding **--cscpath csc** flag in build command.
