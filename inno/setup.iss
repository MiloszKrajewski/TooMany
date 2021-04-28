; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "TooMany"
#define MyAppVersion "0.4.36-beta"
#define MyAppPublisher "Milosz Krajewski"
#define MyAppURL "https://github.com/MiloszKrajewski/TooMany"
#define MyAppExeName "2many.host.exe"

#define Root ".\.."
#define Staging ".\..\.output\app"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{6F728B61-3056-4461-935F-10EB07708EBD}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={commonpf}\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile={#Root}\LICENSE
OutputDir={#Staging}\..
OutputBaseFilename=TooMany-{#MyAppVersion}-net5-win-setup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
ChangesEnvironment=true

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "{#Staging}\2many.host.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#Staging}\2many.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#Staging}\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#Staging}\2many.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#Staging}\wwwroot\*"; DestDir: "{app}\wwwroot"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
;current user only
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "TooMany"; ValueData: "{app}\{#MyAppExeName}"; Tasks:AutoRun;

[Tasks]
Name: AutoRun; Description: &Start automatically when user logs in; Flags: unchecked
Name: AddToPath; Description: &Add application directory to your environmental path; Flags: unchecked

[Code]
const
  ModPathName = 'AddToPath';
  ModPathType = 'user';

function ModPathDir(): TArrayOfString;
begin
  setArrayLength(Result, 1);
  Result[0] := ExpandConstant('{app}');
end;

#include "modpath.pas"