files := main.c chunk.c memory.c debug.c value.c vm.c scanner.c compiler.c object.c

..\build\clox.exe: $(files)
	gcc -o ..\build\clox $(files)

clean:
	powershell if (Test-Path ..\build\clox.exe) { Remove-Item ..\build\clox.exe }