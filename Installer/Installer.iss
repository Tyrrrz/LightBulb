#define AppName "LightBulb"
#define AppVersion "2.0"

[Setup]
AppId={{892F745F-A497-42ED-B503-8D74936D0BEB}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher="Alexey 'Tyrrrz' Golub"
AppPublisherURL="https://github.com/Tyrrrz/LightBulb"
AppSupportURL="https://github.com/Tyrrrz/LightBulb/issues"
AppUpdatesURL="https://github.com/Tyrrrz/LightBulb/releases"
AppMutex=LightBulb_Identity
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
AllowNoIcons=yes
DisableWelcomePage=yes
DisableProgramGroupPage=no
DisableReadyPage=yes
SetupIconFile=..\favicon.ico
UninstallDisplayIcon=..\favicon.ico
LicenseFile=..\License.txt
OutputDir=bin\
OutputBaseFilename=LightBulb-Installer

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\License.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "Source\*"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\LightBulb.exe"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{group}\{#AppName} on Github"; Filename: "https://github.com/Tyrrrz/LightBulb"

[Registry]
Root: HKLM; Subkey: "Software\Microsoft\Windows NT\CurrentVersion\ICM"; ValueType: dword; ValueName: "GdiICMGammaRange"; ValueData: "256"

[Run]
Filename: "{app}\LightBulb.exe"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{autoappdata}\LightBulb"

[Code]
procedure InstallDotnetCore();
var
  Architecture: String;
  ErrorCode: Integer;
begin
  Architecture := '<auto>';
  if ProcessorArchitecture = paX64 then Architecture := 'x64';
  if ProcessorArchitecture = paARM64 then Architecture := 'arm64';

  ShellExec('', 'powershell',
    '-NoProfile -ExecutionPolicy unrestricted -Command "Write-Host ''Installing .NET Core runtime...''; [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; &([scriptblock]::Create((Invoke-WebRequest -UseBasicParsing ''https://dot.net/v1/dotnet-install.ps1''))) -Channel Current -Runtime dotnet -Architecture ' + Architecture + '"',
    '', SW_SHOW, ewWaitUntilTerminated, ErrorCode);
  ShellExec('', 'powershell',
    '-NoProfile -ExecutionPolicy unrestricted -Command "Write-Host ''Installing .NET Core Desktop runtime...''; [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; &([scriptblock]::Create((Invoke-WebRequest -UseBasicParsing ''https://dot.net/v1/dotnet-install.ps1''))) -Channel Current -Runtime windowsdesktop -Architecture ' + Architecture + '"',
    '', SW_SHOW, ewWaitUntilTerminated, ErrorCode);
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageId = wpInstalling then InstallDotnetCore();
end;