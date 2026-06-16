# Restablece el proxy de Windows (WinINET) a "automático", limpiando cualquier
# proxy local (127.0.0.1:<puerto>) que haya quedado seteado y que corta internet.
#
# - Desactiva el proxy manual (ProxyEnable = 0) y borra ProxyServer / AutoConfigURL.
# - Activa "Detectar la configuración automáticamente" (WPAD).
# - Notifica a Windows para aplicar los cambios sin reiniciar.
#
# Solo modifica HKCU (tu usuario): NO requiere permisos de administrador.
#
# Uso:  pwsh tools/reset-proxy.ps1
# Escritorio (sin depender del repo): pwsh tools/build-reset-proxy-desktop.ps1

$ErrorActionPreference = "Stop"
$is = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Internet Settings"

Write-Host "Estado anterior:" -ForegroundColor Yellow
$prev = Get-ItemProperty -Path $is
Write-Host ("  ProxyEnable = {0}" -f $prev.ProxyEnable)
Write-Host ("  ProxyServer = {0}" -f $prev.ProxyServer)

# 1) Apagar proxy manual y limpiar valores residuales.
Set-ItemProperty -Path $is -Name ProxyEnable -Value 0 -Type DWord
Set-ItemProperty -Path $is -Name ProxyServer -Value "" -Type String
Remove-ItemProperty -Path $is -Name AutoConfigURL -ErrorAction SilentlyContinue

# 2) Activar "Detectar configuración automáticamente".
Set-ItemProperty -Path $is -Name AutoDetect -Value 1 -Type DWord

# 3) Reflejar el cambio en el blob de conexiones (bit 0x08 = autodetect, sin proxy manual).
$conn = Join-Path $is "Connections"
foreach ($name in @("DefaultConnectionSettings", "SavedLegacySettings")) {
    try {
        $val = (Get-ItemProperty -Path $conn -Name $name -ErrorAction Stop).$name
        if ($val -and $val.Length -ge 12) {
            $val[8] = 0x09   # 0x01 (base) + 0x08 (autodetect)
            $val[9] = 0; $val[10] = 0; $val[11] = 0
            Set-ItemProperty -Path $conn -Name $name -Value $val -Type Binary
        }
    } catch { }
}

# 4) Notificar a WinINET para que tome los cambios sin reiniciar.
$sig = @'
[DllImport("wininet.dll", SetLastError = true)]
public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
'@
$wininet = Add-Type -MemberDefinition $sig -Name WinInet -Namespace Net -PassThru
$null = $wininet::InternetSetOption([IntPtr]::Zero, 39, [IntPtr]::Zero, 0)  # INTERNET_OPTION_SETTINGS_CHANGED
$null = $wininet::InternetSetOption([IntPtr]::Zero, 37, [IntPtr]::Zero, 0)  # INTERNET_OPTION_REFRESH

Write-Host "`nProxy restablecido a automatico (sin proxy local)." -ForegroundColor Green
$now = Get-ItemProperty -Path $is
Write-Host ("  ProxyEnable = {0}" -f $now.ProxyEnable)
Write-Host ("  ProxyServer = '{0}'" -f $now.ProxyServer)
Write-Host ("  AutoDetect  = {0}" -f $now.AutoDetect)
