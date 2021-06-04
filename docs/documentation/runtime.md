# Runtime

During runtime psburn updates and creates some of powershell script variables to use them in script.

| Variable          | Values                                | Type   |
|-------------------|---------------------------------------|--------|
| $PSScriptRoot     | Same path where executable is located | string |
| $PSScriptTempRoot | Original path of extracted script     | string |
| $Executable       | Path of powershell executable         | string |
