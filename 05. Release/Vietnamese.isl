[LangOptions]
LanguageName=Ti<1EBF>ng Vi<1EC7>t
LanguageID=$0409
LanguageCodePage=65001

[Messages]

SetupAppTitle=Cài đặt 
SetupWindowTitle=Cài đặt - %1
UninstallAppTitle=Gỡ cài đặt
UninstallAppFullTitle=%1 Gỡ cài đặt

; *** Misc. common
InformationTitle=Thông tin
ConfirmTitle=Xác nhận
ErrorTitle=Error

; *** SetupLdr messages
SetupLdrStartupMessage=This will install %1. Do you wish to continue?
LdrCannotCreateTemp=Unable to create a temporary file. Setup aborted
LdrCannotExecTemp=Unable to execute file in the temporary directory. Setup aborted

; *** Startup error messages
LastErrorMessage=%1.%n%nError %2: %3
SetupFileMissing=The file %1 is missing from the installation directory. Please correct the problem or obtain a new copy of the program.
SetupFileCorrupt=The setup files are corrupted. Please obtain a new copy of the program.
SetupFileCorruptOrWrongVer=The setup files are corrupted, or are incompatible with this version of Setup. Please correct the problem or obtain a new copy of the program.
InvalidParameter=An invalid parameter was passed on the command line:%n%n%1
SetupAlreadyRunning=Setup is already running.
WindowsVersionNotSupported=This program does not support the version of Windows your computer is running.
WindowsServicePackRequired=This program requires %1 Service Pack %2 or later.
NotOnThisPlatform=This program will not run on %1.
OnlyOnThisPlatform=This program must be run on %1.
OnlyOnTheseArchitectures=This program can only be installed on versions of Windows designed for the following processor architectures:%n%n%1
MissingWOW64APIs=The version of Windows you are running does not include functionality required by Setup to perform a 64-bit installation. To correct this problem, please install Service Pack %1.
WinVersionTooLowError=This program requires %1 version %2 or later.
WinVersionTooHighError=This program cannot be installed on %1 version %2 or later.
AdminPrivilegesRequired=You must be logged in as an administrator when installing this program.
PowerUserPrivilegesRequired=You must be logged in as an administrator or as a member of the Power Users group when installing this program.
SetupAppRunningError=Setup has detected that %1 is currently running.%n%nPlease close all instances of it now, then click OK to continue, or Cancel to exit.
UninstallAppRunningError=Uninstall has detected that %1 is currently running.%n%nPlease close all instances of it now, then click OK to continue, or Cancel to exit.

; *** Misc. errors
ErrorCreatingDir=Setup was unable to create the directory "%1"
ErrorTooManyFilesInDir=Unable to create a file in the directory "%1" because it contains too many files

; *** Setup common messages
ExitSetupTitle=Hủy cài đặt 
ExitSetupMessage=Cài đặt phần mềm chưa hoàn thành. Nếu hủy bỏ, phần mềm sẽ không được cài đặt, mọi thay đổi sẽ được đưa về trạng thái ban đầu.%n%nHủy cài đặt?
AboutSetupMenuItem=&About Setup...
AboutSetupTitle=About Setup
AboutSetupMessage=%1 version %2%n%3%n%n%1 home page:%n%4
AboutSetupNote=
TranslatorNote=

; *** Buttons
ButtonBack=< &Quay lại
ButtonNext=&Tiếp tục >
ButtonInstall=&Cài đặt
ButtonOK=Đồng ý
ButtonCancel=Hủy bỏ
ButtonYes=&Đồng ý
ButtonYesToAll=Đồng ý
ButtonNo=&Hủy bỏ
ButtonNoToAll=Không đồng ý
ButtonFinish=&Kết thúc
ButtonBrowse=&Browse...
ButtonWizardBrowse=B&rowse...
ButtonNewFolder=&Make New Folder

; *** "Select Language" dialog messages
SelectLanguageTitle=Chọn ngôn ngữ cài đặt
SelectLanguageLabel=Chọn ngôn ngữ hiển thị khi cài đặt.%n(Select setup language)

