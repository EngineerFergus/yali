files := main.c chunk.c memory.c

..\build\clox.exe: $(files)
	gcc -o ..\build\clox main.c chunk.c memory.c

clean:
	powershell Remove-Item ..\build\clox.exe