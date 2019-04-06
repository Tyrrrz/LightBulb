$ErrorActionPreference = 'Stop';
$packageArgs = @{
  packageName = $env:ChocolateyPackageName
  softwareName = 'lightbulb*'
  unzipLocation = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
  url = 'https://github.com/Tyrrrz/LightBulb/releases/download/1.6.4.1/LightBulb-Installer.exe'
  fileType = 'exe'
  silentArgs = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
}
Install-ChocolateyPackage @packageArgs