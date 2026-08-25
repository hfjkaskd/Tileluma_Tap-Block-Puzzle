set GEN_CLIENT=./net8.0/Luban.dll
set CONF_ROOT=../../../_Data/luban.conf
set UNITY_ASSETS_PATH=%WORKSPACE%/Assets

dotnet %GEN_CLIENT% ^
    -t client ^
    -c cs-simple-json ^
    --conf %CONF_ROOT% ^
    -x outputCodeDir=%UNITY_ASSETS_PATH%/Scripts/GeneratedCode/Luban/Code

pause