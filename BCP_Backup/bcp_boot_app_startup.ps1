$LNKFILE = "$HOME\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\BCP起動ソフト.lnk"
$cpath = (Get-Location).Path
$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$LNKFILE")
$Shortcut.TargetPath = "$cpath" + "\boot\BCP_Backup_Boot.exe"
$Shortcut.IconLocation = "$cpath" + "\boot\BCP_Backup_Boot.exe,0"
$Shortcut.WorkingDirectory = "$cpath" + "\boot"
$Shortcut.Save()

$bytes = [System.IO.File]::ReadAllBytes("$LNKFILE")
$bytes[0x15] = $bytes[0x15] -bor 0x20 
[System.IO.File]::WriteAllBytes("$LNKFILE", $bytes)

