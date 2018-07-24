@ECHO OFF

ECHO.
ECHO Generating All Latest Packages...

REM Set Global Paths
SET UnityExe="%ProgramFiles%\Unity\Editor\Unity.exe"
SET ProjectDir=%cd%\..\..
SET AssetDir=%ProjectDir%\Assets
SET PackageDir=%ProjectDir%\UnityPackages\Latest

CALL Package-LUIS.bat

ECHO.
ECHO.
ECHO.
PAUSE