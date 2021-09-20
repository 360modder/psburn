@echo off

echo creating installer package for win-x64
echo editing psburn.csproj
python edit_csproj.py win-x64
echo running dotnet publish
cd ../psburn
dotnet publish
cd ../release
echo running iscc.exe
ls
"C:\Program Files (x86)\Inno Setup 6\iscc.exe" setup.iss
