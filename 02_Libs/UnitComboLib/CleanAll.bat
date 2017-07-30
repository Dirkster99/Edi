@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO UnitCombobox
ECHO UnitComboLib
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO Deleting BIN and OBJ Folders in FileListView
ECHO.
RMDIR /S /Q .\.vs
RMDIR /S /Q .\UnitCombobox\bin
RMDIR /S /Q .\UnitCombobox\obj
ECHO.
ECHO Deleting BIN and OBJ Folders in FileListViewTest
ECHO.
RMDIR /S /Q .\UnitComboLib\bin
RMDIR /S /Q .\UnitComboLib\obj
ECHO.

PAUSE

:EndOfBatch
