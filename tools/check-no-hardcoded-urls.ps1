Param(
    [string[]]$Paths = @("src", "tests")
)

$pattern = "https?://"
$globs = @("*.cs", "*.cshtml", "*.js", "*.ts", "*.tsx", "*.csx", "!**/wwwroot/lib/**")

$arguments = @("-n", $pattern) + ($Paths | ForEach-Object { $_ }) + ($globs | ForEach-Object { "-g"; $_ })
$output = & rg @arguments 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Error "Hardcoded URLs detected. Move them to appsettings and bind via Options."
    Write-Output $output
    exit 1
}

Write-Output "No hardcoded URLs found."
