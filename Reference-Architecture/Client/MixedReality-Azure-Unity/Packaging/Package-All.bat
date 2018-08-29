@ECHO OFF

ECHO.
ECHO Generating All Packages...


CALL Package-LUIS.bat
CALL Package-Vision.bat


ECHO.
ECHO.
ECHO.
PAUSE