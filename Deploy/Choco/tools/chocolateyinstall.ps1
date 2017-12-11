$ErrorActionPreference = 'Stop';
$packageName= 'lightbulb'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/Tyrrrz/LightBulb/releases/download/1.6.3.5/LightBulb-Installer.exe'
$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  fileType      = 'exe'
  url           = $url

  softwareName  = 'lightbulb*'

  checksum      = '4ae5c09e730368b93249053d711b1c76a84d9d9d8e0be3ee72b7b3b415c1bc5e'
  checksumType  = 'sha256'
  
  silentArgs   = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
}
Install-ChocolateyPackage @packageArgs