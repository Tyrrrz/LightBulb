param([string] $newVersion)

function Replace-TextInFile {
    param([string] $filePath, [string] $pattern, [string] $replacement)

    $content = [System.IO.File]::ReadAllText($filePath)
    $content = [System.Text.RegularExpressions.Regex]::Replace($content, $pattern, $replacement)
    [System.IO.File]::WriteAllText($filePath, $content)
}

Replace-TextInFile "$PSScriptRoot\LightBulb\Properties\AssemblyInfo.cs" '(?<=Assembly.*?Version\(")(.*?)(?="\)\])' $newVersion
Replace-TextInFile "$PSScriptRoot\Deploy\Choco\lightbulb.nuspec" '(?<=<version>)(.*?)(?=</version>)' $newVersion
Replace-TextInFile "$PSScriptRoot\Deploy\Choco\tools\chocolateyinstall.ps1" '(?<=download/)(.*?)(?=/)' $newVersion
Replace-TextInFile "$PSScriptRoot\Deploy\Installer\Installer.iss" '(?<=#define MyAppVersion ")(.*?)(?=")' $newVersion