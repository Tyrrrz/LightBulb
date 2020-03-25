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
    $installerDownloadUrl = "https://download.visualstudio.microsoft.com/download/pr/5954c748-86a1-4823-9e7d-d35f6039317a/169e82cbf6fdeb678c5558c5d0a83834/windowsdesktop-runtime-3.1.3-win-x64.exe"
} else {
    $installerDownloadUrl = "https://download.visualstudio.microsoft.com/download/pr/7cd5c874-5d11-4e72-81f0-4a005d956708/0eb310169770c893407169fc3abaac4f/windowsdesktop-runtime-3.1.3-win-x86.exe"
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
