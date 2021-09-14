powershell -ExecutionPolicy Bypass -File update_assets.ps1
dotnet publish -r win-x64 -c Release -p:PublishTrimmed=true