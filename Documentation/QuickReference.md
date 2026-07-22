# Quick Reference - Commands Cheat Sheet

## Analysis Commands

### Run API Migration Analysis
```powershell
# Preview obsolete Unity APIs in source code
powershell -ExecutionPolicy Bypass -File "Tools\Migration\api-migration-analyze.ps1"
```

### Run Legacy Integration Analysis
```powershell
# Identify Facebook, Tapjoy, and other legacy integrations
powershell -ExecutionPolicy Bypass -File "Tools\Migration\legacy-integration-removal.ps1" -GenerateStubs
```

## Migration Commands

### Apply API Migrations (Preview)
```powershell
# Dry run - see what would change without modifying files
cd Tools\Migration
powershell -ExecutionPolicy Bypass -File .\api-migration-apply.ps1 -DryRun
```

### Apply API Migrations (Execute)
```powershell
# Apply changes with automatic backup
powershell -ExecutionPolicy Bypass -File .\api-migration-apply.ps1 -Apply
```

## Search Commands

### Find Remaining Obsolete APIs
```powershell
# Search for obsolete Unity API patterns
cd Source
Select-String -Pattern "\.renderer\.|\.rigidbody\.|Application\.LoadLevel" -Include "*.cs" -Recurse
```

### Find Legacy Integrations
```powershell
# Search for Facebook, Tapjoy references
cd Source
Select-String -Pattern "Facebook|Tapjoy|Flurry" -Include "*.cs" -Recurse
```

## Project Setup Commands

### Create Unity Project Folders
```powershell
$unityProject = "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster"
$scriptsPath = "$unityProject\Assets\_Project\Scripts"

# Create folder structure
New-Item -ItemType Directory -Path "$scriptsPath\Core" -Force
New-Item -ItemType Directory -Path "$scriptsPath\Minigames" -Force
New-Item -ItemType Directory -Path "$scriptsPath\UI" -Force
New-Item -ItemType Directory -Path "$scriptsPath\Managers" -Force
New-Item -ItemType Directory -Path "$scriptsPath\Stubs" -Force
```

### Copy Source Files to Unity
```powershell
$sourceDir = "c:\Users\konst\OneDrive\Desktop\playr-remaster\Source"
$scriptsPath = "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project\Scripts"

# Copy stub classes
Copy-Item "$sourceDir\Stubs\*" "$scriptsPath\Stubs\" -Recurse -Force

# Copy all source files (adjust as needed)
Copy-Item "$sourceDir\*.cs" "$scriptsPath\" -Force
```

### Copy Assets to Unity
```powershell
$assetsSource = "c:\Users\konst\OneDrive\Desktop\playr-remaster\Assets\Unity"
$assetsTarget = "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project"

# Copy textures
Copy-Item "$assetsSource\Data\Texture2D" "$assetsTarget\Textures\" -Recurse -Force

# Copy audio
Copy-Item "$assetsSource\Data\AudioClip" "$assetsTarget\Audio\" -Recurse -Force
```

## Unity Editor Commands

### Check Unity Installation
```powershell
# Verify Unity Hub
Test-Path "C:\Program Files\Unity Hub\Unity Hub.exe"

# List installed Unity versions
Get-ChildItem "C:\Program Files\Unity\Hub\Editor" -Directory
```

### Launch Unity Hub
```powershell
Start-Process "C:\Program Files\Unity Hub\Unity Hub.exe"
```

## Android Build Commands

### Check ADB
```powershell
# Check if ADB is available
adb version
```

### Install APK to Device
```powershell
# Install built APK
adb install path\to\iamplayr.apk

# View Unity logs
adb logcat | Select-String "Unity"

# Clear logs
adb logcat -c
```

## Backup Commands

### Create Source Backup
```powershell
# Manual backup before changes
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
Copy-Item "Source" "Source_Backup_$timestamp" -Recurse -Force
```

### List Backups
```powershell
# Show all backup folders
Get-ChildItem -Directory -Filter "Source_Backup_*" | Select-Object Name, CreationTime
```

