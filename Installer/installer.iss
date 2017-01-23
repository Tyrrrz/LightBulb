#define MyAppName "LightBulb"
#define MyAppVersion "1.4.1"
#define MyAppPublisher "Alexey 'Tyrrrz' Golub"
#define MyAppURL "http://www.tyrrrz.me/projects/?id=lb"

[Setup]
AppId={{381A89B9-C46F-4FAE-976C-75398A67AF52}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AlwaysRestart=yes
PrivilegesRequired=admin
AllowNoIcons=yes
DisableWelcomePage=yes
DisableProgramGroupPage=no
DisableReadyPage=yes
Compression=lzma
SolidCompression=yes
SetupIconFile=favicon.ico
LicenseFile=..\License.txt
OutputDir=Output\
OutputBaseFilename=LightBulb_Install

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\LightBulb\bin\Release\LightBulb.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\LightBulb\bin\Release\LightBulb.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\LightBulb\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\LightBulb.exe"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commonstartup}\{#MyAppName}"; Filename: "{app}\LightBulb.exe"

[Registry]
;Available colors hack
Root: HKLM32; Subkey: "Software\Microsoft\Windows NT\CurrentVersion\ICM"; ValueType: dword; ValueName: "GdiICMGammaRange"; ValueData: "256"; Flags: uninsdeletekey
Root: HKLM64; Subkey: "Software\Microsoft\Windows NT\CurrentVersion\ICM"; ValueType: dword; ValueName: "GdiICMGammaRange"; ValueData: "256"; Flags: uninsdeletekey

[Run]
Filename: "{app}\LightBulb.exe"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent