#define MyAppName "LightBulb"
#define MyAppVersion "1.6.3"
#define MyAppPublisher "Alexey 'Tyrrrz' Golub"
#define MyAppURL "http://www.tyrrrz.me/projects/?id=lb"

[Setup]
AppId={{892F745F-A497-42ED-B503-8D74936D0BEB}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
AppMutex=LightBulb_Identity
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
ArchitecturesInstallIn64BitMode=x64
ChangesEnvironment=yes
AllowNoIcons=yes
DisableWelcomePage=yes
DisableProgramGroupPage=no
DisableReadyPage=yes
SetupIconFile=..\..\favicon.ico
UninstallDisplayIcon=..\..\favicon.ico
LicenseFile=..\..\License.txt
OutputDir=bin\
OutputBaseFilename=LightBulb_Install_{#MyAppVersion}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\..\License.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\LightBulb\bin\Release\LightBulb.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\LightBulb\bin\Release\LightBulb.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\LightBulb\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\LightBulb.exe"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{group}\{#MyAppName} on Github"; Filename: "https://github.com/Tyrrrz/LightBulb"
Name: "{commonstartup}\{#MyAppName}"; Filename: "{app}\LightBulb.exe"

[Registry]
; Change valid gamma range
Root: HKLM; Subkey: "Software\Microsoft\Windows NT\CurrentVersion\ICM"; ValueType: dword; ValueName: "GdiICMGammaRange"; ValueData: "256"; Flags: uninsdeletekey

[Run]
Filename: "{app}\LightBulb.exe"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent