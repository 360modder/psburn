# Modify your script before using psburn

# RULE 1: Use of "" double qoutes is not allowed
# RULE 2: When using command line variables then don't declare those variable in your script

# EXAMPLE 1: To display output
# Don't use --> Write-Output "Some Fixed Variables:"
# Alternatively use
Write-Output 'Some Fixed Variables:'

# EXAMPLE 2: To join two strings
# Don't use --> $modify = "PSScriptRoot: $PSScriptRoot"
# Alternatively use
$modify = 'PSScriptRoot: ' + $PSScriptRoot
Write-Output $modify

# EXAMPLE 3: To display two or more string outputs on same line
# Don't use --> Write-Output
# Instead you can use
Write-Host 'Hello' 'World' '!'

# EXAMPLE 4: Setting variables from command line
# Note: Don't declare this variable in your script
# Use --> rules.exe -cat -name 360modder
# This will create a variable structure like this,
# $name = '360modder'
Write-Host 'Variable Name Value:' $name
