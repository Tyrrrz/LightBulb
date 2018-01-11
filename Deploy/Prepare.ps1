# --- INSTALLER ---

# Run Inno Setup Compiler
& "c:\Program Files (x86)\Inno Setup 5\iscc" "$PSScriptRoot\Installer\Installer.iss"

# --- PORTABLE ---

# Get files
$files = @()
$files += Get-ChildItem -Path "$PSScriptRoot\..\LightBulb\bin\Release\*" -Include "*.exe", "*.dll", "*.config"
$files += Get-ChildItem -Path "$PSScriptRoot\Portable\*";

# Pack into archive
New-Item "$PSScriptRoot\Portable\bin" -ItemType Directory -Force
$files | Compress-Archive -DestinationPath "$PSScriptRoot\Portable\bin\LightBulb-Portable.zip" -Force

# --- CHOCOLATEY ---

# Create package
New-Item "$PSScriptRoot\Choco\bin\" -ItemType Directory -Force
choco pack $PSScriptRoot\Choco\lightbulb.nuspec --out $PSScriptRoot\Choco\bin\