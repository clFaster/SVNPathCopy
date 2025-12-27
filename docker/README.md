# SVN Test Environment

Local SVN server for testing the SVNPathCopy shell extension.

## Prerequisites

- **Podman** installed with `podman machine` running
- **SVN client** (TortoiseSVN command-line tools or SlikSVN)

## Quick Start

```powershell
cd docker
.\setup-test-env.ps1
```

That's it! The script will:
1. Start the SVN server container
2. Create a test repository with sample commits
3. Set up a working copy at `C:\temp\svnpathcopy-test-wc`
4. Open Explorer to the test folder

## Test Data Created

| File/Folder | Status | Use Case |
|-------------|--------|----------|
| `committed_file.txt` | Clean, committed | Copy URL with revision |
| `modified_file.txt` | Modified (uncommitted) | Copy URL of modified file |
| `added_file.txt` | Scheduled for add | Copy URL before first commit |
| `unversioned_file.txt` | Unversioned | Should show error/not appear |
| `committed_folder/` | Clean, committed | Folder URL copying |
| `file with spaces.txt` | Clean, committed | URL encoding test |
| `special-chars_äöü.txt` | Clean, committed | Unicode handling |

## Commands

| Action | Command |
|--------|---------|
| Full reset + setup | `.\setup-test-env.ps1` |
| Reset only (no setup) | `.\setup-test-env.ps1 -ResetOnly` |
| Custom working copy path | `.\setup-test-env.ps1 -WorkingCopyPath "D:\my-test"` |

## Troubleshooting

- **Podman not running**: Run `podman machine init` then `podman machine start`
- **SVN not found**: Install TortoiseSVN with command-line tools option
- **Port 3690 in use**: Stop other SVN servers or change the port in `docker-compose.yml`
