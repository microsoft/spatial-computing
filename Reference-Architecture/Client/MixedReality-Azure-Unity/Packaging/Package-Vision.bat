@ECHO OFF
ECHO.

REM Set Paths
SET UnityExe="%ProgramFiles%\Unity\Hub\Editor\2018.2.0f2\Editor\Unity.exe"
SET ProjectDir=%cd%\..
SET AssetDir=%ProjectDir%\Assets
SET PackageDir=%ProjectDir%\UnityPackages

REM Set Package Info
SET PackageName=MR-Vision
SET PackageVersion=1.0.0
SET PackageFileName=%PackageName%-%PackageVersion%.unitypackage
SET PackageLatestName=%PackageName%-Latest.unitypackage

ECHO Packaging %PackageName% %PackageVersion%

ECHO Defining Asset List
SET Assets=Assets\Plugins\Vision
SET Assets=%Assets% Assets\Plugins\Newtonsoft.Json
SET Assets=%Assets% Assets\MixedRealityAzure\Common
SET Assets=%Assets% Assets\MixedRealityAzure\Vision

ECHO Generating %PackageFileName% ...
%UnityExe% -batchmode -projectPath %ProjectDir%\ -exportPackage %Assets% %PackageDir%\%PackageFileName% -quit
IF ERRORLEVEL 1 GOTO ERROR
COPY /Y %PackageDir%\%PackageFileName% %PackageDir%\%PackageLatestName%

:SUCCESS
ECHO "Package Success!"
GOTO END

:ERROR
ECHO "Packaging Error"
GOTO END

:END