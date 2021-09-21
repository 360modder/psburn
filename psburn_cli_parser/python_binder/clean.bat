@echo off
powershell -C "Remove-Item __pycache__ -Recurse"
powershell -C "Remove-Item build -Recurse"
powershell -C "Remove-Item dist -Recurse"
powershell -C "Remove-Item python_binder.spec"