; *** Common wizard text
ClickNext=Mời bạn bấm vào nút "Tiếp tục" để bắt đầu quá trình cài đặt.
BeveledLabel=
BrowseDialogTitle=Chọn thư mục cài đặt
BrowseDialogLabel=Chọn 1 thư mục trong danh sách dưới, sau đó chọn "Đồng ý"
NewFolderName=New Folder

; *** "Welcome" wizard page
WelcomeLabel1=Chào mừng!
WelcomeLabel2=Cảm ơn bạn đã sử dụng Chữ ký số của VNPT CA

; *** "Password" wizard page
WizardPassword=Password
PasswordLabel1=This installation is password protected.
PasswordLabel3=Please provide the password, then click Next to continue. Passwords are case-sensitive.
PasswordEditLabel=&Password:
IncorrectPassword=The password you entered is not correct. Please try again.

; *** "License Agreement" wizard page
WizardLicense=Điều khoản sử dụng
LicenseLabel=Vui lòng đọc các thông tin quan trọng dưới đây trước khi tiếp tục cài đặt phần mềm.
LicenseLabel3=
LicenseAccepted=Tôi đã đọc và chấp nhận các điều khoản trên
LicenseNotAccepted=Tôi không chấp nhận các điều khoản trên

; *** "Information" wizard pages
WizardInfoBefore=Information
InfoBeforeLabel=Please read the following important information before continuing.
InfoBeforeClickLabel=When you are ready to continue with Setup, click Next.
WizardInfoAfter=Information
InfoAfterLabel=Please read the following important information before continuing.
InfoAfterClickLabel=When you are ready to continue with Setup, click Next.

; *** "User Information" wizard page
WizardUserInfo=User Information
UserInfoDesc=Please enter your information.
UserInfoName=&User Name:
UserInfoOrg=&Organization:
UserInfoSerial=&Serial Number:
UserInfoNameRequired=You must enter a name.

; *** "Select Destination Location" wizard page
WizardSelectDir=Chọn thư mục cài đặt
SelectDirDesc=
SelectDirLabel3=[name] sẽ được cài đặt vào thư mục sau.
SelectDirBrowseLabel=Chọn "Tiếp tục" để cài đặt. Bạn có thể cài đặt [name] vào thư mục khác, chọn "Browse" để thay đổi.
DiskSpaceMBLabel=Cần ít nhất [mb] MB dung lượng trống trong hệ thống để cài đặt phần mềm.
CannotInstallToNetworkDrive=Không thể cài đặt trên thiết bị mạng.
CannotInstallToUNCPath=Không thể cài đặt vào UNC
InvalidPath=You must enter a full path with drive letter; for example:%n%nC:\APP%n%nor a UNC path in the form:%n%n\\server\share
InvalidDrive=The drive or UNC share you selected does not exist or is not accessible. Please select another.
DiskSpaceWarningTitle=Không đủ dung lượng trống
DiskSpaceWarning=Setup requires at least %1 KB of free space to install, but the selected drive only has %2 KB available.%n%nDo you want to continue anyway?
DirNameTooLong=The folder name or path is too long.
InvalidDirName=The folder name is not valid.
BadDirName32=Folder names cannot include any of the following characters:%n%n%1
DirExistsTitle=Folder Exists
DirExists=The folder:%n%n%1%n%nalready exists. Would you like to install to that folder anyway?
DirDoesntExistTitle=Folder Does Not Exist
DirDoesntExist=The folder:%n%n%1%n%ndoes not exist. Would you like the folder to be created?

; *** "Select Components" wizard page
WizardSelectComponents=Select Components
SelectComponentsDesc=Which components should be installed?
SelectComponentsLabel2=Select the components you want to install; clear the components you do not want to install. Click Next when you are ready to continue.
FullInstallation=Full installation
; if possible don't translate 'Compact' as 'Minimal' (I mean 'Minimal' in your language)
CompactInstallation=Compact installation
CustomInstallation=Custom installation
NoUninstallWarningTitle=Components Exist
NoUninstallWarning=Setup has detected that the following components are already installed on your computer:%n%n%1%n%nDeselecting these components will not uninstall them.%n%nWould you like to continue anyway?
ComponentSize1=%1 KB
ComponentSize2=%1 MB
ComponentsDiskSpaceMBLabel=Current selection requires at least [mb] MB of disk space.

