$licenseFilePath = "$PSScriptRoot/../License.txt"

$projectDirPath = "$PSScriptRoot/../LightBulb"
$publishDirPath = "$PSScriptRoot/bin/build/"
$artifactFilePath = "$PSScriptRoot/bin/LightBulb.zip"

# Prepare directory
if (Test-Path $publishDirPath) {
    Remove-Item $publishDirPath -Recurse -Force
}
New-Item $publishDirPath -ItemType Directory -Force

# Build & publish
dotnet publish $projectDirPath -o $publishDirPath -c Release | Out-Host

$files = @()
$files += Get-Item -Path $licenseFilePath
$files += Get-ChildItem -Path $publishDirPath

# Pack into archive
$files | Compress-Archive -DestinationPath $artifactFilePath -Force