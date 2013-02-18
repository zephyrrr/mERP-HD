mkdir .\Reference\

copy ..\Support\Feng\*.dll .\Reference
copy ..\Support\Feng\*.pdb .\Reference
IF ERRORLEVEL 1 pause

copy ..\Support\Feng\*.exe .\Reference
IF ERRORLEVEL 1 pause

mkdir .\Reference\Help
xcopy ..\Support\Feng\Help .\Reference\Help\ /E /Y
IF ERRORLEVEL 1 pause

