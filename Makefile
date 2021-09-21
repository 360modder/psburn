GitBashPath := "C:\\Program Files\\Git\\bin\\bash.exe"
InnoSetupPath := "C:\\Program Files (x86)\\Inno Setup 6\\iscc.exe"
PsburnVersion := "1.1.3"

.PHONY: help test clean update_assets release setup

help:
	@echo make targets:
	@echo   test 			run a test for script compile task
	@echo   clean			clean bin and test generated files
	@echo   update_assets 	update assets file in assets directory
	@echo   release 		package tarball distribution for linux-x64 and osx-x64
	@echo   setup			create inno setup installer for win-x64
	@echo   help			shows this help message

test:
	@cd test && powershell -ExecutionPolicy Bypass -File cli_test.ps1

clean:
	@powershell -C "Remove-Item psburn/bin -Recurse"
	@powershell -C "Remove-Item psburn_cli_parser/bin -Recurse"
	@powershell -C "Remove-Item psburn_cli_parser/csharp_binder/bin -Recurse"
	@powershell -C "Remove-Item test/dist -Recurse"
	@powershell -C "Remove-Item test/test_script.cs"

update_assets:
	@cd psburn && powershell -ExecutionPolicy Bypass -File update_assets.ps1

release: update_assets
	@cd psburn && dotnet publish -c Release -r linux-x64 -p:PublishTrimmed=true
	@$(GitBashPath) -c "tar -czvf release/psburn.$(PsburnVersion).linux-x64.tar.gz -C psburn/bin/Release/netcoreapp3.1/linux-x64/publish ."
	@cd psburn && dotnet publish -c Release -r osx-x64 -p:PublishTrimmed=true
	@$(GitBashPath) -c "tar -czvf release/psburn.$(PsburnVersion).osx-x64.tar.gz -C psburn/bin/Release/netcoreapp3.1/osx-x64/publish ."

setup: update_assets
	@cd psburn && dotnet publish -c Release -r win-x64 -p:PublishTrimmed=true
	@cd release && $(InnoSetupPath) setup.iss
