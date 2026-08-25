set WORKSPACE=../..
set LUBAN_DLL=./net8.0/Luban.dll
set CONF_ROOT=%WORKSPACE%/../_Data/Data
set UNITY_ASSETS_PATH=%WORKSPACE%/Assets


echo %UNITY_ASSETS_PATH%

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-simple-json ^
    -d json  ^
    --conf %CONF_ROOT%/luban.conf ^
    --excludeTag never ^
    -x pathValidator.rootDir=%WORKSPACE% ^
    -x outputCodeDir=%UNITY_ASSETS_PATH%/Scripts/GeneratedCode/Luban/Code ^
    -x outputDataDir=%UNITY_ASSETS_PATH%/GameRes/ConfigAssets/TableJson ^   

pause