; *** "Select Additional Tasks" wizard page
WizardSelectTasks=Chọn tác vụ cài đặt bổ sung
SelectTasksDesc=
SelectTasksLabel2=Chọn tác vụ mà bạn muốn thực hiện bổ sung trong quá trình cài đặt [name], sau đó nhấn chọn "Tiếp tục".

; *** "Select Start Menu Folder" wizard page
WizardSelectProgramGroup=Select Start Menu Folder
SelectStartMenuFolderDesc=Where should Setup place the program's shortcuts?
SelectStartMenuFolderLabel3=Setup will create the program's shortcuts in the following Start Menu folder.
SelectStartMenuFolderBrowseLabel=To continue, click Next. If you would like to select a different folder, click Browse.
MustEnterGroupName=You must enter a folder name.
GroupNameTooLong=The folder name or path is too long.
InvalidGroupName=The folder name is not valid.
BadGroupName=The folder name cannot include any of the following characters:%n%n%1
NoProgramGroupCheck2=&Don't create a Start Menu folder

; *** "Ready to Install" wizard page
WizardReady=Chuẩn bị cài đặt
ReadyLabel1=Sẵn sàng cài đặt [name] trên máy tính của bạn.
ReadyLabel2a=Nhấn chọn "Cài đặt" để bắt đầu cài đặt phần mềm, hoặc chọn "Quay lại" nếu bạn muốn thay đổi thiết lập.
ReadyLabel2b=Chọn "Cài đặt" để bắt đầu cài đặt phần mềm. 
ReadyMemoUserInfo=Thông tin người dùng:
ReadyMemoDir=Thư mục cài đặt:
ReadyMemoType=Loại cài đặt:
ReadyMemoComponents=Selected components:
ReadyMemoGroup=Thư mục trong menu Start:
ReadyMemoTasks=Tác vụ bổ sung:

; *** "Preparing to Install" wizard page
WizardPreparing=Đang chuẩn bị cài đặt
PreparingDesc=Đang chuẩn bị quá trình cài đặt [name] trên máy tính của bạn.
PreviousInstallNotCompleted=Quá trình cài đặt/hủy cài đặt phần mềm trước đó chưa hoàn thành. Bạn cần phải khởi động lại máy tính để hoàn thành quá trình đó.%n%nSau khi khởi động lại máy tính, vui lòng chạy bộ cài đặt để hoàn thành quá trình cài đặt [name].
CannotContinue=Không thể tiếp tục cài đặt phần mềm. Vui lòng chọn "Hủy bỏ" để hủy cài đặt.
ApplicationsFound=Phần mềm dưới đây đang sử dụng tệp cần được cập nhật trong quá trình cài đặt. Bạn cần cho phép quá trình cài đặt đóng các phần mềm này để tiếp tục.
ApplicationsFound2=Phần mềm dưới đây đang sử dụng tệp cần được cập nhật trong quá trình cài đặt. Bạn cần cho phép quá trình cài đặt đóng các phần mềm này để tiếp tục. Sau khi hoàn thành quá trình cài đặt, phần mềm sẽ được tự động khởi động lại.
CloseApplications=&Tự động đóng phần mềm
DontCloseApplications=&Không đóng các phần mềm này
ErrorCloseApplications=Không thể tự động đóng các phần mềm này. Vui lòng đóng các phần mềm này để tiếp tục quá trình cài đặt.

; *** "Installing" wizard page
WizardInstalling=Đang cài đặt
InstallingLabel=Vui lòng đợi, [name] đang được cài đặt trên máy tính của bạn.

