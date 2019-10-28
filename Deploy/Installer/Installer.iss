#define MyAppName "LightBulb"
#define MyAppVersion "1.6.4.1"
#define MyAppPublisher "Alexey 'Tyrrrz' Golub"
#define MyAppURL "http://tyrrrz.me/Projects/LightBulb"

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
OutputBaseFilename=LightBulb-Installer

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\..\License.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\LightBulb\bin\netcoreapp3.0\Release\LightBulb.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\LightBulb\bin\netcoreapp3.0\Release\*.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\LightBulb\bin\netcoreapp3.0\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\LightBulb.exe"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{group}\{#MyAppName} on Github"; Filename: "https://github.com/Tyrrrz/LightBulb"
Name: "{commonstartup}\{#MyAppName}"; Filename: "{app}\LightBulb.exe"

[Registry]
; Change valid gamma range
Root: HKLM; Subkey: "Software\Microsoft\Windows NT\CurrentVersion\ICM"; ValueType: dword; ValueName: "GdiICMGammaRange"; ValueData: "256"

[Run]
Filename: "{app}\LightBulb.exe"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent