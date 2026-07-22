# Project Initialization Guide
# I AM PLAYR Remaster - Step-by-Step Setup

## Current Status

### ✅ Completed
- Unity Hub installed (v3.19.5)
- Project structure created
- Migration tools developed
- Source code analysis complete:
  - 330 C# source files analyzed
  - 49 files with obsolete Unity APIs
  - 186 total API migrations needed
  - 24 files with Facebook integration
  - 8 files with Tapjoy ads
  - Stub classes generated

### 🔄 In Progress
- Unity 2022.3 LTS installation (manual step via Unity Hub)

### 📋 Next Steps
1. Complete Unity Editor installation
2. Create Unity project
3. Apply API migrations
4. Import source code
5. Configure Android build

---

## Phase 1: Unity Setup (30-45 minutes)

### 1.1 Install Unity Editor
**Unity Hub should be open. Follow these steps:**

1. In Unity Hub, go to **Installs** tab
2. Click **Install Editor** button
3. Select **Unity 2022.3 LTS** (latest Long-Term Support version)
4. In Add modules screen, select:
   - ✅ Android Build Support
   - ✅ Android SDK & NDK Tools
   - ✅ OpenJDK (Java Development Kit)
   - ✅ Documentation (optional but recommended)
5. Click **Continue** and accept license agreements
6. Wait for installation (8-10 GB download)

### 1.2 Verify Installation
After installation completes, verify in PowerShell:

```powershell
# Check Unity installation
Get-ChildItem "C:\Program Files\Unity\Hub\Editor" -Directory

# Should show a folder like: 2022.3.x
```

### 1.3 Activate License
First time launching Unity Editor:
1. Choose **Unity Personal** (free)
2. Sign in with Unity ID (create account if needed)
3. Accept terms for personal use

---

## Phase 2: Source Code Migration (2-3 hours)

### 2.1 Review Analysis Reports
Open and review generated reports:

```powershell
# Open migration report
code Documentation\Architecture\migration-report.md

# Open legacy integrations report
code Documentation\Architecture\legacy-integrations.md
```

**Key Findings:**
- **Critical APIs**: 0 (SceneManager calls already exist or are minimal)
- **High Priority**: 154 occurrences
  - `.renderer.` → `GetComponent<Renderer>().` (130 times)
  - `.rigidbody.` → `GetComponent<Rigidbody>().` (24 times)
- **Medium Priority**: 32 occurrences
  - `GUIText` → UI Text (19 times)
  - `GUITexture` → UI Image (12 times)
  - `.animation.` (1 time)

### 2.2 Apply Automated Migrations
Run the migration scripts:

```powershell
# Preview changes (safe, no modifications)
cd Tools\Migration
powershell -ExecutionPolicy Bypass -File .\api-migration-apply.ps1 -DryRun

# If preview looks good, apply changes
powershell -ExecutionPolicy Bypass -File .\api-migration-apply.ps1 -Apply

# Backup will be created automatically in Source_Backup_[timestamp]
```

### 2.3 Manual Fixes Required
After automated migration, manually fix these patterns:

#### Component Access (High Priority)
Search and replace in Source folder:

- **Pattern**: `.renderer.`
  - **Replace**: `.GetComponent<Renderer>().`
  - **Add using**: `UnityEngine` (already present)

- **Pattern**: `.rigidbody.`
  - **Replace**: `.GetComponent<Rigidbody>().`
  
- **Pattern**: `.collider.`
  - **Replace**: `.GetComponent<Collider>().`

- **Pattern**: `.audio.`
  - **Replace**: `.GetComponent<AudioSource>().`

- **Pattern**: `.animation.`
  - **Replace**: `.GetComponent<Animation>().`

#### Legacy GUI Components (Medium Priority)
These require scene-level changes in Unity:

- `GUIText` → Convert to Unity UI Text or TextMeshPro
- `GUITexture` → Convert to Unity UI Image

---

## Phase 3: Unity Project Creation (1 hour)

### 3.1 Create New Unity Project
In Unity Hub:

1. Click **New Project** button
2. Select **3D (URP)** template (Universal Render Pipeline for modern graphics)
   - Or use **3D** template if original game used standard pipeline
3. Project Settings:
   - **Project Name**: `IAMPlayrRemaster`
   - **Location**: `c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject`
4. Click **Create Project**
5. Wait for Unity to initialize (2-3 minutes)

### 3.2 Configure Project Settings

Once Unity opens, configure project:

#### Build Settings
1. **File → Build Settings**
2. Select **Android** platform
3. Click **Switch Platform**
4. Player Settings:
   - **Company Name**: Your company name
   - **Product Name**: I AM PLAYR
   - **Package Name**: `com.yourcompany.iamplayr`
   - **Minimum API Level**: Android 7.0 (API 24) or higher
   - **Target API Level**: Latest (API 33+)

#### Quality Settings
1. **Edit → Project Settings → Quality**
2. Adjust quality presets for mobile:
   - Use **Medium** or **High** for default

#### Player Settings
1. **Edit → Project Settings → Player**
2. **Android Tab**:
   - **Multithreaded Rendering**: ✅ Enabled
   - **Auto Graphics API**: ✅ Enabled
   - **Scripting Backend**: IL2CPP (for better performance)
   - **API Compatibility Level**: .NET Standard 2.1

### 3.3 Project Folder Structure
Create folders in Unity Project:

```
IAMPlayrRemaster/
├── Assets/
│   ├── _Project/
│   │   ├── Scripts/
│   │   │   ├── Core/          # GameData, boot flow
│   │   │   ├── Minigames/     # Minigame implementations
│   │   │   ├── UI/            # UI controllers
│   │   │   ├── Managers/      # Audio, difficulty, etc.
│   │   │   └── Stubs/         # Legacy integration stubs
│   │   ├── Scenes/
│   │   ├── Prefabs/
│   │   ├── Materials/
│   │   ├── Textures/
│   │   └── Audio/
│   ├── Plugins/               # Third-party DLLs
│   └── StreamingAssets/       # Asset bundles, data
```

---

## Phase 4: Source Code Import (2-3 hours)

### 4.1 Prepare Source Files
Before importing, ensure migrations are complete:

```powershell
# Verify migrations applied
cd Source
Select-String -Pattern "\.renderer\.|\.rigidbody\.|Application\.LoadLevel" -Include "*.cs" -Recurse
# Should return minimal or no results
```

### 4.2 Import Core Scripts
Copy migrated source files to Unity project:

```powershell
# Copy core game scripts
$sourceDir = "c:\Users\konst\OneDrive\Desktop\playr-remaster\Source"
$unityScripts = "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project\Scripts"

# Create script directories
New-Item -ItemType Directory -Path "$unityScripts\Core" -Force
New-Item -ItemType Directory -Path "$unityScripts\Minigames" -Force
New-Item -ItemType Directory -Path "$unityScripts\UI" -Force
New-Item -ItemType Directory -Path "$unityScripts\Managers" -Force

# Copy stub classes first
Copy-Item "$sourceDir\Stubs\*" "$unityScripts\Stubs\" -Recurse -Force

# Copy core files (GameData, boot flow, etc.)
Copy-Item "$sourceDir\GameData.cs" "$unityScripts\Core\" -Force
Copy-Item "$sourceDir\BootHelper.cs" "$unityScripts\Core\" -Force
# ... continue for other core files
```

**Recommended Import Order:**
1. **Stub classes** (Facebook, Tapjoy stubs)
2. **Core systems** (GameData, enums, constants)
3. **Managers** (AudioManager, AssetBundleManager, etc.)
4. **Minigames** (MinigameChallenge, implementations)
5. **UI** (Views, controllers)
6. **Utilities** (helpers, extensions)

### 4.3 Resolve Compilation Errors
In Unity Console, fix errors:

1. **Missing namespaces**: Add required `using` statements
2. **Missing assets**: Create placeholder assets/prefabs
3. **Legacy API calls**: Manually update remaining obsolete APIs
4. **Facebook/Tapjoy calls**: Verify stubs are properly referenced

---

## Phase 5: Asset Import (1-2 hours)

### 5.1 Import Unity Assets
Copy assets from decompiled APK:

```powershell
$assetsSource = "c:\Users\konst\OneDrive\Desktop\playr-remaster\Assets\Unity"
$assetsTarget = "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project"

# Copy textures
Copy-Item "$assetsSource\Data\Texture2D" "$assetsTarget\Textures\" -Recurse -Force

# Copy audio
Copy-Item "$assetsSource\Data\AudioClip" "$assetsTarget\Audio\" -Recurse -Force

# Copy scenes (will need reconstruction)
Copy-Item "$assetsSource\Scenes" "$assetsTarget\Scenes\" -Recurse -Force
```

