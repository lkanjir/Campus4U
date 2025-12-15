$exePath = Get-ChildItem "..\Campus4U\bin\Debug" -Filter Campus4U.exe -Recurse -ErrorAction SilentlyContinue |
         Select-Object -First 1

if (-not $exePath) {
  Write-Error "Campus4U.exe nije pronađen. Prvo buildaj projekt za Debug (Campus4U\bin\Debug\...\Campus4U.exe)."
  exit 1
}

$base = 'HKCU:\Software\Classes\campus4u'
Write-Host "Registracija campus4u:// za $($exePath.FullName)"

New-Item -Path $base -Force | Out-Null
Set-ItemProperty -Path $base -Name '(default)' -Value 'URL:Campus4U Protocol'
Set-ItemProperty -Path $base -Name 'URL Protocol' -Value ''

New-Item -Path "$base\shell\open\command" -Force | Out-Null
Set-ItemProperty -Path "$base\shell\open\command" -Name '(default)' -Value "`"$($exePath.FullName)`" `"%1`""

Write-Host 'Gotovo. Test (powershell): Start-Process "campus4u://callback?code=test" ili Win + R -> campus4u://callback?code=test'

#za brisanje kljuca iz HKCU -> reg delete HKCU\Software\Classes\campus4u /f