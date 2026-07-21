# Build Status & Known Issues

## Current Build Status

✅ **Source Code Compiles:** 0 errors, 336 warnings (Unity serialization - normal)

```bash
cd Source/
dotnet build Assembly-CSharp.csproj
# Result: Build succeeded with 0 errors
```

---

## Fixed Issues

### ✅ MinigameWall.cs (Line 796)
**Error:** CS0165 - Use of unassigned local variable 'num'  
**Fix:** Removed decompiler artifact, simplified switch case to direct lighting call  
**Status:** Fixed and compiling

---

## Known Issues Requiring Unity Migration

### 1. Obsolete Unity APIs (Widespread)

#### Scene Management
❌ `Application.LoadLevel(SCENES scene)` → ✅ `SceneManager.LoadScene()`  
❌ `Application.loadedLevel` → ✅ `SceneManager.GetActiveScene().buildIndex`

**Files affected:** GameData.cs (lines 1030+), likely others

#### GameObject APIs  
❌ `.active` property → ✅ `.SetActive(bool)`  
❌ `gameObject.active = true` → ✅ `gameObject.SetActive(true)`

**Files affected:** Multiple (common pattern)

#### Component Access
❌ `rigidbody.velocity` → ✅ `GetComponent<Rigidbody>().velocity`  
❌ `renderer.material` → ✅ `GetComponent<Renderer>().material`  
❌ `collider.bounds` → ✅ `GetComponent<Collider>().bounds`

**Files affected:** Physics and rendering code throughout

#### Deprecated UI
❌ `guiTexture`, `guiText` → ✅ Use Unity UI (Canvas, Image, Text)  
**Files affected:** Legacy GUI code

#### Networking
❌ `WWW` class → ✅ `UnityWebRequest`  
**Files affected:** Any web/API calls

---

### 2. Legacy Social Integrations (328 matches across 33 files)

#### Facebook Integration
- FacebookHandler.cs
- FacebookManager.cs  
- FacebookAndroid.cs
- IntroView.cs (login prompts)
- Various score/achievement sharing

**Action Required:** Stub out or remove completely

#### Tapjoy Ads
- TapjoyPlugin.cs
- TapjoyManager.cs
- GameTypeMenuManager.cs (ad hiding)

**Action Required:** Remove ad integration code

#### Other Integrations
- WebIntegrationManager.cs
- FlurryAndroid.cs (analytics)
- P31RestKit.dll (REST API)

**Action Required:** Remove or replace with modern equivalents

---

### 3. Dependencies to Modernize

#### JsonFx.Json (Legacy)
Current: JsonFx.Json.dll  
Modern alternatives:
- Unity's built-in JsonUtility
- Newtonsoft.Json (Json.NET)
- System.Text.Json (.NET Core)

#### P31RestKit (Outdated)
Current: P31RestKit.dll  
Modern alternatives:
- UnityWebRequest (built-in)
- UniTask + async/await patterns
- Modern HTTP clients

---

## Migration Strategy

### Phase 1: Clean Build Environment
✅ Organize source code  
✅ Extract and organize assets  
✅ Document architecture  
⬜ Set up Unity project

### Phase 2: Stub Legacy Code
⬜ Create no-op stubs for Facebook APIs  
⬜ Create no-op stubs for Tapjoy APIs  
⬜ Remove/stub other integrations  

### Phase 3: API Modernization
⬜ Replace Application.LoadLevel with SceneManager  
⬜ Update component access patterns  
⬜ Remove deprecated GUI code  
⬜ Update networking to UnityWebRequest  

### Phase 4: Asset Import
⬜ Import Unity scenes (level0, level1, level2)  
⬜ Import shared assets and asset bundles  
⬜ Reconstruct scene hierarchy  
⬜ Link prefabs and references  

### Phase 5: Testing & Validation
⬜ Test in Unity Editor  
⬜ Verify core gameplay loop  
⬜ Build Android APK  
⬜ Test on emulator/device  

---

## Build Configuration

**Original Target:** .NET 3.5 (Unity 4.x era)  
**Recommended Target:** .NET Standard 2.1 or .NET 4.x (Unity 2020+)

**Dependencies to Bundle:**
- UnityEngine.dll (will come from Unity installation)
- JsonFx.Json.dll → replace with modern JSON library
- P31RestKit.dll → remove, use UnityWebRequest

---

## Warnings Summary

Current build has 336 warnings, primarily:
- CS0649: Field never assigned (Unity serialized fields - normal)
- CS0618: Member is obsolete (Unity API deprecations - will fix during migration)
- CS0414: Field assigned but never used (Unity serialization pattern - normal)

**Action:** These warnings are expected for decompiled Unity code and will be addressed during Unity project setup.

---

## Testing Checklist (Post-Migration)

- [ ] Game boots to main menu (MAP scene)
- [ ] Can select location and game type (GAME_TYPE_MENU)
- [ ] Can launch minigame (GAME_LEVEL)
- [ ] Wall minigame playable
- [ ] Passing minigame playable
- [ ] Shooting minigame playable
- [ ] Score tracking works
- [ ] Lives/game over logic works
- [ ] Can return to menu
- [ ] Android build succeeds
- [ ] Runs on modern Android device (API 30+)

---

## Risk Assessment

**Low Risk:**
- Core gameplay architecture is solid
- Minigame system well-structured
- Clear separation of concerns

**Medium Risk:**
- Extensive use of obsolete APIs (fixable, just time-consuming)
- Asset bundle compatibility (Unity version changes)
- Scene reconstruction (references may break)

**High Risk:**
- Dependencies on removed social features (needs careful stubbing)
- Unknown dependencies in asset bundles
- Potential native plugin issues (Android libraries in APK)

---

## Additional Notes

- Original game is Unity 4.x era (2013-2014)
- Target modern Unity 2022 LTS or 2023 LTS
- Focus on offline gameplay, remove all online features
- Asset bundles may need rebuilding in new Unity version
- Shaders may need updating (legacy → URP/HDRP)

**Estimated Migration Effort:** Medium (2-4 weeks for experienced Unity dev)
