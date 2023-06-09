$UnityEditorInstallDir = 'D:\Programs\Unity\Editors'
$UnityEditorVersion = '2021.3.24f1'

$SimpleWebServer = Join-Path `
    $UnityEditorInstallDir $UnityEditorVersion `
    'Editor' 'Data' 'PlaybackEngines' 'WebGLSupport' 'BuildTools' `
    'SimpleWebServer.exe'

$Port = 7777
$BuildDir = Join-Path $PSScriptRoot 'Build' 'WebGL'

if (!(Test-Path $BuildDir)) {
    Write-Error "WebGL build directory not found: $BuildDir"
    exit 1
}

Write-Host "Launching browser..."
Start-Process "http://localhost:$Port"

& $SimpleWebServer $BuildDir $Port