# Reset the local SVN test environment (Podman)
# - Stops the compose stack
# - Removes the named volume (SVN repositories)
# - Removes the default working copy folder

param(
    [string]$WorkingCopyPath = "C:\temp\svnpathcopy-test-wc"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Write-Host "Resetting SVNPathCopy test environment" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan

Push-Location $PSScriptRoot
try {
    Write-Host "Stopping containers..." -ForegroundColor Green
    podman compose down | Out-Null

    Write-Host "Removing SVN volume (docker_svn-repos)..." -ForegroundColor Green
    # Compose provider names the volume 'docker_svn-repos' based on the folder name.
    podman volume rm -f docker_svn-repos 2>$null | Out-Null

    if (Test-Path $WorkingCopyPath) {
        Write-Host "Removing working copy: $WorkingCopyPath" -ForegroundColor Green
        try {
            Remove-Item -Recurse -Force $WorkingCopyPath
        } catch {
            Write-Host "WARNING: Could not remove working copy (in use): $WorkingCopyPath" -ForegroundColor Yellow
        }
    }

    Write-Host "Starting fresh server..." -ForegroundColor Green
    podman compose up -d | Out-Null

    Write-Host "Done." -ForegroundColor Green
} finally {
    Pop-Location
}
