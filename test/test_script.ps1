#@parser {description: "gretting program"}
#@param {variable: "name", alias: "n", required: "true", help: "name to greet"}
#@param {variable: "twice", alias: "t", value: "false", type: "bool", required: "false", help: "greet person two times"}

Write-Output "$name you may have a good day"

if ($two) {
	Write-Output "$name you may have a good day second time"
}