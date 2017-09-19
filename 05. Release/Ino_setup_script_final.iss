#include <idp.iss>

#define MyAppName "VNPT-CA Token Manager"
#define MyAppVersion "1.0.1"
#define MyAppPublisher "VNPT Software"
#define MyAppURL "http://www.vnptsoftware.vn/"
#define MyAppExeName "TokenManager.exe"

#define ReleaseDir "E:\PROJECTS\Token Manager\trunk\05. Release"
#define SourceReleaseDir "E:\PROJECTS\Token Manager\trunk\02. Source\TokenManager_net_4.0\TokenManager\bin\Release" 

[Setup]
AppId={{4EF3BF2E-DEA2-4317-B3A6-6C9E0FD1FE9A}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppPublisher}\{#MyAppName}
DisableProgramGroupPage=yes
; AppMutex=VNPT-CA-TokenManager

OutputDir={#ReleaseDir}\output
OutputBaseFilename=VnptCA_TokenManager_v{#MyAppVersion}
SetupIconFile={#ReleaseDir}\res\Logo-VNPT.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
UninstallDisplayIcon={app}\{#MyAppExeName}
; Show blue window behind setup window
; WindowVisible=yes
WizardSmallImageFile={#ReleaseDir}\setup_logo.bmp
DisableWelcomePage=no
WizardImageFile={#ReleaseDir}\Wellcome_back.bmp
SetupLogging=yes

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1


[Files]
Source: "{#SourceReleaseDir}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs 
Source: "{#ReleaseDir}\sys_res\*"; DestDir: "C\Windows\SysWOW64"; Check: IsWin64; Flags: recursesubdirs createallsubdirs 
Source: "{#ReleaseDir}\sys_res\*"; DestDir: "C:\Windows\System32\"; Flags: recursesubdirs createallsubdirs 
Source: "{#ReleaseDir}\NetFrameworkInstaller.exe"; DestDir: "{tmp}"; Flags: recursesubdirs createallsubdirs


[Icons]
Name: "{commonprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Languages]                          
Name: "vi"; MessagesFile: "compiler:Vietnamese.isl"; LicenseFile: "{#ReleaseDir}\res\License-vi.rtf"
Name: "en"; MessagesFile: "compiler:Default.isl"; LicenseFile: "{#ReleaseDir}\res\License-en.rtf"


[Registry]
; VNPT-CA-CTDT PKI-Token CSP
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Cryptography\Defaults\Provider\VNPT-CA-CTDT PKI-Token CSP"; ValueType: string; ValueName: "Image Path"; ValueData: "%SystemRoot%\System32\vnpt-ca_csp11_s.dll"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Cryptography\Defaults\Provider\VNPT-CA-CTDT PKI-Token CSP"; ValueType: binary; ValueName: "Signature"; ValueData: "66 1f fe c8 52 45 65 56 8b 3a a1 d7 c4 94 e3 66 c0 21 7c 93 5f 09 2f 8f 2d ae c2 f7 de 27 b5 53 38 21 88 fe 67 42 f2 73 9e a0 2d b1 f8 87 8f 7b 06 67 fd ff f5 d4 2a e9 8d 46 d0 56 ba 49 b6 d0 35 ea 76 6a 17 16 81 b1 2b 42 4a e9 18 09 79 9d 89 7e 3c c4 45 d4 d9 3a db 3f 2e 0a 30 fd c3 30 b2 d3 a2 45 e2 e8 6e df 25 2c bd e6 1a 46 b4 24 0b 68 03 09 dd ae 65 bc 80 d1 84 dc 64 15 50 13 00 00 00 00 00 00 00 00"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Cryptography\Defaults\Provider\VNPT-CA-CTDT PKI-Token CSP"; ValueType: dword ; ValueName: "Type"; ValueData: 1; Flags: uninsdeletekey
; VNPT-CA-CTDT v6-Token CSP
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Cryptography\Defaults\Provider\VNPT-CA-CTDT V6-Token CSP"; ValueType: string; ValueName: "Image Path"; ValueData: "%SystemRoot%\System32\vnptca_p11_v6_s.dll"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Cryptography\Defaults\Provider\VNPT-CA-CTDT V6-Token CSP"; ValueType: binary; ValueName: "Signature"; ValueData: "66 1f fe c8 52 45 65 56 8b 3a a1 d7 c4 94 e3 66 c0 21 7c 93 5f 09 2f 8f 2d ae c2 f7 de 27 b5 53 38 21 88 fe 67 42 f2 73 9e a0 2d b1 f8 87 8f 7b 06 67 fd ff f5 d4 2a e9 8d 46 d0 56 ba 49 b6 d0 35 ea 76 6a 17 16 81 b1 2b 42 4a e9 18 09 79 9d 89 7e 3c c4 45 d4 d9 3a db 3f 2e 0a 30 fd c3 30 b2 d3 a2 45 e2 e8 6e df 25 2c bd e6 1a 46 b4 24 0b 68 03 09 dd ae 65 bc 80 d1 84 dc 64 15 50 13 00 00 00 00 00 00 00 00"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Cryptography\Defaults\Provider\VNPT-CA-CTDT V6-Token CSP"; ValueType: dword ; ValueName: "Type"; ValueData: 1; Flags: uninsdeletekey
; Bkav-CA-CTDT Token CSP
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Cryptography\Defaults\Provider\Bkav-CA-CTDT Token CSP"; ValueType: string; ValueName: "Image Path"; ValueData: "%SystemRoot%\System32\BkavCA_s.dll"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Cryptography\Defaults\Provider\Bkav-CA-CTDT Token CSP"; ValueType: binary; ValueName: "Signature"; ValueData: "66 1f fe c8 52 45 65 56 8b 3a a1 d7 c4 94 e3 66 c0 21 7c 93 5f 09 2f 8f 2d ae c2 f7 de 27 b5 53 38 21 88 fe 67 42 f2 73 9e a0 2d b1 f8 87 8f 7b 06 67 fd ff f5 d4 2a e9 8d 46 d0 56 ba 49 b6 d0 35 ea 76 6a 17 16 81 b1 2b 42 4a e9 18 09 79 9d 89 7e 3c c4 45 d4 d9 3a db 3f 2e 0a 30 fd c3 30 b2 d3 a2 45 e2 e8 6e df 25 2c bd e6 1a 46 b4 24 0b 68 03 09 dd ae 65 bc 80 d1 84 dc 64 15 50 13 00 00 00 00 00 00 00 00"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Cryptography\Defaults\Provider\Bkav-CA-CTDT Token CSP"; ValueType: dword ; ValueName: "Type"; ValueData: 1; Flags: uninsdeletekey
; Start on system startup
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#MyAppName}"; ValueData: "{app}\{#MyAppExeName}"; Flags: uninsdeletevalue

Root: HKLM; Subkey: "SOFTWARE\VNPT Software\VNPT CA Token Manager"; Permissions: users-modify; ValueType: string; ValueName: "lang"; ValueData: "{language}"; Flags: uninsdeletevalue
Root: HKLM; Subkey: "SOFTWARE\VNPT Software\VNPT CA Token Manager"; Permissions: users-modify; ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"; Flags: uninsdeletevalue

[Run]    
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec

[Code]
 procedure CurUninstallStepChanged (CurUninstallStep: TUninstallStep);
 var
     mres : integer;
 begin
    case CurUninstallStep of                   
      usPostUninstall:
        begin
          mres := MsgBox(ExpandConstant('{cm:RemoveSetting}'), mbConfirmation, MB_YESNO or MB_DEFBUTTON2)
          if mres = IDYES then
            DelTree(ExpandConstant('{userappdata}\VNPT Software'), True, True, True);
       end;
   end;
end;

//Kiem tra phien ban Windows, yeu cau windows XP sp3 tro len
function InitializeSetup: Boolean;
var
  Version: TWindowsVersion;
begin
  GetWindowsVersionEx(Version);

  // Disallow installation on domain controllers
  if Version.ProductType = VER_NT_DOMAIN_CONTROLLER then
  begin
    SuppressibleMsgBox('This program cannot be installed on domain controllers.',
      mbCriticalError, MB_OK, IDOK);
    Result := False;
    Exit;
  end;

  // On Windows 2000, check for SP4
  if Version.NTPlatform and
     (Version.Major = 5) and
     (Version.Minor = 0) then
  begin
    SuppressibleMsgBox(CustomMessage('DisallowDomain'),
      mbCriticalError, MB_OK, IDOK);
    Result := False;
    Exit;
  end;

  // On Windows XP, check for SP3
  if Version.NTPlatform and
     (Version.Major = 5) and
     (Version.Minor = 1) and
     (Version.ServicePackMajor < 3) then
  begin
    SuppressibleMsgBox(CustomMessage('RequireMinVersion'),
      mbCriticalError, MB_OK, IDOK);
    Result := False;
    Exit;
  end;

  Result := True;
end;

function IsWindowXP(): Boolean;
var
  Version: TWindowsVersion;
begin
  Result := False;
  GetWindowsVersionEx(Version);
  if (Version.Major = 5) and
     (Version.Minor = 1) then 
     begin
        Result := True;
     end;
end;

// Check if .net 4.0 or higher version has been installed in system----------------------------
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1'          .NET Framework 1.1
//    'v2.0'          .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//    'v4.5.1'        .NET Framework 4.5.1
//    'v4.5.2'        .NET Framework 4.5.2
//    'v4.6'          .NET Framework 4.6
//    'v4.6.1'        .NET Framework 4.6.1
//    'v4.6.2'        .NET Framework 4.6.2
//    'v4.7'          .NET Framework 4.7
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key, versionKey: string;
    install, release, serviceCount, versionRelease: cardinal;
    success: boolean;
begin
    versionKey := version;
    versionRelease := 0;

    // .NET 1.1 and 2.0 embed release number in version key
    if version = 'v1.1' then begin
        versionKey := 'v1.1.4322';
    end else if version = 'v2.0' then begin
        versionKey := 'v2.0.50727';
    end

    // .NET 4.5 and newer install as update to .NET 4.0 Full
    else if Pos('v4.', version) = 1 then begin
        versionKey := 'v4\Full';
        case version of
          'v4.5':   versionRelease := 378389;
          'v4.5.1': versionRelease := 378675; // 378758 on Windows 8 and older
          'v4.5.2': versionRelease := 379893;
          'v4.6':   versionRelease := 393295; // 393297 on Windows 8.1 and older
          'v4.6.1': versionRelease := 394254; // 394271 before Win10 November Update
          'v4.6.2': versionRelease := 394802; // 394806 before Win10 Anniversary Update
          'v4.7':   versionRelease := 460798; // 460805 before Win10 Creators Update
        end;
    end;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + versionKey;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0 and newer use value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 and newer use additional value Release
    if versionRelease > 0 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= versionRelease);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;
//---------------------------------------------------------------------------

// Install .net framework 4.0------------------------------------------------
function InstallFramework(): Boolean;
var
  StatusText: string;
  ResultCode: Integer;
begin
  Result := True;
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := CustomMessage('InstallingDotNet');
  WizardForm.ProgressGauge.Style := npbstMarquee;
  try
    if not Exec(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'), '/passive /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      MsgBox('.NET installation failed with code: ' + IntToStr(ResultCode) + '.', mbError, MB_OK);
      Result := False;
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
    WizardForm.ProgressGauge.Style := npbstNormal;

    DeleteFile(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
  end;
end;
//----------------------------------------------------------------------------

procedure CurStepChanged(CurStep: TSetupStep);
begin
  case CurStep of
    ssPostInstall:
      begin
        if not IsDotNetDetected('v4.0', 0) then
        begin
          InstallFramework();
        end;
      end;
  end;
end;

[UninstallRun]
Filename: "{cmd}"; Parameters: "/C ""taskkill /im {#MyAppExeName} /f /t"
