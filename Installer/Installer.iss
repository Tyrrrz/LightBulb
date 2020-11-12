#define AppName "LightBulb"
#define AppVersion "2.3"

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
Source: ".installed"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\License.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "Source\*"; DestDir: "{app}"; Flags: ignoreversion
Source: "Install-Dotnet.ps1"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\LightBulb.exe"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{group}\{#AppName} on Github"; Filename: "https://github.com/Tyrrrz/LightBulb"

[Registry]
Root: HKLM; Subkey: "Software\Microsoft\Windows NT\CurrentVersion\ICM"; ValueType: dword; ValueName: "GdiICMGammaRange"; ValueData: "256"

[Run]
Filename: "{app}\LightBulb.exe"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Name: "{userappdata}\LightBulb"; Type: filesandordirs

[Code]
procedure InstallDotnet();
var
  ErrorCode: Integer;
begin
  ShellExec('', 'powershell',
    '-NoProfile -ExecutionPolicy Unrestricted -File "' + ExpandConstant('{app}') + '\Install-Dotnet.ps1"',
    '', SW_SHOW, ewWaitUntilTerminated, ErrorCode);  
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then InstallDotnet();
end;