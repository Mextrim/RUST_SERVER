# Keeps the Rust server running.
#
# If the server process ends for any reason, closed window included, the watchdog
# starts it again after a short delay. Started by start-watchdog.bat, stopped by
# stop-watchdog.bat. Everything it does is recorded in watchdog.log.

$ErrorActionPreference = 'Continue'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$stopFile = Join-Path $root 'stop_watchdog.txt'
$logFile = Join-Path $root 'watchdog.log'
$runScript = Join-Path $root 'run.bat'

function Write-Log([string]$message)
{
    Add-Content -Path $logFile -Encoding UTF8 -Value ((Get-Date -Format 'yyyy-MM-dd HH:mm:ss') + ' ' + $message)
}

# Only one watchdog may run. A second copy would fight over the server process.
$mutex = New-Object System.Threading.Mutex($false, 'Local\RLXRustServerWatchdog')
if (-not $mutex.WaitOne(0, $false))
{
    Write-Log 'another watchdog instance is already running, exiting'
    exit 0
}

if (-not (Test-Path $runScript))
{
    Write-Log ('run.bat not found at ' + $runScript + ', exiting')
    exit 1
}

if (Test-Path $stopFile) { Remove-Item $stopFile -Force }

# run.bat must not stop on its pause, otherwise it looks alive while the server is down.
$env:RLX_NO_PAUSE = '1'

Write-Log 'watchdog started'

while ($true)
{
    if (Test-Path $stopFile)
    {
        Write-Log 'stop file found, watchdog exiting'
        break
    }

    # The server may have been started by hand. Let it be, just wait for it to end.
    if (Get-Process -Name 'RustDedicated' -ErrorAction SilentlyContinue)
    {
        Start-Sleep -Seconds 10
        continue
    }

    Write-Log 'starting server'
    $process = Start-Process -FilePath 'cmd.exe' -ArgumentList '/c', ('"' + $runScript + '"') -WorkingDirectory $root -WindowStyle Minimized -PassThru
    Write-Log ('server window started, pid ' + $process.Id)

    while (-not $process.HasExited)
    {
        if (Test-Path $stopFile) { break }
        Start-Sleep -Seconds 5
    }

    if ($process.HasExited)
    {
        Write-Log ('server exited with code ' + $process.ExitCode + ', restarting in 10 seconds')
        Start-Sleep -Seconds 10
    }
}

Write-Log 'watchdog stopped'
$mutex.ReleaseMutex()
$mutex.Dispose()