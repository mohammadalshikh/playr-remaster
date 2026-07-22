# Unity Setup Guide - I AM PLAYR Remaster

## Prerequisites
- [x] Windows PC with 20GB+ free space
- [x] Unity Hub installed (via winget)
- [ ] Unity 2022.3 LTS installed
- [ ] Android Build Support modules installed

## Unity 2022.3 LTS Installation

### Step 1: Install Unity Editor
1. Open Unity Hub (should be running)
2. Navigate to **Installs** tab
3. Click **Install Editor** button
4. Select **Unity 2022.3 LTS** (Latest Long-Term Support)
   - Version: 2022.3.x (latest patch)
   - Recommended for production projects

### Step 2: Select Modules
Required modules for Android development:
- ✅ **Android Build Support**
- ✅ **Android SDK & NDK Tools**
- ✅ **OpenJDK** (Java Development Kit)

Optional but recommended:
- **Documentation** (offline Unity docs)
- **Microsoft Visual Studio Community** (or use VS Code)

### Step 3: Installation
- Click **Continue** and accept terms
- Installation size: ~8-10 GB
- Time: 15-30 minutes depending on internet speed
- Unity Hub will download and install automatically

### Step 4: Verify Installation
After installation:
```powershell
# Check Unity installation
Get-ChildItem "C:\Program Files\Unity\Hub\Editor" -Directory
# Should show: 2022.3.x folder
```

## Android SDK Setup

Unity's Android module includes:
- Android SDK (API levels 30+)
- Android NDK (Native Development Kit)
- OpenJDK 11

If you need to customize paths:
1. Unity Hub → Preferences → External Tools
2. Verify Android SDK/NDK/JDK paths

## License Activation

Unity Personal (Free) License:
1. First launch of Unity Editor will prompt for license
2. Select **Unity Personal**
3. Sign in with Unity ID (create if needed)
4. Accept terms for free personal use

## Next Steps

After Unity installation completes:
1. Return to this terminal/IDE
2. Proceed with project creation
3. Import and migrate source code
4. Configure Android build settings

## Troubleshooting

### Unity Hub won't start
```powershell
# Reinstall via winget
winget uninstall Unity.UnityHub
winget install Unity.UnityHub
```

### Android modules fail to install
- Check internet connection
- Retry module installation from Unity Hub
- Manually download Android SDK if needed

### Disk space issues
- Minimum 20GB required
- Unity Editor: ~8GB
- Project + assets: ~2-5GB
- Android SDK: ~5-8GB
- Build cache: ~2-5GB

## References
- [Unity 2022.3 LTS Release Notes](https://unity.com/releases/editor/whats-new/2022.3.0)
- [Android Build Setup](https://docs.unity3d.com/Manual/android-sdksetup.html)
- [Unity Personal License Terms](https://unity.com/legal/terms-of-service)
