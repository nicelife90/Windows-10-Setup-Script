# Отключить службу "Функциональные возможности для подключенных пользователей и телеметрия"
# Тумблер находится слева
if (Get-Service -Name DiagTrack | Where-Object -FilterScript {$_.Status -eq "Running" -or $_.StartType -ne "Disabled"})
{
	"Scripts\1. Privacy\1. Turn off Connected User Experiences and Telemetry service.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if (Get-Service -Name DiagTrack | Where-Object -FilterScript {$_.Status -ne "Running" -or $_.StartType -eq "Disabled"})
{
	Get-Service -Name DiagTrack | Set-Service -StartupType Automatic
	Get-Service -Name DiagTrack | Start-Service
}

# Отключить cлужбы для отдельных пользователей
# Тумблер находится слева
$services = @(
	"PimIndexMaintenanceSvc_*"
	"UnistoreSvc_*"
	"UserDataSvc_*"
)
if (Get-Service -Name $services | Where-Object -FilterScript {$_.Status -eq "Running"})
{
	$Running = $true
}
if (((Get-ItemPropertyValue -Path HKLM:\System\CurrentControlSet\Services\$services -Name Start) -ne 4) -or ((Get-ItemPropertyValue -Path HKLM:\System\CurrentControlSet\Services\$services -Name UserServiceFlags) -ne 4))
{
	$Values = $false
}
if ($Stopped -or (-not $Values))
{
	"Scripts\1. Privacy\2. Turn off per-user services.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа. Требуется перезагрузка
$services = @(
	"PimIndexMaintenanceSvc_*"
	"UnistoreSvc_*"
	"UserDataSvc_*"
)
if (Get-Service -Name $services | Where-Object -FilterScript {$_.Status -ne "Running"})
{
	$Running = $false
}
if (((Get-ItemPropertyValue -Path HKLM:\System\CurrentControlSet\Services\$services -Name Start) -eq 4) -or ((Get-ItemPropertyValue -Path HKLM:\System\CurrentControlSet\Services\$services -Name UserServiceFlags) -eq 4))
{
	$Values = $true
}
if ($Stopped -or $Values)
{
	New-ItemProperty -Path HKLM:\System\CurrentControlSet\Services\PimIndexMaintenanceSvc -Name Start -PropertyType DWord -Value 4 -Force
	New-ItemProperty -Path HKLM:\System\CurrentControlSet\Services\PimIndexMaintenanceSvc -Name UserServiceFlags -PropertyType DWord -Value 4 -Force
	New-ItemProperty -Path HKLM:\System\CurrentControlSet\Services\UnistoreSvc -Name Start -PropertyType DWord -Value 4 -Force
	New-ItemProperty -Path HKLM:\System\CurrentControlSet\Services\UnistoreSvc -Name UserServiceFlags -PropertyType DWord -Value 4 -Force
	New-ItemProperty -Path HKLM:\System\CurrentControlSet\Services\UserDataSvc -Name Start -PropertyType DWord -Value 4 -Force
	New-ItemProperty -Path HKLM:\System\CurrentControlSet\Services\UserDataSvc -Name UserServiceFlags -PropertyType DWord -Value 4 -Force
}
<# Значение тумблера на "выкл" ###
if (Get-EtwTraceSession -Name DiagLog)
{}
if (Get-EtwTraceSession -Name Diagtrack-Listener)
{}
# Stop and remove Diagtrack ETL trace session under ###
# Отключить сборщик SQMLogger при следующем запуске ПК
Get-EtwTraceSession -Name DiagLog | Stop-EtwTraceSession
Get-EtwTraceSession -Name Diagtrack-Listener | Stop-EtwTraceSession
# Вернуть по умолчанию
Start-EtwTraceSession -Name DiagLog -LogFileMode 276824448
Start-EtwTraceSession -Name Diagtrack-Listener -LogFileMode 142606609

# Значение тумблера на "выкл"
$Loggers = @(
	"DiagLog"
	"SQMLogger"
)
if ((Get-AutologgerConfig -Name $Loggers).Start -eq 1)
{
	$Loggers[0]
}
# Turn off the SQMLogger session at the next computer restart ###
# Отключить сборщик SQMLogger при следующем запуске ПК
Update-AutologgerConfig -Name DiagLog -Start 0
Update-AutologgerConfig -Name SQMLogger -Start 0
# Вернуть по умолчанию
Update-AutologgerConfig -Name DiagLog -Start 1
Update-AutologgerConfig -Name SQMLogger -Start 1
#>

if ((Get-WindowsEdition -Online).Edition -eq "Enterprise" -or (Get-WindowsEdition -Online).Edition -eq "Education")
{
	# Тумблер находится слева
	if ((Get-ItemPropertyValue -Path HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection -Name AllowTelemetry) -ne 0)
	{
		"Scripts\1. Privacy\5. Set the operating system diagnostic data level.ps1"
	}
	# Вернуть по умолчанию. Если любые другие редакции
	# Тумблер находится справа
	else
	{
		New-ItemProperty -Path HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection -Name AllowTelemetry -PropertyType DWord -Value 3 -Force
	}
}
if ((Get-WindowsEdition -Online).Edition -ne "Enterprise" -or (Get-WindowsEdition -Online).Edition -ne "Education")
{
	# Тумблер находится слева
	if ((Get-ItemPropertyValue -Path HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection -Name AllowTelemetry) -ne 1)
	{
		"Scripts\1. Privacy\5. Set the operating system diagnostic data level.ps1"
	}
	# Вернуть по умолчанию. Если любые другие редакции
	# Тумблер находится справа
	else
	{
		New-ItemProperty -Path HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection -Name AllowTelemetry -PropertyType DWord -Value 3 -Force
	}
}

# Отключить отчеты об ошибках Windows
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path "HKCU:\Software\Microsoft\Windows\Windows Error Reporting" -Name Disabled) -ne 1)
{
	"Scripts\1. Privacy\6. Turn off Windows Error Reporting.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path "HKCU:\Software\Microsoft\Windows\Windows Error Reporting" -Name Disabled) -eq 1)
{
	Remove-ItemProperty -Path HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection -Name AllowTelemetry -Force
}

# Изменить частоту формирования отзывов на "Никогда"
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Siuf\Rules -Name NumberOfSIUFInPeriod) -ne 0)
{
	"Scripts\1. Privacy\7. Change Windows Feedback frequency to Never.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Siuf\Rules -Name NumberOfSIUFInPeriod) -eq 1)
{
	Remove-Item -Path HKCU:\Software\Microsoft\Siuf\Rules -Force
}

# Отключить задачи диагностического отслеживания
$tasks = @(
	"ProgramDataUpdater"
	"Microsoft Compatibility Appraiser"
	"Microsoft-Windows-DiskDiagnosticDataCollector"
	"TempSignedLicenseExchange"
	"MapsToastTask"
	"DmClient"
	"FODCleanupTask"
	"DmClientOnScenarioDownload"
	"BgTaskRegistrationMaintenanceTask"
	"File History (maintenance mode)"
	"WinSAT"
	"UsbCeip"
	"Consolidator"
	"Proxy"
	"MNO Metadata Parser"
	"NetworkStateChangeTask"
	"GatherNetworkInfo"
	"XblGameSaveTask"
	"EnableLicenseAcquisition"
	"QueueReporting"
	"FamilySafetyMonitor"
	"FamilySafetyRefreshTask"
)
# Если устройство не является ноутбуком
if ((Get-CimInstance -ClassName Win32_ComputerSystem).PCSystemType -ne 2)
{
	# HelloFace
	$tasks += "FODCleanupTask"
}
foreach ($task in $tasks)
{
	# Тумблер находится слева
	if (((Get-ScheduledTask -TaskName $task).State) -eq "Ready")
	{
		"Scripts\1. Privacy\8. Turn off diagnostics tracking scheduled tasks.ps1"
	}
}
$tasks = @(
	"ProgramDataUpdater"
	"Microsoft Compatibility Appraiser"
	"Microsoft-Windows-DiskDiagnosticDataCollector"
	"TempSignedLicenseExchange"
	"MapsToastTask"
	"DmClient"
	"FODCleanupTask"
	"DmClientOnScenarioDownload"
	"BgTaskRegistrationMaintenanceTask"
	"File History (maintenance mode)"
	"WinSAT"
	"UsbCeip"
	"Consolidator"
	"Proxy"
	"MNO Metadata Parser"
	"NetworkStateChangeTask"
	"GatherNetworkInfo"
	"XblGameSaveTask"
	"EnableLicenseAcquisition"
	"QueueReporting"
	"FamilySafetyMonitor"
	"FamilySafetyRefreshTask"
)
# Если устройство не является ноутбуком
if ((Get-CimInstance -ClassName Win32_ComputerSystem).PCSystemType -ne 2)
{
	# HelloFace
	$tasks += "FODCleanupTask"
}
foreach ($task in $tasks)
{
	# Вернуть по умолчанию
	# Тумблер находится справа
	if (((Get-ScheduledTask -TaskName $task).State) -eq "Disabled")
	{
		Get-ScheduledTask -TaskName $task | Enable-ScheduledTask
	}
}

# Не использовать данные для входа для автоматического завершения настройки устройства и открытия приложений после перезапуска или обновления
# Тумблер находится слева
$SID = (Get-CimInstance -ClassName Win32_UserAccount | Where-Object -FilterScript {$_.Name -eq $env:USERNAME}).SID
if ((Get-ItemPropertyValue -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\UserARSO\$sid" -Name OptOut) -ne 1)
{
	"Scripts\1. Privacy\9. Do not use sign-in info to automatically finish setting up device and reopen apps after an update or restart.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
$SID = (Get-CimInstance -ClassName Win32_UserAccount | Where-Object -FilterScript {$_.Name -eq $env:USERNAME}).SID
if ((Get-ItemPropertyValue -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\UserARSO\$sid" -Name OptOut) -eq 1)
{
	Remove-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\UserARSO\$sid" -Name OptOut -Force
}

# Не позволять веб-сайтам предоставлять местную информацию за счет доступа к списку языков
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path "HKCU:\Control Panel\International\User Profile" -Name HttpAcceptLanguageOptOut) -ne 1)
{
	"Scripts\1. Privacy\10. Do not let websites provide locally relevant content by accessing language list.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path "HKCU:\Control Panel\International\User Profile" -Name HttpAcceptLanguageOptOut) -eq 1)
{
	Remove-ItemProperty -Path "HKCU:\Control Panel\International\User Profile" -Name HttpAcceptLanguageOptOut -Force
}

# Не разрешать приложениям использовать идентификатор рекламы
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo -Name Enabled) -ne 0)
{
	"Scripts\1. Privacy\11. Do not allow apps to use advertising ID.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo -Name Enabled) -eq 0)
{
	Remove-ItemProperty -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo -Name Enabled -Force
}

# Не разрешать приложениям на других устройствах запускать приложения и отправлять сообщения на этом устройстве и наоборот
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\CDP -Name RomeSdkChannelUserAuthzPolicy) -ne 0)
{
	"Scripts\1. Privacy\12. Do not let apps on other devices open and message apps on this device, and vice versa.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\CDP -Name RomeSdkChannelUserAuthzPolicy) -eq 0)
{
	Remove-ItemProperty -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\CDP -Name RomeSdkChannelUserAuthzPolicy -Force
}

# Не показывать экран приветствия Windows после обновлений и иногда при входе, чтобы сообщить о новых функциях и предложениях
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-310093Enabled) -ne 0)
{
	"Scripts\1. Privacy\13. Do not show the Windows welcome experiences after updates.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-310093Enabled) -eq 0)
{
	Remove-ItemProperty -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-310093Enabled -Force
}

# Получать советы, подсказки и рекомендации при использованию Windows
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-338389Enabled) -ne 1)
{
	"Scripts\1. Privacy\14. Get tip, trick, and suggestions as you use Windows.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-338389Enabled) -eq 1)
{
	Remove-ItemProperty -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-338389Enabled -Force
}

# Не показывать рекомендуемое содержимое в приложении "Параметры"
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-338393Enabled) -ne 0)
{
	$SubscribedContent338393Enabled = $false
}
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-353694Enabled) -ne 0)
{
	$SubscribedContent353694Enabled = $false
}
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-353696Enabled) -ne 0)
{
	$SubscribedContent353696Enabled = $false
}
if (-not ($SubscribedContent338393Enabled -and $SubscribedContent353694Enabled -and $SubscribedContent353696Enabled))
{
	"Scripts\1. Privacy\15. Do not show suggested content in the Settings app.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-338393Enabled) -eq 0)
{
	$SubscribedContent338393Enabled = $true
}
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-353694Enabled) -eq 0)
{
	$SubscribedContent353694Enabled = $true
}
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-353696Enabled) -ne 0)
{
	$SubscribedContent353696Enabled = $true
}
if ($SubscribedContent338393Enabled -and $SubscribedContent353694Enabled -and $SubscribedContent353696Enabled)
{
	New-ItemProperty -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-338393Enabled -PropertyType DWord -Value 0 -Force
	New-ItemProperty -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-353694Enabled -PropertyType DWord -Value 0 -Force
	New-ItemProperty -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SubscribedContent-353696Enabled -PropertyType DWord -Value 0 -Force
}

