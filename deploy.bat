cd ..
mkdir Hd2Deploy
del /Q .\Hd2Deploy\*.*

cd .\Feng
rem call "build-vs.bat"
call "DeployAssemblies.bat"
cd..

cd .\Hd2
call "CopyAssembliesFeng.bat"
rem  call "build-vs.bat"
call "CopyConfig.bat"

cd .\Reference
del *.log
del log.txt*
del *.vshost.*
cd..

copy .\Reference\*.* ..\Hd2Deploy

copy .\Release.config ..\Hd2eploy\Feng.Run.exe.config

cd ..\Hd2Deploy

del *.pdb
del *.log
del *.bak
del *.sql
del log.txt*
del *.vshost.*
del schemaexport.*
del Hd.Run.*
del HibernatingRhinos.NHibernate.Profiler.Appender.dll
del SqlServerProject.dll
del ipy.exe
rem del ADInfosUtil.*

rmdir hdm

del Hd2.*
rename Feng.Run.exe.config Hd.exe.config
copy ..\Hd.Run.exe .\Hd.exe


cd ..\Hd2

IF ERRORLEVEL 1 pause
