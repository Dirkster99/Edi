@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO Compiling...
ECHO.
%windir%\microsoft.net\framework\v4.0.30319\msbuild /m Xceed.Wpf.AvalonDock.sln /p:Configuration=Release "/p:Platform=Any CPU"
@IF %ERRORLEVEL% NEQ 0 GOTO err
@exit /B 0
:err
@PAUSE
@exit /B 1
