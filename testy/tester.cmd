@echo off
copy ..\bin\Debug\inkom-eMTe.exe .\inkom-eMTe.exe
echo Tyto testy co nesmi projit:
inkom-eMTe nesmi01.mt
inkom-eMTe nesmi02.mt
inkom-eMTe nesmi03.mt
inkom-eMTe nesmi04.mt
inkom-eMTe nesmi05.mt
inkom-eMTe nesmi06.mt
inkom-eMTe nesmi07.mt
inkom-eMTe nesmi08.mt
inkom-eMTe nesmi09.mt
del nesmi*.exe
pause
echo spoustim testy co maji projit:
inkom-eMTe 01.mt
inkom-eMTe 02.mt
inkom-eMTe 03.mt
inkom-eMTe aaa.mt
inkom-eMTe op.mt
inkom-eMTe op1.mt
inkom-eMTe source.mt
del *.exe
pause