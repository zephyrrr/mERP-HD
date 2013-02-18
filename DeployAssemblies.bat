copy .\Reference\Hd.*.dll ..\Application\Hd\
copy Hd.Run\release.config ..\Application\Hd\Hd.exe.config
copy Hd.Run\hd.model.*.config ..\Application\Hd\

IF ERRORLEVEL 1 pause