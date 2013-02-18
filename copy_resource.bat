mkdir .\Reference\LocalResource
del /Q .\Reference\LocalResource\* 

rem Copy Script
cd PythonScript
if exist pythonFiles.txt del pythonFiles.txt>nul
dir /s /b *.py > pythonFiles.txt
for /f %%i in (pythonFiles.txt ) do copy %%i ..\Reference\LocalResource
del pythonFiles.txt>nul
cd ..

rem Copy Assembly
cd Reference
copy hd.*.dll ..\Reference\LocalResource
cd..

rem Copy Report
cd Hd.Report
copy *.xsd ..\Reference\LocalResource
copy *.rpt ..\Reference\LocalResource
copy *.rdlc ..\Reference\LocalResource
cd..

rem Copy Config
cd Hd.Run
copy hd.model.*.config ..\Reference\LocalResource
copy sessionfactory.config ..\Reference\LocalResource
cd..

rem CopyConfigToRun
set ConfigDir=.\Hd.Run

copy %ConfigDir%\app.config .\Reference\Feng.Run.exe.config
copy %ConfigDir%\release.config .\Reference\Feng.Run.exe.config
copy %ConfigDir%\hd.model.*.config .\Reference

copy %ConfigDir%\app.config .\Reference\ADInfosUtil.exe.config 

copy %ConfigDir%\app.config .\Reference\neokernel.exe.config
copy %ConfigDir%\mime_types .\Reference\
copy %ConfigDir%\neokernel_props.xml .\Reference\

copy %ConfigDir%\app.config .\Reference\ipy.exe.config

.\Reference\ipy.exe .\encrypt_connectionstring.py

rem Upload Resource
.\Reference\ipy.exe .\upload_resource.py


IF ERRORLEVEL 1 pause