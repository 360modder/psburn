Set-Location ../psburn_cli_parser/csharp_binder
dotnet build -c Release
Copy-Item bin/Release/ICSharpCode.SharpZipLib.dll "$PSScriptRoot/assets/ICSharpCode.SharpZipLib.dll"
Copy-Item bin/Release/PsburnCliParser.dll "$PSScriptRoot/assets/PsburnCliParser.dll"
Copy-Item ../csharp_binder/csharp_binder.cs "$PSScriptRoot/assets/csharp_binder.cs"
Copy-Item ../python_binder/python_binder.py "$PSScriptRoot/assets/python_binder.py"