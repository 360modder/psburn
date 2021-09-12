# Workflow

psburn itself does't compile the powershell scripts but it encapulates the psscript inside a python or c# program and then compile those programs with specific toolchains.

## Commands

psburn comes with two methods for compiling pscripts, the first one is using python and pyinstaller and the second one is using c# compiler (mono on linux/macos). *cross* command works for python and *create* command works for c#. *cross* and *create* commands generates specific code for your script to run, then you can build a executable from those files using *cbuild* and *build* respectively.

You can understand the psburn workflow from the below diagram.

![PsBurn Workflow](../assets/mermaid-diagram-20210531105847.svg)

!!! question "What happens during execution of packaged powershell script ?"
	Since psburn doesn't actually compile powershell script it stores a copy of powershell script inside it as embedded resource and extract this script to a temporary directory and run it from there.
