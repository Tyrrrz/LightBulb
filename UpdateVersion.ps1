param([string] $newVersion)

function Replace-TextInFile {
    param([string] $filePath, [string] $pattern, [string] $replacement)

    $content = [IO.File]::ReadAllText($filePath)
    $content = [Text.RegularExpressions.Regex]::Replace($content, $pattern, $replacement)
    [IO.File]::WriteAllText($filePath, $content, [Text.Encoding]::UTF8)
}

Replace-TextInFile "$PSScriptRoot\LightBulb\LightBulb.csproj" '(?<=<Version>)(.*?)(?=</Version>)' $newVersion
Replace-TextInFile "$PSScriptRoot\Deploy\Installer.iss" '(?<=#define AppVersion ")(.*?)(?=")' $newVersion