$root = Resolve-Path (Join-Path $PSScriptRoot '..')
$envFile = Join-Path $root '.env'
$localFile = Join-Path $root '.env.local'

function Import-EnvFile {
    param([string] $Path)
    if (-not (Test-Path -LiteralPath $Path)) {
        return
    }

    foreach ($rawLine in Get-Content -LiteralPath $Path) {
        $line = $rawLine.Trim()
        if ($line.Length -eq 0 -or $line.StartsWith('#')) {
            continue
        }

        $separator = $line.IndexOf('=')
        if ($separator -lt 1) {
            continue
        }

        $name = $line.Substring(0, $separator).Trim()
        $value = $line.Substring($separator + 1).Trim().Trim('"')
        Set-Item -Path "Env:$name" -Value $value
    }
}

function Add-LocalSecret {
    param([string] $Name)
    $current = [Environment]::GetEnvironmentVariable($Name, 'Process')
    if (-not [string]::IsNullOrWhiteSpace($current)) {
        return
    }

    $bytes = New-Object byte[] 48
    [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
    $secret = [Convert]::ToBase64String($bytes)
    Add-Content -LiteralPath $localFile -Value "$Name=$secret"
    Set-Item -Path "Env:$Name" -Value $secret
}

Import-EnvFile $envFile
Import-EnvFile $localFile
Add-LocalSecret 'Authentication__Jwt__SigningKey'
Add-LocalSecret 'MoodleIntegration__ApiKey'
Add-LocalSecret 'Authentication__BootstrapKey'

$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:RabbitMq__HostName = 'localhost'
if ($env:RABBITMQ_DEFAULT_USER) { $env:RabbitMq__UserName = $env:RABBITMQ_DEFAULT_USER }
if ($env:RABBITMQ_DEFAULT_PASS) { $env:RabbitMq__Password = $env:RABBITMQ_DEFAULT_PASS }
$env:MoodleIntegration__BaseUrl = 'http://localhost:5010'
$env:AcademicAnalysis__BaseUrl = 'http://localhost:5030'
$env:AcademicAgent__BaseUrl = 'http://localhost:8010'
if ($env:ACADEMIC_AGENT_API_KEY) { $env:AcademicAgent__ApiKey = $env:ACADEMIC_AGENT_API_KEY }
if (-not $env:MoodleSettings__BaseUrl) { $env:MoodleSettings__BaseUrl = 'http://localhost:8080' }
