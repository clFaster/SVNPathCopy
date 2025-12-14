# PowerShell script to quickly set up a test SVN working copy
# Run this after starting the Docker container

param(
    [string]$WorkingCopyPath = "C:\temp\svnpathcopy-test-wc",
    [string]$SvnUrl = "svn://localhost/testrepo",
    [string]$Username = "svnadmin",
    [string]$Password = "svnadmin"
)

Write-Host "SVN Test Setup Script" -ForegroundColor Cyan
Write-Host "=====================" -ForegroundColor Cyan

# Check if SVN client is available
$svnPath = "C:\Program Files\TortoiseSVN\bin\svn.exe"
if (-not (Test-Path $svnPath)) {
    Write-Host "ERROR: SVN command-line client not found at $svnPath!" -ForegroundColor Red
    Write-Host "Please install TortoiseSVN command-line tools from:" -ForegroundColor Yellow
    Write-Host "  https://tortoisesvn.net/downloads.html" -ForegroundColor Yellow
    Write-Host "Look for the MSI that includes 'svn' in the filename." -ForegroundColor Yellow
    exit 1
}

# Check if Docker container is running
$container = podman ps --filter "name=svn-server" --format "{{.Names}}" 2>$null
if (-not $container) {
    Write-Host "WARNING: SVN server container doesn't appear to be running!" -ForegroundColor Yellow
    Write-Host "Starting it now..." -ForegroundColor Yellow
    Push-Location $PSScriptRoot
    podman compose up -d
    Pop-Location
    Start-Sleep -Seconds 5
}

# Create working copy directory
if (-not (Test-Path $WorkingCopyPath)) {
    Write-Host "Creating working copy directory: $WorkingCopyPath" -ForegroundColor Green
    New-Item -ItemType Directory -Path $WorkingCopyPath -Force | Out-Null
}

# Check if already a working copy
if (Test-Path (Join-Path $WorkingCopyPath ".svn")) {
    Write-Host "Directory is already an SVN working copy." -ForegroundColor Yellow
    Write-Host "Updating..." -ForegroundColor Yellow
    Push-Location $WorkingCopyPath
    & $svnPath update --username $Username --password $Password --non-interactive
    Pop-Location
} else {
    Write-Host "Checking out repository..." -ForegroundColor Green
    & $svnPath checkout $SvnUrl $WorkingCopyPath --username $Username --password $Password --non-interactive
}

Write-Host "Creating/ensuring test scenario (3 commits + uncommitted items)..." -ForegroundColor Green

# Always create three sequential commits with clearly named files.
# (SVN revision numbers are global, so we label by commit step rather than assuming rev=1/2/3.)
$commit1File = Join-Path $WorkingCopyPath "01_commit1_baseline.txt"
$commit2File = Join-Path $WorkingCopyPath "02_commit2_modified.txt"
$commit3File = Join-Path $WorkingCopyPath "03_commit3_added.txt"

Push-Location $WorkingCopyPath
& $svnPath update --username $Username --password $Password --non-interactive | Out-Null

# Clean up legacy artifacts from older versions of this script (working copy only)
foreach ($legacyPath in @(
    (Join-Path $WorkingCopyPath "01_committed_rev1_baseline.txt"),
    (Join-Path $WorkingCopyPath "02_committed_rev2_modified.txt"),
    (Join-Path $WorkingCopyPath "03_committed_rev3_added.txt"),
    (Join-Path $WorkingCopyPath "rev1_committed_folder"),
    (Join-Path $WorkingCopyPath "uncommitted_folder")
)) {
    if (Test-Path $legacyPath) {
        try { Remove-Item -Recurse -Force $legacyPath } catch { }
    }
}

if (-not (Test-Path $commit1File) -or -not (Test-Path $commit2File)) {
    "COMMIT 1 - baseline committed" | Out-File -FilePath $commit1File -Encoding UTF8
    "COMMIT 1 - will be modified & committed in commit 2" | Out-File -FilePath $commit2File -Encoding UTF8

    $commit1Folder = Join-Path $WorkingCopyPath "commit1_committed_folder"
    New-Item -ItemType Directory -Path $commit1Folder -Force | Out-Null
    "COMMIT 1 - nested committed" | Out-File -FilePath (Join-Path $commit1Folder "01_nested_commit1.txt") -Encoding UTF8

    & $svnPath add 01_commit1_baseline.txt 02_commit2_modified.txt commit1_committed_folder --force | Out-Null
    & $svnPath commit 01_commit1_baseline.txt 02_commit2_modified.txt commit1_committed_folder -m "commit1: baseline" --username $Username --password $Password --non-interactive | Out-Null
}

"COMMIT 2 - modified and committed" | Out-File -FilePath $commit2File -Encoding UTF8
& $svnPath commit 02_commit2_modified.txt -m "commit2: modify 02_commit2_modified.txt" --username $Username --password $Password --non-interactive | Out-Null

if (-not (Test-Path $commit3File)) {
    "COMMIT 3 - added and committed" | Out-File -FilePath $commit3File -Encoding UTF8
    & $svnPath add 03_commit3_added.txt | Out-Null
    & $svnPath commit 03_commit3_added.txt -m "commit3: add 03_commit3_added.txt" --username $Username --password $Password --non-interactive | Out-Null
}

& $svnPath update --username $Username --password $Password --non-interactive | Out-Null
Pop-Location

# Create working-copy-only uncommitted items
"UNCOMMITTED - modified working copy (do not commit)" | Out-File -FilePath $commit1File -Encoding UTF8

"UNVERSIONED - new file (do not add)" | Out-File -FilePath (Join-Path $WorkingCopyPath "10_unversioned_uncommitted.txt") -Encoding UTF8

$unversionedFolder = Join-Path $WorkingCopyPath "20_unversioned_folder"
New-Item -ItemType Directory -Path $unversionedFolder -Force | Out-Null
"UNVERSIONED - nested file" | Out-File -FilePath (Join-Path $unversionedFolder "21_unversioned_nested.txt") -Encoding UTF8

"SCHEDULED-ADD - file is added but not committed" | Out-File -FilePath (Join-Path $WorkingCopyPath "30_scheduled_add_not_committed.txt") -Encoding UTF8
Push-Location $WorkingCopyPath
& $svnPath add 30_scheduled_add_not_committed.txt 2>$null | Out-Null
Pop-Location

Write-Host ""
Write-Host "Setup complete!" -ForegroundColor Cyan
Write-Host ""
Write-Host "Working copy location: $WorkingCopyPath" -ForegroundColor White
Write-Host "SVN URL: $SvnUrl" -ForegroundColor White
Write-Host ""
Write-Host "To test SVNPathCopy:" -ForegroundColor Yellow
Write-Host "  1. Open Windows Explorer" -ForegroundColor Yellow
Write-Host "  2. Navigate to: $WorkingCopyPath" -ForegroundColor Yellow
Write-Host "  3. Right-click on any file" -ForegroundColor Yellow
Write-Host "  4. Look for 'SVN Path Copy' menu items" -ForegroundColor Yellow
Write-Host ""

# Open Explorer to the working copy
Write-Host "Opening Explorer to working copy..." -ForegroundColor Green
Start-Process explorer $WorkingCopyPath