### 5.2 Reconstruct Scenes
Decompiled scenes may not import perfectly:

1. Open each scene file
2. Verify scene hierarchy
3. Reconnect broken prefab references
4. Reassign materials and textures
5. Test scene functionality

**Key Scenes:**
- `MAP.unity` - Main menu/world map
- `GAME_TYPE_MENU.unity` - Game mode selection
- `GAME_LEVEL.unity` - Minigame gameplay

---

## Phase 6: Testing & Build (2-4 hours)

### 6.1 In-Editor Testing
Test each component:

- [ ] Game boots without errors
- [ ] Main menu loads
- [ ] Can select game mode
- [ ] Minigames load and run
- [ ] No Facebook/Tapjoy errors
- [ ] Audio plays correctly
- [ ] UI displays properly

### 6.2 Android Build
Create test build:

1. **File → Build Settings**
2. Ensure Android platform selected
3. Click **Build**
4. Save APK to `c:\Users\konst\OneDrive\Desktop\playr-remaster\Builds\`

### 6.3 Device Testing
Install and test on Android device/emulator:

```powershell
# Install via ADB
adb install path\to\iamplayr.apk

# View logs
adb logcat | Select-String "Unity"
```

**Test checklist:**
- [ ] App launches successfully
- [ ] Touch controls work
- [ ] Graphics render correctly
- [ ] Audio plays
- [ ] All minigames playable
- [ ] No crashes
- [ ] Performance acceptable (30+ FPS)

---

## Phase 7: Polish & Optimization (ongoing)

### 7.1 Code Quality
- Remove unused imports
- Add XML documentation
- Refactor complex methods
- Add error handling

### 7.2 Performance
- Profile using Unity Profiler
- Optimize draw calls
- Reduce texture memory
- Optimize audio compression

### 7.3 Modern Features
Consider adding:
- Local high score system (replace Facebook leaderboards)
- Achievement system (local tracking)
- Settings menu (audio, graphics, controls)
- Tutorial mode
- Better UI with modern design

---

## Troubleshooting

### Unity Hub Issues
```powershell
# Reinstall Unity Hub
winget uninstall Unity.UnityHub
winget install Unity.UnityHub
```

### Compilation Errors
1. Check migration reports for unfixed APIs
2. Ensure all using statements added
3. Verify stub classes imported
4. Check Unity version compatibility

### Asset Import Issues
1. Verify asset paths are correct
2. Check file permissions
3. Re-import assets if corrupted
4. Recreate prefabs if references broken

### Build Failures
1. Check Android SDK path
2. Verify NDK installed
3. Update build tools
4. Check minimum API level

---

## Resources

### Generated Reports
- [API Migration Report](../Documentation/Architecture/migration-report.md)
- [Legacy Integrations Report](../Documentation/Architecture/legacy-integrations.md)

### Unity Documentation
- [Unity 2022.3 LTS Manual](https://docs.unity3d.com/2022.3/Documentation/Manual/)
- [Android Build Setup](https://docs.unity3d.com/Manual/android-sdksetup.html)
- [Script Upgrade Guide](https://docs.unity3d.com/Manual/ScriptingRuntimeUpgrade.html)

### Migration Scripts
- `Tools/Migration/api-migration-analyze.ps1` - Analyze obsolete APIs
- `Tools/Migration/api-migration-apply.ps1` - Apply automated fixes
- `Tools/Migration/legacy-integration-removal.ps1` - Remove social integrations

---

## Timeline Estimate

| Phase | Task | Estimated Time |
|-------|------|----------------|
| 1 | Unity Setup | 30-45 min |
| 2 | Source Migration | 2-3 hours |
| 3 | Project Creation | 1 hour |
| 4 | Code Import | 2-3 hours |
| 5 | Asset Import | 1-2 hours |
| 6 | Testing & Build | 2-4 hours |
| 7 | Polish | Ongoing |
| **Total** | **Initial Setup** | **9-14 hours** |

Quality over speed - take time to test each phase thoroughly!

---

## Next Immediate Steps

1. **Wait for Unity 2022.3 LTS installation to complete in Unity Hub**
2. Review generated migration reports
3. Run API migration scripts in dry-run mode
4. Prepare for Unity project creation

**Once Unity is installed, return here and proceed with Phase 2!**
