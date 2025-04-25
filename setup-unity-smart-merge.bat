@echo off
setlocal

REM Change this to match your Unity version if different
set UNITY_PATH="C:/Program Files/Unity/Hub/Editor/6000.0.46f1/Editor/Data/Tools/UnityYAMLMerge.exe"

echo Configuring Unity Smart Merge for Git...

git config --global merge.unityyamlmerge.name "Unity Smart Merge"
git config --global merge.unityyamlmerge.driver %UNITY_PATH% merge -p %%O %%B %%A %%A

echo.
echo ✅ Done! Unity Smart Merge has been configured.
echo Git will now use UnityYAMLMerge for .prefab, .unity, and .asset files.
pause