; *** "Setup Completed" wizard page
FinishedHeadingLabel=Hoàn thành!
FinishedLabelNoIcons=Đã hoàn thành quá trình cài đặt [name] trên máy tính của bạn.
FinishedLabel=Đã hoàn thành quá trình cài đặt [name] trên máy tính của bạn.
ClickFinish=Chọn "Kết thúc" để hoàn thành.
FinishedRestartLabel=To complete the installation of [name], Setup must restart your computer. Would you like to restart now?
FinishedRestartMessage=To complete the installation of [name], Setup must restart your computer.%n%nWould you like to restart now?
ShowReadmeCheck=Yes, I would like to view the README file
YesRadio=&Yes, restart the computer now
NoRadio=&No, I will restart the computer later
; used for example as 'Run MyProg.exe'
RunEntryExec=Run %1
; used for example as 'View Readme.txt'
RunEntryShellExec=View %1

; *** "Setup Needs the Next Disk" stuff
ChangeDiskTitle=Setup Needs the Next Disk
SelectDiskLabel2=Please insert Disk %1 and click OK.%n%nIf the files on this disk can be found in a folder other than the one displayed below, enter the correct path or click Browse.
PathLabel=&Path:
FileNotInDir2=The file "%1" could not be located in "%2". Please insert the correct disk or select another folder.
SelectDirectoryLabel=Please specify the location of the next disk.

; *** Installation phase messages
SetupAborted=Cài đặt không thành công.%n%nVui lòng xử lý các vấn đề trên và chạy lại tệp cài đặt.
EntryAbortRetryIgnore=Chọn Retry để thử lại, Ignore để bỏ qua, hoặc Cancel để hủy cài đặt.

; *** Installation status messages
StatusClosingApplications=Đang đóng phần mềm...
StatusCreateDirs=Đang tạo thư mục cài đặt...
StatusExtractFiles=Đang giải nén...
StatusCreateIcons=Đang tạo phím tắt...
StatusCreateIniEntries=Creating INI entries...
StatusCreateRegistryEntries=Đang tạo khóa trong Registry...
StatusRegisterFiles=Đang ghi dữ liệu vào Registry...
StatusSavingUninstall=Đang lưu thông tin hủy cài đặt...
StatusRunProgram=Đang hoàn thành cài đặt...
StatusRestartingApplications=Khởi động lại phần mềm...
StatusRollback=Đang hủy bỏ các thay đổi...

; *** Misc. errors
ErrorInternal2=Internal error: %1
ErrorFunctionFailedNoCode=%1 failed
ErrorFunctionFailed=%1 failed; code %2
ErrorFunctionFailedWithMessage=%1 failed; code %2.%n%3
ErrorExecutingProgram=Unable to execute file:%n%1

; *** Registry errors
ErrorRegOpenKey=Error opening registry key:%n%1\%2
ErrorRegCreateKey=Error creating registry key:%n%1\%2
ErrorRegWriteKey=Error writing to registry key:%n%1\%2

; *** INI errors
ErrorIniEntry=Error creating INI entry in file "%1".

; *** File copying errors
FileAbortRetryIgnore=Click Retry to try again, Ignore to skip this file (not recommended), or Abort to cancel installation.
FileAbortRetryIgnore2=Click Retry to try again, Ignore to proceed anyway (not recommended), or Abort to cancel installation.
SourceIsCorrupted=The source file is corrupted
SourceDoesntExist=The source file "%1" does not exist
ExistingFileReadOnly=The existing file is marked as read-only.%n%nClick Retry to remove the read-only attribute and try again, Ignore to skip this file, or Abort to cancel installation.
ErrorReadingExistingDest=An error occurred while trying to read the existing file:
FileExists=The file already exists.%n%nWould you like Setup to overwrite it?
ExistingFileNewer=The existing file is newer than the one Setup is trying to install. It is recommended that you keep the existing file.%n%nDo you want to keep the existing file?
ErrorChangingAttr=An error occurred while trying to change the attributes of the existing file:
ErrorCreatingTemp=An error occurred while trying to create a file in the destination directory:
ErrorReadingSource=An error occurred while trying to read the source file:
ErrorCopying=An error occurred while trying to copy a file:
ErrorReplacingExistingFile=An error occurred while trying to replace the existing file:
ErrorRestartReplace=RestartReplace failed:
ErrorRenamingTemp=An error occurred while trying to rename a file in the destination directory:
ErrorRegisterServer=Unable to register the DLL/OCX: %1
ErrorRegSvr32Failed=RegSvr32 failed with exit code %1
ErrorRegisterTypeLib=Unable to register the type library: %1

