Set-Location ../psburn
powershell -ExecutionPolicy Bypass -File update_assets.ps1
dotnet build -c Release
Set-Location $PSScriptRoot
$env:PATH += ";../psburn/bin/Release/netcoreapp3.1"
psburn create test_script.ps1
psburn build test_script.ps1 test_script.cs
./dist/test_script.exe user