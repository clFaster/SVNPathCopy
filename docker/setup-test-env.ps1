<#
.SYNOPSIS
    Sets up a complete SVN test environment for SVNPathCopy development.

.DESCRIPTION
    This script:
    1. Resets any existing SVN server container and volume
    2. Starts a fresh SVN server
    3. Creates a test repository with multiple commits
    4. Sets up a working copy with various file states for testing

.PARAMETER WorkingCopyPath
    Path where the SVN working copy will be created.
    Default: C:\temp\svnpathcopy-test-wc

.PARAMETER ResetOnly
    Only reset the environment, don't create test data.

.EXAMPLE
    .\setup-test-env.ps1
    
.EXAMPLE
    .\setup-test-env.ps1 -WorkingCopyPath "D:\svn-test"
#>

param(
    [string]$WorkingCopyPath = "C:\temp\svnpathcopy-test-wc",
    [string]$SvnUrl = "svn://localhost/testrepo",
    [string]$Username = "svnadmin",
    [string]$Password = "svnadmin",
    [switch]$ResetOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ============================================================================
# Helper Functions
# ============================================================================

function Write-Step {
    param([string]$Message)
    Write-Host "`n>> $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "   $Message" -ForegroundColor Green
}

function Write-Warn {
    param([string]$Message)
    Write-Host "   $Message" -ForegroundColor Yellow
}

function Get-SvnPath {
    # Check common SVN locations
    $locations = @(
        "C:\Program Files\TortoiseSVN\bin\svn.exe",
        "C:\Program Files (x86)\TortoiseSVN\bin\svn.exe",
        "C:\Program Files\SlikSvn\bin\svn.exe"
    )
    
    foreach ($loc in $locations) {
        if (Test-Path $loc) {
            return $loc
        }
    }
    
    # Try PATH
    $svn = Get-Command svn -ErrorAction SilentlyContinue
    if ($svn) {
        return $svn.Source
    }
    
    return $null
}

function Invoke-Svn {
    param(
        [string[]]$Arguments,
        [switch]$ShowOutput
    )
    $allArgs = $Arguments + @("--username", $Username, "--password", $Password, "--non-interactive", "--no-auth-cache")
    if ($ShowOutput) {
        & $script:SvnExe @allArgs
    } else {
        & $script:SvnExe @allArgs 2>&1 | Out-Null
    }
    if ($LASTEXITCODE -ne 0) {
        throw "SVN command failed: svn $($Arguments -join ' ')"
    }
}

function Invoke-SvnWithRetry {
    param(
        [string[]]$Arguments,
        [int]$MaxRetries = 5,
        [int]$DelaySeconds = 2
    )
    for ($i = 1; $i -le $MaxRetries; $i++) {
        try {
            Invoke-Svn -Arguments $Arguments
            return
        }
        catch {
            if ($i -eq $MaxRetries) {
                throw
            }
            Write-Warn "Attempt $i failed, retrying in $DelaySeconds seconds..."
            Start-Sleep -Seconds $DelaySeconds
        }
    }
}

# ============================================================================
# Prerequisites Check
# ============================================================================

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " SVNPathCopy Test Environment Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Step "Checking prerequisites..."

# Check Podman
$podman = Get-Command podman -ErrorAction SilentlyContinue
if (-not $podman) {
    Write-Host "ERROR: Podman is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Install from: https://podman.io/getting-started/installation" -ForegroundColor Yellow
    exit 1
}
Write-Success "Podman found"

# Check SVN
$script:SvnExe = Get-SvnPath
if (-not $script:SvnExe) {
    Write-Host "ERROR: SVN command-line client not found!" -ForegroundColor Red
    Write-Host "Install TortoiseSVN with command-line tools from:" -ForegroundColor Yellow
    Write-Host "  https://tortoisesvn.net/downloads.html" -ForegroundColor Yellow
    exit 1
}
Write-Success "SVN client found: $script:SvnExe"

# ============================================================================
# Reset Environment
# ============================================================================

Write-Step "Resetting environment..."

Push-Location $PSScriptRoot
try {
    # Stop containers
    Write-Success "Stopping containers..."
    podman compose down 2>&1 | Out-Null

    # Remove volume
    Write-Success "Removing SVN data volume..."
    podman volume rm -f docker_svn-repos 2>&1 | Out-Null

    # Remove working copy
    if (Test-Path $WorkingCopyPath) {
        Write-Success "Removing working copy: $WorkingCopyPath"
        try {
            Remove-Item -Recurse -Force $WorkingCopyPath -ErrorAction Stop
        }
        catch {
            Write-Warn "Could not fully remove working copy (may be in use)"
            # Try to remove what we can
            Get-ChildItem $WorkingCopyPath -Recurse -Force -ErrorAction SilentlyContinue | 
                Remove-Item -Force -Recurse -ErrorAction SilentlyContinue
        }
    }

    # Start fresh server
    Write-Success "Starting SVN server..."
    podman compose up -d 2>&1 | Out-Null

    # Wait for server to be ready
    Write-Success "Waiting for server to initialize..."
    Start-Sleep -Seconds 3

}
finally {
    Pop-Location
}

if ($ResetOnly) {
    Write-Host "`nReset complete. Use -ResetOnly:`$false or run without the flag to create test data." -ForegroundColor Green
    exit 0
}

# ============================================================================
# Create Working Copy
# ============================================================================

Write-Step "Creating working copy..."

# Create directory
if (-not (Test-Path $WorkingCopyPath)) {
    New-Item -ItemType Directory -Path $WorkingCopyPath -Force | Out-Null
}

# Checkout (with retry since server may still be starting)
Write-Success "Checking out repository..."
Invoke-SvnWithRetry @("checkout", $SvnUrl, $WorkingCopyPath)

Push-Location $WorkingCopyPath
try {
    # ============================================================================
    # Commit 1: Basic files and folders
    # ============================================================================
    
    Write-Step "Creating commit 1: Basic structure..."
    
    # Simple committed file
    "This is a committed file at revision 1.`nUse this to test 'Copy SVN URL with REV'." | 
        Out-File -FilePath "committed_file.txt" -Encoding UTF8
    
    # File that will be modified later
    "Original content - will be modified." | 
        Out-File -FilePath "modified_file.txt" -Encoding UTF8
    
    # Folder with nested content
    New-Item -ItemType Directory -Path "committed_folder" -Force | Out-Null
    "Nested file in committed folder." | 
        Out-File -FilePath "committed_folder\nested_file.txt" -Encoding UTF8
    
    # File with spaces (tests URL encoding)
    "File with spaces in the name for URL encoding tests." | 
        Out-File -FilePath "file with spaces.txt" -Encoding UTF8
    
    # File with special characters
    "File with special characters for encoding tests." | 
        Out-File -FilePath "special-chars_äöü.txt" -Encoding UTF8
    
    Invoke-Svn @("add", "committed_file.txt", "modified_file.txt", "committed_folder", "file with spaces.txt", "special-chars_äöü.txt")
    Invoke-Svn @("commit", "-m", "Initial commit: basic test files")
    
    Write-Success "Commit 1 complete"
    
    # ============================================================================
    # Commit 2: Modify a file (creates different revision)
    # ============================================================================
    
    Write-Step "Creating commit 2: Modified file..."
    
    "Modified content - this file now has a different revision than committed_file.txt." | 
        Out-File -FilePath "modified_file.txt" -Encoding UTF8
    
    Invoke-Svn @("commit", "-m", "Modify modified_file.txt")
    
    Write-Success "Commit 2 complete"
    
    # ============================================================================
    # Commit 3: Add another file (shows revision progression)
    # ============================================================================
    
    Write-Step "Creating commit 3: Additional file..."
    
    "This file was added in revision 3.`nDemonstrates revision numbers progressing." | 
        Out-File -FilePath "later_addition.txt" -Encoding UTF8
    
    Invoke-Svn @("add", "later_addition.txt")
    Invoke-Svn @("commit", "-m", "Add later_addition.txt")
    
    Write-Success "Commit 3 complete"
    
    # ============================================================================
    # Update to get all changes
    # ============================================================================
    
    Invoke-Svn @("update")
    
    # ============================================================================
    # Create uncommitted states for testing
    # ============================================================================
    
    Write-Step "Creating uncommitted test states..."
    
    # Modified but not committed (status: M)
    "LOCAL MODIFICATION - not committed yet.`nShould still be able to copy URL." | 
        Out-File -FilePath "modified_file.txt" -Encoding UTF8
    Write-Success "Modified file: modified_file.txt (status: M)"
    
    # Scheduled for add but not committed (status: A)
    "SCHEDULED FOR ADD - svn add was run but not committed.`nTests URL availability for pending adds." | 
        Out-File -FilePath "added_file.txt" -Encoding UTF8
    Invoke-Svn @("add", "added_file.txt")
    Write-Success "Added file: added_file.txt (status: A)"
    
    # Unversioned file (status: ?)
    "UNVERSIONED - not tracked by SVN.`nContext menu should handle this gracefully." | 
        Out-File -FilePath "unversioned_file.txt" -Encoding UTF8
    Write-Success "Unversioned file: unversioned_file.txt (status: ?)"
    
    # Unversioned folder with files
    New-Item -ItemType Directory -Path "unversioned_folder" -Force | Out-Null
    "Unversioned nested file." | 
        Out-File -FilePath "unversioned_folder\some_file.txt" -Encoding UTF8
    Write-Success "Unversioned folder: unversioned_folder/ (status: ?)"

}
finally {
    Pop-Location
}

# ============================================================================
# Summary
# ============================================================================

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " Setup Complete!" -ForegroundColor Cyan  
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`nWorking Copy: " -NoNewline -ForegroundColor White
Write-Host $WorkingCopyPath -ForegroundColor Yellow

Write-Host "SVN URL:      " -NoNewline -ForegroundColor White
Write-Host $SvnUrl -ForegroundColor Yellow

Write-Host "`nTest Files Created:" -ForegroundColor White
Write-Host "  [Committed]   committed_file.txt      - Clean, at revision 1" -ForegroundColor Gray
Write-Host "  [Committed]   modified_file.txt       - Locally modified (uncommitted changes)" -ForegroundColor Gray
Write-Host "  [Committed]   later_addition.txt      - Clean, at revision 3" -ForegroundColor Gray
Write-Host "  [Committed]   file with spaces.txt    - Tests URL encoding" -ForegroundColor Gray
Write-Host "  [Committed]   special-chars_äöü.txt   - Tests Unicode handling" -ForegroundColor Gray
Write-Host "  [Committed]   committed_folder/       - Committed directory" -ForegroundColor Gray
Write-Host "  [Added]       added_file.txt          - Scheduled for add, not committed" -ForegroundColor Gray
Write-Host "  [Unversioned] unversioned_file.txt    - Not tracked by SVN" -ForegroundColor Gray
Write-Host "  [Unversioned] unversioned_folder/     - Unversioned directory" -ForegroundColor Gray

Write-Host "`nTo test SVNPathCopy:" -ForegroundColor Yellow
Write-Host "  1. Right-click files in Explorer" -ForegroundColor White
Write-Host "  2. Look for 'Copy SVN URL' menu items" -ForegroundColor White
Write-Host "  3. Paste to verify the copied URL" -ForegroundColor White

Write-Host "`nOpening Explorer..." -ForegroundColor Green
Start-Process explorer $WorkingCopyPath
