@ECHO OFF


REM Set Paths
SET Version=1.0.0
SET UnityExe="%ProgramFiles%\Unity\Hub\Editor\2017.4.1f1\Editor\Unity.exe"
SET ProjectDir=%cd%\..
SET AssetDir=%ProjectDir%\Assets
SET PackageDir=%ProjectDir%\UnityPackages


REM Build Asset List for Package
SET Assets=Assets\Plugins\LUIS
SET Assets=%Assets% Assets\Plugins\LUIS
SET Assets=%Assets% Assets\Plugins\Newtonsoft.Json
SET Assets=%Assets% Assets\Plugins\System.Collections
SET Assets=%Assets% Assets\Plugins\System.Runtime
SET Assets=%Assets% Assets\Plugins\System.Threading.Tasks
SET Assets=%Assets% Assets\MixedRealityAzure\Common
SET Assets=%Assets% Assets\MixedRealityAzure\LUIS
SET Assets=%Assets% Assets\MixedRealityAzure-Examples\LUIS


REM Package
%UnityExe% -batchmode -projectPath %ProjectDir%\ -exportPackage %Assets% %PackageDir%\LUIS-%Version%.unitypackage -quit

echo "Packaging complete."