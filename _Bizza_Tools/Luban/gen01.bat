set WORKSPACE=../..
set LUBAN_DLL=./net8.0/Luban.dll
set CONF_PATH=%WORKSPACE%/../_Data/luban01.conf
set UNITY_ASSETS_PATH=%WORKSPACE%/Assets


echo %UNITY_ASSETS_PATH%

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-simple-json ^
    -d json  ^
    --conf %CONF_PATH%^
    --excludeTag never test data02 data03 ^
    -x pathValidator.rootDir=%WORKSPACE% ^
    -x outputCodeDir=%UNITY_ASSETS_PATH%/Game/Scripts/GeneratedCode/Luban/Code ^
    -x outputDataDir=%UNITY_ASSETS_PATH%/Game/Resources/ConfigAssets/TableJson/Table01 ^   
