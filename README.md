# SVN Path Copy

Adds an context menu entry to copy the url of an SVN file or folder with the last change revision number (?p=number).
> For Example: https://your-svn.org/your-repo/file.txt?p=2

![SVNPathCopy Context Menu preview](docs/images/SVNPathCopyExtension.PNG)

## Flowchart
```mermaid
flowchart TD
    A[Right-Click on File or Folder] --> B{Is in SVN Repo?}
    B -- No --> C[Don't show SVN Path Copy Context Menu]
    B -- Yes --> D[Show SVN Path Copy Context Menu]
    D --> E[Click on Context Menu Item]
    E --> F{Is Item under SVN Version Control?}
    F -- No --> G[Show Error]
    F -- Yes --> H{Was Item locally changed?}
    H -- No --> I[Show Error]
    H -- Yes --> J[Copy Path]
```

## Developing

SVN Path Copy is currently developed in Visual Studio 2017.

### Dependecies

 - [SharpShell](https://github.com/dwmkerr/sharpshell)
 - [SharpSVN](https://sharpsvn.open.collab.net/)

### Building

If all dependencies are installed you can build SVN Path Copy in Visual Studios.
Open the main project `./SVNPathCopy/SVNPathCopy.sln` and you should be able to build it.

## Licensing

SVNPathCopy is under the GPL-3.0 License - the details are at [LICENSE.md](https://raw.githubusercontent.com/clFaster/SVNPathCopy/master/LICENSE)
