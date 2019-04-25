@echo off
if [%1]==[/?] goto HELP
if [%1]==[-?] goto HELP
PATH=C:\PROGRA~2\MONO-2~1.7\bin;%PATH%
IF NOT [%2]==[] ( GOTO ICON )
IF [%2]==[] ( GOTO NOICON )
:NOICON
echo OUTPUT_FILE: %1
echo Compiling...
call mcs CreationFunctions.cs Program.cs Properties\AssemblyInfo.cs -out:%1 -platform:anycpu
echo Done
GOTO EXIT
:ICON
echo OUTPUT_FILE: %1
echo ICON_FILE: %2
echo Compiling...
call mcs CreationFunctions.cs Program.cs Properties\AssemblyInfo.cs -out:%1 -platform:anycpu -win32icon:%2
echo Done
GOTO EXIT
:HELP
echo.     %0 OUTPUT_FILE ICON_FILE
:EXIT