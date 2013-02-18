cd .\Reference
mkdir hbm

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Base
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Base\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Ba
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Ba\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Yw
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Yw\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Jk
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Jk\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Jk2
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Jk2\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Ck
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Ck\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Nmcg
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Nmcg\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Kj
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Kj\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Cn
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Cn\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Fp
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Fp\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Cx
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Cx\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Px
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Px\Domain.hbm.xml

del /Q ..\Reference\hbm\*
..\Reference\ADInfosUtil.exe -hbm Hd.Model.Ky
copy /B /Y ..\Reference\hbm\* ..\Hd.Model.Ky\Domain.hbm.xml

IF ERRORLEVEL 1 pause

cd ..