## File Statistics

### Count Source Files
```powershell
# Count C# files
(Get-ChildItem Source -Filter "*.cs" -Recurse).Count

# Total lines of code
(Get-ChildItem Source -Filter "*.cs" -Recurse | Get-Content | Measure-Object -Line).Lines
```

### Find Large Files
```powershell
# Find largest source files
Get-ChildItem Source -Filter "*.cs" -Recurse | 
    Sort-Object Length -Descending | 
    Select-Object Name, @{Name="Size(KB)";Expression={[math]::Round($_.Length/1KB,2)}} -First 20
```

## Git Commands (if using Git)

### Initialize Repository
```powershell
git init
git add .
git commit -m "Initial commit - source code analyzed"
```

### Create Feature Branch
```powershell
# For migrations
git checkout -b feature/api-migration

# For legacy removal
git checkout -b feature/remove-legacy-integrations
```

### View Changes
```powershell
# Show modified files
git status

# Show diff
git diff

# Show diff for specific file
git diff Source/GameData.cs
```

## Troubleshooting Commands

### PowerShell Execution Policy
```powershell
# Check current policy
Get-ExecutionPolicy

# Set to allow scripts (admin required)
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser

# Run single script bypassing policy
powershell -ExecutionPolicy Bypass -File script.ps1
```

### Check Disk Space
```powershell
# Check available space
Get-PSDrive C | Select-Object Used, Free
```

### Kill Stuck Unity Process
```powershell
# Find Unity processes
Get-Process | Where-Object {$_.Name -like "*Unity*"}

# Kill specific process
Stop-Process -Name "Unity" -Force
```

## Useful Aliases

```powershell
# Add to PowerShell profile for convenience
Set-Alias unify-migrate "c:\Users\konst\OneDrive\Desktop\playr-remaster\Tools\Migration\api-migration-apply.ps1"
Set-Alias unify-analyze "c:\Users\konst\OneDrive\Desktop\playr-remaster\Tools\Migration\api-migration-analyze.ps1"

# Create profile if doesn't exist
if (!(Test-Path $PROFILE)) { New-Item -Path $PROFILE -Type File -Force }

# Edit profile
notepad $PROFILE
```

## Documentation Shortcuts

```powershell
# Open key documents in VS Code
code README.md
code Documentation\Setup\ProjectInitialization.md
code Documentation\Architecture\migration-report.md
code Documentation\Architecture\legacy-integrations.md
```

## One-Liner Quick Actions

```powershell
# Full analysis pipeline
powershell -ExecutionPolicy Bypass -File "Tools\Migration\api-migration-analyze.ps1"; powershell -ExecutionPolicy Bypass -File "Tools\Migration\legacy-integration-removal.ps1" -GenerateStubs

# Backup and migrate
$backup = "Source_Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"; Copy-Item "Source" $backup -Recurse; powershell -ExecutionPolicy Bypass -File "Tools\Migration\api-migration-apply.ps1" -Apply

# Count issues remaining
(Select-String -Path "Source\*.cs" -Pattern "\.renderer\.|\.rigidbody\.|GUIText|GUITexture" -Recurse).Count
```

---

## File Locations Quick Reference

| Item | Path |
|------|------|
| **Source Code** | `Source\` |
| **Unity Project** | `UnityProject\IAMPlayrRemaster\` |
| **Migration Tools** | `Tools\Migration\` |
| **Documentation** | `Documentation\` |
| **Assets** | `Assets\Unity\` |
| **Dependencies** | `Dependencies\Managed\` |
| **Stub Classes** | `Source\Stubs\` |

---

## Important Notes

- Always run analysis scripts before applying changes
- Use `-DryRun` flag first to preview migrations
- Automatic backups are created when using `-Apply`
- Check migration reports in `Documentation\Architecture\`
- Stub classes prevent null reference errors during transition
- Use PowerShell ISE or VS Code for better script debugging

---

*Quick reference for I AM PLAYR remaster project*
*Keep this document open during development*
