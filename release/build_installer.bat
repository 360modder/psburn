@echo off

echo Creating installer package for win-x64
echo Editing psburn.csproj
python edit_csproj.py win-x64
echo Running dotnet publish
cd ../psburn
dotnet publish
cd ../release
echo Running iscc.exe
"C:\Program Files (x86)\Inno Setup 6\iscc.exe" setup.iss
