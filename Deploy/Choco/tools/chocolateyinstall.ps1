$ErrorActionPreference = 'Stop';
$packageName= 'lightbulb'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/Tyrrrz/LightBulb/releases/download/1.6.3/LightBulb_Install_1.6.3.exe'
$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  fileType      = 'EXE_MSI_OR_MSU'
  url           = $url

  softwareName  = 'lightbulb*'

  checksum      = '847973f8cada2533c3c5314393a03c03fc1a694dcfa13a9c94e0d5fe7a706823'
  checksumType  = 'sha256'
  
  silentArgs   = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
}
Install-ChocolateyPackage @packageArgs