# Отключить автоматическую установку рекомендованных приложений
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path  HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SilentInstalledAppsEnabled) -ne 0)
{
	"Scripts\1. Privacy\16. Turn off automatic installing suggested apps.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SilentInstalledAppsEnabled) -eq 1)
{
	Remove-ItemProperty -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager -Name SilentInstalledAppsEnabled -Force
}

# Не предлагать способыe завершения настройки устройства для максимально эффективного использования Windows
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\UserProfileEngagement -Name ScoobeSystemSettingEnabled) -ne 0)
{
	"Scripts\1. Privacy\17. Do not suggest ways I can finish setting up my device to get the most out of Windows.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\UserProfileEngagement -Name ScoobeSystemSettingEnabled) -eq 1)
{
	Remove-ItemProperty -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\UserProfileEngagement -Name ScoobeSystemSettingEnabled -Force
}

# Не предлагать персонализированные возможности, основанные на выбранном параметре диагностических данных
# Тумблер находится слева
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\Privacy -Name TailoredExperiencesWithDiagnosticDataEnabled) -ne 0)
{
	"Scripts\1. Privacy\18. Do not offer tailored experiences based on the diagnostic data setting.ps1"
}
# Вернуть по умолчанию
# Тумблер находится справа
if ((Get-ItemPropertyValue -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\Privacy -Name TailoredExperiencesWithDiagnosticDataEnabled) -eq 1)
{
	Remove-ItemProperty -Path HKCU:\Software\Microsoft\Windows\CurrentVersion\Privacy -Name TailoredExperiencesWithDiagnosticDataEnabled -Force
}