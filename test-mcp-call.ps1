<#
.SYNOPSIS
  Call a CarRental.MCP tool directly, without any MCP client, for local testing.

.EXAMPLE
  .\test-mcp-call.ps1 -ListTools

.EXAMPLE
  .\test-mcp-call.ps1 -ToolName list_vehicles

.EXAMPLE
  .\test-mcp-call.ps1 -ToolName get_vehicle -ArgsJson '{"vehicleId":48213}'
#>
param(
    [string]$ToolName = "list_vehicles",
    [string]$ArgsJson = "{}",
    [switch]$ListTools
)

$exePath = "C:\PoojaWorkSpace\VSMCPServer\publish\CarRental.MCP\CarRental.MCP.exe"
if (-not (Test-Path $exePath)) {
    Write-Error "Exe not found at $exePath - run 'dotnet publish src\CarRental.MCP -c Release -o publish\CarRental.MCP' first."
    exit 1
}

$psi = New-Object System.Diagnostics.ProcessStartInfo
$psi.FileName = $exePath
$psi.RedirectStandardInput = $true
$psi.RedirectStandardOutput = $true
$psi.RedirectStandardError = $true
$psi.UseShellExecute = $false
$psi.EnvironmentVariables["DOTNET_ENVIRONMENT"] = "Development"

$proc = New-Object System.Diagnostics.Process
$proc.StartInfo = $psi

$stdoutLines = [System.Collections.Concurrent.ConcurrentQueue[string]]::new()
Register-ObjectEvent -InputObject $proc -EventName OutputDataReceived -Action {
    if ($EventArgs.Data) { $Event.MessageData.Enqueue($EventArgs.Data) }
} -MessageData $stdoutLines | Out-Null

$stderrLines = [System.Collections.Concurrent.ConcurrentQueue[string]]::new()
Register-ObjectEvent -InputObject $proc -EventName ErrorDataReceived -Action {
    if ($EventArgs.Data) { $Event.MessageData.Enqueue($EventArgs.Data) }
} -MessageData $stderrLines | Out-Null

$proc.Start() | Out-Null
$proc.BeginOutputReadLine()
$proc.BeginErrorReadLine()

$stdin = $proc.StandardInput
$stdin.AutoFlush = $true
$stdin.WriteLine('{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-06-18","capabilities":{},"clientInfo":{"name":"local-test","version":"1.0"}}}')
$stdin.WriteLine('{"jsonrpc":"2.0","method":"notifications/initialized"}')

if ($ListTools) {
    $stdin.WriteLine('{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}')
} else {
    $callMsg = @{ jsonrpc = "2.0"; id = 2; method = "tools/call"; params = @{ name = $ToolName; arguments = (ConvertFrom-Json $ArgsJson) } } | ConvertTo-Json -Depth 10 -Compress
    $stdin.WriteLine($callMsg)
}

$sw = [System.Diagnostics.Stopwatch]::StartNew()
while ($sw.Elapsed.TotalSeconds -lt 25 -and $stdoutLines.Count -lt 2) {
    Start-Sleep -Milliseconds 200
}

$stdin.Close()
Start-Sleep -Milliseconds 200
if (-not $proc.HasExited) { Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue }

if ($stdoutLines.Count -lt 2) {
    Write-Warning "No response within 25s."
    Write-Output "---stderr---"
    $stderrLines | Select-Object -Last 20
    exit 1
}

$responseLine = ($stdoutLines | Select-Object -Last 1)
$response = $responseLine | ConvertFrom-Json

if ($ListTools) {
    $response.result.tools | ForEach-Object {
        [pscustomobject]@{ Name = $_.name; Description = $_.description }
    } | Format-Table -AutoSize -Wrap
} elseif ($response.result.isError) {
    Write-Output "TOOL ERROR:"
    Write-Output $response.result.content[0].text
    Write-Output "---stderr (filtered)---"
    $stderrLines | Where-Object { $_ -match "error|exception|fail" } | Select-Object -Last 15
} else {
    $text = $response.result.content[0].text
    try {
        $text | ConvertFrom-Json | ConvertTo-Json -Depth 10
    } catch {
        Write-Output $text
    }
}
