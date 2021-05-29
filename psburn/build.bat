dotnet publish -r win-x64 -c Release --no-self-contained -p:PublishSingleFile=true
dotnet publish -r linux-x64 -c Release -p:PublishTrimmed=true -p:PublishSingleFile=true
dotnet publish -r osx-x64 -c Release -p:PublishTrimmed=true -p:PublishSingleFile=true