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
ArchitecturesInstallIn64BitMode=x64
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
Source: "bin\build\*"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\LightBulb.exe"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{group}\{#AppName} on Github"; Filename: "https://github.com/Tyrrrz/LightBulb"

[Run]
Filename: "{app}\LightBulb.exe"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent