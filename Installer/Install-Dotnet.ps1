Write-Host "========================================================================"
Write-Host ""
Write-Host "This script will install .NET runtime which is required for LightBulb"
Write-Host ""
Write-Host "========================================================================"
Write-Host ""

# Ensure it's not already installed
Write-Host "Checking installed .NET runtimes..."

$runtimes = & dotnet --list-runtimes | Out-String -Stream
foreach ($runtime in $runtimes)
{
    if ($runtime -like "Microsoft.WindowsDesktop.App 3.1.*") {
        Write-Host "Already installed: $runtime"
        Write-Host "Exiting..."
        exit
    }
}

# Get .NET runtime installer URL
$installerDownloadUrl = ""
if ([Environment]::Is64BitOperatingSystem) {
    $installerDownloadUrl = "https://download.visualstudio.microsoft.com/download/pr/3240250e-6fe0-4258-af69-85abef6c00de/e01ee0af6c65d894f4a02bdf6705ec7b/windowsdesktop-runtime-3.1.2-win-x64.exe"
} else {
    $installerDownloadUrl = "https://download.visualstudio.microsoft.com/download/pr/b824906f-bd6e-4067-86a6-95c61620674d/cfcdab84a01cee94fdaa31271c3d4d47/windowsdesktop-runtime-3.1.2-win-x86.exe"
}

# Download .NET runtime to temp directory
Write-Host "Downloading installer from $installerDownloadUrl"
Write-Host "Please wait, this can take some time..."

$installerFilePath = [IO.Path]::ChangeExtension([IO.Path]::GetTempFileName(), "exe")

Import-Module BitsTransfer
Start-BitsTransfer $installerDownloadUrl $installerFilePath

# Run the installer
$process = Start-Process $installerFilePath -Wait
$process.WaitForExit()