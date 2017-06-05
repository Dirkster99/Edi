@ECHO OFF
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO Xceed.Wpf.AvalonDock.AvalonDock
ECHO Xceed.Wpf.AvalonDock.AvalonDock.Themes.Expression
ECHO Xceed.Wpf.AvalonDock.AvalonDock.Themes.Metro
ECHO TestApplication
ECHO.
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO Deleting BIN and OBJ Folders in AvalonDock
ECHO.
RMDIR /S /Q Xceed.Wpf.AvalonDock\bin
RMDIR /S /Q Xceed.Wpf.AvalonDock\obj

REM DEL /F /Q /S /A:H Automation.Tasks.Core\StyleCop.Cache >> C:\TEMP\CleanFiles.txt

RMDIR /S /Q Xceed.Wpf.AvalonDock.Themes.Expression\bin
RMDIR /S /Q Xceed.Wpf.AvalonDock.Themes.Expression\obj

RMDIR /S /Q Xceed.Wpf.AvalonDock.Themes.Metro\bin
RMDIR /S /Q Xceed.Wpf.AvalonDock.Themes.Metro\obj

ECHO Deleting BIN and OBJ Folders in TestApplication folder
ECHO.
RMDIR /S /Q TestApplication\bin
RMDIR /S /Q TestApplication\obj

PAUSE

:EndOfBatch