; *** Post-installation errors
ErrorOpeningReadme=An error occurred while trying to open the README file.
ErrorRestartingComputer=Setup was unable to restart the computer. Please do this manually.

; *** Uninstaller messages
UninstallNotFound=Không tìm thấy "%1". Không thể tiến hành gỡ cài đặt.
UninstallOpenError=Không thể khởi động "%1"
UninstallUnsupportedVer=The uninstall log file "%1" is in a format not recognized by this version of the uninstaller. Cannot uninstall
UninstallUnknownEntry=An unknown entry (%1) was encountered in the uninstall log
ConfirmUninstall=Bạn có chắc chắn muốn gỡ cài đặt (%1) và tất cả các thiết lập?
UninstallOnlyOnWin64=This installation can only be uninstalled on 64-bit Windows.
OnlyAdminCanUninstall=This installation can only be uninstalled by a user with administrative privileges.
UninstallStatusLabel=%1 đang được gỡ bỏ khỏi máy tính của bạn.
UninstalledAll=%1 đã được gỡ bỏ khỏi máy tính của bạn.
UninstalledMost=%1 đã được gỡ bỏ.%n%nMột số thành phần không thể xóa. Bạn có thể xóa chúng bằng tay.
UninstalledAndNeedsRestart=To complete the uninstallation of %1, your computer must be restarted.%n%nWould you like to restart now?
UninstallDataCorrupted="%1" file is corrupted. Cannot uninstall

; *** Uninstallation phase messages
ConfirmDeleteSharedFileTitle=Remove Shared File?
ConfirmDeleteSharedFile2=The system indicates that the following shared file is no longer in use by any programs. Would you like for Uninstall to remove this shared file?%n%nIf any programs are still using this file and it is removed, those programs may not function properly. If you are unsure, choose No. Leaving the file on your system will not cause any harm.
SharedFileNameLabel=File name:
SharedFileLocationLabel=Location:
WizardUninstalling=Gỡ cài đặt
StatusUninstalling=Đang gỡ cài đặt %1...

; *** Shutdown block reasons
ShutdownBlockReasonInstallingApp=Installing %1.
ShutdownBlockReasonUninstallingApp=Uninstalling %1.

; The custom messages below aren't used by Setup itself, but if you make
; use of them in your scripts, you'll want to translate them.

[CustomMessages]

NameAndVersion=%1 phiên bản %2
AdditionalIcons=Thêm phím tắt:
CreateDesktopIcon=Tạo một phím tắt trên &màn hình nền
CreateQuickLaunchIcon=Bổ sung vào danh mục "&Quick Launch"
ProgramOnTheWeb=%1 on the Web
UninstallProgram=Gỡ cài đặt %1
LaunchProgram=Khởi động phần mềm %1
AssocFileExtension=&Associate %1 with the %2 file extension
AssocingFileExtension=Associating %1 with the %2 file extension...
AutoStartProgramGroupDescription=Startup:
AutoStartProgram=Automatically start %1
AddonHostProgramNotFound=%1 could not be located in the folder you selected.%n%nDo you want to continue anyway?

; TuanBS custom messages
RemoveSetting=Bạn có muốn gỡ bỏ tất cả tùy chọn của phần mềm?
DisallowDomain=VNPT CA Token Manager không thể cài đặt trên phiên bản Windows này.
RequireMinVersion=VNPT CA Token Manager yêu cầu Windows XP sp3 hoặc cao hơn.
InstallingDotNet=Đang cài đặt .NET Framework 4.0. Quá trình này có thể mất một vài phút ...
