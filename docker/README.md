# SVN Test Server (Podman)

Minimal instructions to run a local SVN server for testing the SVNPathCopy shell extension using Podman.

Prerequisites
- Podman installed and `podman machine` available on Windows
- SVN command-line client (TortoiseSVN command-line tools or SlikSVN)

Quick start
```powershell
cd docker
podman machine start
podman compose up -d
```

Checkout the test repository
```powershell
mkdir C:\temp\svn-test
cd C:\temp\svn-test
svn checkout svn://localhost/testrepo . --username svnadmin --password svnadmin
```

Reset the test environment (fresh repo + scripted test data)
```powershell
cd docker
./reset-test-env.ps1
./setup-test-wc.ps1
```

What the test setup provides
- Repository URL: `svn://localhost/testrepo`
- Default credentials: `svnadmin` / `svnadmin`
- The `setup-test-wc.ps1` script creates three sequential commits with clearly named files, and leaves a mix of uncommitted items (modified, unversioned, scheduled-add) so you can test all states.

Quick troubleshooting
- If Podman cannot connect, run `podman machine init` and `podman machine start`.
- If `svn` is not found, install TortoiseSVN (include command-line tools) or SlikSVN.

Reset and recreate steps are intentionally minimal — use `reset-test-env.ps1` to destroy the volume and start fresh.

That's all you need to run local tests for the shell extension.
