$licenseFilePath = "$PSScriptRoot/../License.txt"

$projectDirPath = "$PSScriptRoot/../LightBulb"
$publishDirPath = "$PSScriptRoot/bin/build/"

$portableArtifactFilePath = "$PSScriptRoot/bin/LightBulb.zip"
$installerManifestFilePath = "$PSScriptRoot/Installer.iss"

# Prepare directory
if (Test-Path $publishDirPath) {
    Remove-Item $publishDirPath -Recurse -Force
}
New-Item $publishDirPath -ItemType Directory -Force

# Build & publish
dotnet publish $projectDirPath -o $publishDirPath -c Release | Out-Host

# -- PORTABLE --

$files = @()
$files += Get-Item -Path $licenseFilePath
$files += Get-ChildItem -Path $publishDirPath

$files | Compress-Archive -DestinationPath $portableArtifactFilePath -Force

# -- INSTALLER --

$innosetupFilePath = "c:\Program Files (x86)\Inno Setup 6\ISCC.exe"

if (Test-Path $innosetupFilePath) {
    & $innosetupFilePath $installerManifestFilePath
}
else {
    Write-Host "InnoSetup installation not found. Installer will not be built."
}
