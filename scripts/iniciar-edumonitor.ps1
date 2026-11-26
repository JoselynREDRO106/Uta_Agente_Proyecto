$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$loader = Join-Path $PSScriptRoot 'cargar-entorno.ps1'

function Test-PortListening {
    param([int] $Port)
    $connection = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue |
        Select-Object -First 1
    return $null -ne $connection
}

function Start-ApiWindow {
    param(
        [string] $Title,
        [string] $Project,
        [string] $Url,
        [int] $Port
    )

    if (Test-PortListening $Port) {
        Write-Output "$Title ya escucha en el puerto $Port. No se reinició."
        return
    }

    $command = @"
`$Host.UI.RawUI.WindowTitle = '$Title'
Set-Location -LiteralPath '$root'
. '$loader'
dotnet run --project '$Project' --urls $Url --no-launch-profile
"@
    Start-Process -FilePath 'powershell.exe' -WorkingDirectory $root -ArgumentList '-NoExit', '-Command', $command
    Write-Output "$Title se está iniciando en $Url."
}

Write-Output 'EduMonitor: preparando entorno local.'
. $loader

if (-not $env:RABBITMQ_DEFAULT_PASS) {
    Write-Output 'Falta RABBITMQ_DEFAULT_PASS en .env. RabbitMQ y los servicios que publican eventos no van a conectar.'
}

if (Get-Command docker -ErrorAction SilentlyContinue) {
    docker compose --env-file (Join-Path $root '.env') up -d rabbitmq
    if ($LASTEXITCODE -eq 0) {
        docker compose --env-file (Join-Path $root '.env') --profile ai up -d --build academic-agent
    }
}
else {
    Write-Output 'Docker no está disponible. RabbitMQ y el agente no se iniciaron.'
}

Start-ApiWindow 'EduMonitor Identity' 'backend\Uta.Identity.WebApi' 'http://localhost:5020' 5020
Start-ApiWindow 'EduMonitor Moodle' 'backend\Uta.MoodleIntegration.WebApi' 'http://localhost:5010' 5010
Start-ApiWindow 'EduMonitor Analysis' 'backend\Uta.AcademicAnalysis.WebApi' 'http://localhost:5030' 5030
Start-ApiWindow 'EduMonitor Middleware' 'backend\Uta.Middleware.WebApi' 'http://localhost:5000' 5000

if (-not (Test-PortListening 5173)) {
    $frontend = @"
`$Host.UI.RawUI.WindowTitle = 'EduMonitor Frontend'
Set-Location -LiteralPath '$root\frontend'
npm run dev
"@
    Start-Process -FilePath 'powershell.exe' -WorkingDirectory (Join-Path $root 'frontend') -ArgumentList '-NoExit', '-Command', $frontend
    Write-Output 'Frontend se está iniciando en http://localhost:5173.'
}
else {
    Write-Output 'Frontend ya escucha en el puerto 5173.'
}

Write-Output 'Listo. Las claves nuevas, si hicieron falta, quedaron en .env.local y no se muestran aquí.'
if ((Test-PortListening 5030) -or (Test-PortListening 5000)) {
    Write-Output 'Si Análisis o Middleware ya estaban encendidos, ciérralos y vuelve a ejecutar este script para cargar el libro de calificaciones y la clave del agente.'
}
