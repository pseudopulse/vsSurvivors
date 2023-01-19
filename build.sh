rm -rf vsSurvivors/bin
dotnet restore
dotnet build
rm -rf vsSurvivors/bin/Debug/netstandard2.0/RiskOfOptions.dll
rm -rf ~/.config/r2modmanPlus-local/RiskOfRain2/profiles/vsSurvivors/BepInEx/plugins/vsSurvivors
cp -r vsSurvivors/bin/Debug/netstandard2.0/  ~/.config/r2modmanPlus-local/RiskOfRain2/profiles/vsSurvivors/BepInEx/plugins/vsSurvivors

rm -rf vsBuild
mkdir vsBuild
cp icon.png vsBuild
cp manifest.json vsBuild
cp README.md vsBuild
cp vsSurvivors/bin/Debug/netstandard2.0/vsSurvivors.dll vsBuild
cd vsBuild
rm ../vsSurvivors.zip
zip ../vsSurvivors.zip *
cd ..
