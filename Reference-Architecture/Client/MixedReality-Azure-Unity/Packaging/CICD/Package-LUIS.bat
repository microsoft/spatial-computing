@ECHO OFF
ECHO.

REM Set Package Info
SET PackageName=MR-LUIS
SET PackageLatestName=%PackageName%-Latest.unitypackage

ECHO Packaging %PackageName% Latest

ECHO Defining Asset List
SET Assets=Assets\Plugins\LUIS
SET Assets=%Assets% Assets\Plugins\Newtonsoft.Json
SET Assets=%Assets% Assets\MixedRealityAzure\Common
SET Assets=%Assets% Assets\MixedRealityAzure\LUIS
SET Assets=%Assets% Assets\MixedRealityAzure-Examples\LUIS

ECHO Generating %PackageFileName% ...
%UnityExe% -batchmode -projectPath %ProjectDir%\ -exportPackage %Assets% %PackageDir%\%PackageLatestName% -quit
IF ERRORLEVEL 1 GOTO ERROR

:SUCCESS
ECHO "Package Success!"
GOTO END

:ERROR
ECHO "Packaging Error"
GOTO END

:END