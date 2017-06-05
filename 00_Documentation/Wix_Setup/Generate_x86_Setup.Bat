@ECHO OFF
pushd "%~dp0"
ECHO.

ECHO.
ECHO Generating MSI file please wait...
ECHO.
ECHO.

SET WIX=C:\Program Files (x86)\WiX Toolset v3.11\bin\

REM "%WIX%heat.exe" dir Release -gg -cg EdiCore -out 20_SetupScripts_x86\Edi.wxs

"%WIX%candle.exe" 20_SetupScripts_x86\Edi.wxs 20_SetupScripts_x86\EdiProject.wxs -out .\40_temp_x86\ -ext WixNetFxExtension  -ext WixUtilExtension

REM
REM -ext WixUIExtension enables Wix standard GUI dialogs (browse for setup location feature)
REM 
"%WIX%light.exe" .\40_temp_x86\Edi.wixobj .\40_temp_x86\EdiProject.wixobj -ext WixUIExtension -out Edi_x86.msi -ext WixNetFxExtension  -ext WixUtilExtension

del Edi_x86.wixpdb

IF "%1"=="continue" GOTO Continue
PAUSE
:Continue
