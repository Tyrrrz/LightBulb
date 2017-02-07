$ErrorActionPreference = 'Stop';
$packageName= 'lightbulb'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/Tyrrrz/LightBulb/releases/download/1.6.2/LightBulb_Install_1.6.2.exe'
$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  fileType      = 'EXE_MSI_OR_MSU'
  url           = $url

  softwareName  = 'lightbulb*'

  checksum      = '016274df507716107a691833e7252cb932e4beb7ae3ba39c8b11ecbcb51a3fe3'
  checksumType  = 'sha256'
  
  silentArgs   = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
}
Install-ChocolateyPackage @packageArgs