$ErrorActionPreference = 'Stop';
$packageName= 'lightbulb'
$toolsDir   = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url        = 'https://github.com/Tyrrrz/LightBulb/releases/download/1.6.3.2/LightBulb_Install_1.6.3.2.exe'
$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  fileType      = 'exe'
  url           = $url

  softwareName  = 'lightbulb*'

  checksum      = 'ca5910476df660a0cbbd7fba6bcdf27f8f6815f791ea33cbecba74d285696db3'
  checksumType  = 'sha256'
  
  silentArgs   = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
}
Install-ChocolateyPackage @packageArgs