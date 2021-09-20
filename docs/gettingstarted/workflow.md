# Workflow

psburn itself does't compile the powershell scripts but it encapulates the script inside a c# or python program and then compile those programs with specific toolchains.

## Commands

psburn comes with two methods for compiling scripts, the first one is using c# compiler (mono on linux/macos) and the second one is using python and pyinstaller. *create* command generates specific binding code for your script to run, then you can build a executable from those files using *build*.

!!! question "What happens during execution of packaged powershell script ?"
	Since psburn doesn't actually compile powershell script it stores a copy of powershell script inside it as embedded resource and extract this script to a temporary directory and run it from there.
