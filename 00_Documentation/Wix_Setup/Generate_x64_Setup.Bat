@ECHO OFF
pushd "%~dp0"
ECHO.

ECHO.
ECHO Generating MSI file please wait...
ECHO.
ECHO.

SET WIX=C:\Program Files (x86)\WiX Toolset v3.11\bin\

REM "%WIX%heat.exe" dir Release -gg -cg EdiCore -out 10_SetupScripts\Edi.wxs

"%WIX%candle.exe" 10_SetupScripts\Edi.wxs 10_SetupScripts\EdiProject.wxs -out .\30_temp_x64\ -ext WixNetFxExtension -ext WixUtilExtension

REM
REM -ext WixUIExtension enables Wix standard GUI dialogs (browse for setup location feature)
REM 
"%WIX%light.exe" .\30_temp_x64\Edi.wixobj .\30_temp_x64\EdiProject.wixobj -ext WixUIExtension -out Edi_x64.msi -ext WixNetFxExtension -ext WixUtilExtension

del Edi_x64.wixpdb

IF "%1"=="continue" GOTO Continue
PAUSE
:Continue
