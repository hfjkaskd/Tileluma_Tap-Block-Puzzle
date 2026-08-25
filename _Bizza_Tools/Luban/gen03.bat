set WORKSPACE=../..
set LUBAN_DLL=./net8.0/Luban.dll
set CONF_PATH=%WORKSPACE%/../_Data/luban03.conf
set UNITY_ASSETS_PATH=%WORKSPACE%/Assets


echo %UNITY_ASSETS_PATH%

dotnet %LUBAN_DLL% ^
    -t client ^
    -d json  ^
    --conf %CONF_PATH%^
    --excludeTag never test data02 data03 ^
    -x pathValidator.rootDir=%WORKSPACE% ^
    -x outputDataDir=%UNITY_ASSETS_PATH%/Game/Resources/ConfigAssets/TableJson/Table03 ^   

pause