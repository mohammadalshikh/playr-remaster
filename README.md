# I AM PLAYR - Remaster Project

Modernizing the 2016 Android soccer/football game for current devices.

## Project Overview

**Original Game:** I AM PLAYR (2016)  
**Original APK:** i-am-playr-2-9.apk  
**Goal:** Rebuild as modern Unity project for Android, offline-only (no Facebook/social features)

## Project Structure

```
playr-remaster/
├── Source/               # Decompiled C# source code from AssetRipper
│   ├── *.cs             # All game scripts (336 files)
│   └── Assembly-CSharp.csproj
├── Assets/              # Game assets extracted from APK
│   ├── Unity/           # Unity-specific assets
│   │   ├── Scenes/      # Unity scene files (level0, level1, level2)
│   │   └── Data/        # Asset bundles, shared assets, textures
│   └── Android/         # Android resources (icons, drawables)
├── Dependencies/        # Required DLLs
│   └── Managed/         # Unity managed DLLs (UnityEngine, JsonFx, etc.)
└── Documentation/       # Project docs and notes
```

## Current Status

### ✅ Completed
- Source code decompilation via AssetRipper
- Build verification: `dotnet build Assembly-CSharp.csproj` succeeds (0 errors)
- Fixed MinigameWall.cs compiler error
- Mapped game architecture and boot flow
- Cataloged legacy integration points (Facebook, Tapjoy, etc.)

### 🎯 Core Game Architecture

**Boot Flow:**
1. GameData.Awake() → DontDestroyOnLoad, initialize singleton
2. Load MAP scene (main menu)
3. GAME_TYPE_MENU → select location/game type
4. GAME_LEVEL → play minigame

**Minigame System:**
- Base class: `MinigameChallenge` (abstract)
- Implementations: `MinigameWall`, `MinigamePassing`, `MinigameShooting`
- Controller: `Minigame.cs` manages gameplay loop

### 🚧 To Do
1. Install Unity LTS (2022/2023) with Android Build Support
2. Create modern Unity project
3. Stub/remove legacy integrations:
   - Facebook (FacebookHandler, FacebookManager, FacebookAndroid)
   - Tapjoy (TapjoyPlugin, TapjoyManager)
   - WebIntegration, Flurry, P31RestKit
4. Modernize obsolete Unity APIs:
   - `Application.LoadLevel` → `SceneManager.LoadScene`
   - `.active` → `SetActive()`
   - `rigidbody` → `GetComponent<Rigidbody>()`
   - `renderer.material` → `GetComponent<Renderer>().material`
   - `WWW` → `UnityWebRequest`
5. Port core gameplay scripts
6. Import assets and reconstruct scenes
7. Build and test on Android emulator/device

## Technical Details

**Target Framework:** Unity 2022 LTS or 2023 LTS  
**Platform:** Android (modern API levels)  
**Scripting:** C# (.NET 4.x or .NET Standard 2.1)  
**Build System:** Unity Editor + Android SDK  

**Key Dependencies (from original):**
- UnityEngine.dll
- JsonFx.Json.dll (JSON parsing)
- P31RestKit.dll (REST API - can be removed/replaced)

**Legacy Integrations to Remove:**
- Facebook SDK (328 matches across 33 files)
- Tapjoy Ads
- Web Integration APIs
- Flurry Analytics

## Notes

- Original game uses Unity 4.x era APIs (2013-2014)
- Decompiled code builds successfully with minimal fixes
- Most warnings are Unity serialization related (normal for Unity projects)
- Game architecture is well-structured with clear separation of concerns
- Offline-first design: all Facebook/social features will be stubbed out

## Original APK Info

**Package:** com.we.amplayr  
**Version:** 2.9  
**Min SDK:** Android 2.3 (API 9)  
**Build Date:** August 16, 2013  
**Size:** 22MB (APK), 59MB (extracted)

## Getting Started (Windows)

1. Install Unity Hub
2. Install Unity 2022 LTS or 2023 LTS with Android Build Support
3. Create new Unity project (3D, Android platform)
4. Import Source/ scripts
5. Import Assets/Unity/ data files
6. Configure build settings for Android
7. Test in Unity Editor before building APK

## Repository

This project can be pushed to GitHub for cross-platform development.  
**Note:** Large asset files may need Git LFS or external hosting.
