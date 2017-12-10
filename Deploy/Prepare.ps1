# --- INSTALLER ---

# Run Inno Setup Compiler
& "c:\Program Files (x86)\Inno Setup 5\iscc" "$PSScriptRoot\Installer\Installer.iss"

# --- PORTABLE ---

# Get files
$files = Get-ChildItem -Path "$PSScriptRoot\..\LightBulb\bin\Release\*" -Include "*.exe", "*.dll", "*.config"
$files += Get-ChildItem -Path "$PSScriptRoot\Portable\*";

# Pack into archive
New-Item "$PSScriptRoot\Portable\bin" -ItemType Directory -Force
$files | Compress-Archive -DestinationPath "$PSScriptRoot\Portable\bin\LightBulb-Portable.zip" -Force