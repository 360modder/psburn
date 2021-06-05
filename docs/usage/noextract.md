# No Extract

no extract is a psburn feature which allows to host powershell script within command mode of powershell. Only c# builds support this feature.

```shell
powershell.exe -C "Write-Output 'Hello World'"
```

## Test

Create a file script.ps1 with contents.

```ps1
$test_var = '1.0.0'
Write-Output "psburn v$test_var launched"
```

Now compile script.

```shell
psburn create script.ps1 --no-extract -e && psburn build script.cs script.ps1
```

If you run *dist/script.exe*, you will see this output.

```
psburn v1.0.0 launched
```