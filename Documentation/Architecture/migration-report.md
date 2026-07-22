# Unity API Migration Report
Generated: 2026-07-21 03:02:05

## Summary
- **Total Files Scanned:** 330
- **Files with Issues:** 49
- **Total Issues Found:** 27
- **Critical Issues:** 6
- **High Priority:** 12
- **Medium Priority:** 9

## Issues by API Pattern

### Medium: GUIText
- **Occurrences:** 19
- **Files Affected:** 3
- **Replacement:** `TextMeshPro or UI Text`
- **Required Namespace:** `TMPro or UnityEngine.UI`
- **Description:** Legacy GUI component

#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\ScreenQuad.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\iTween.cs` (17 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\GameTypeMenuManager.cs` (1 occurrences)

### Medium: GUITexture
- **Occurrences:** 12
- **Files Affected:** 3
- **Replacement:** `UI Image`
- **Required Namespace:** `UnityEngine.UI`
- **Description:** Legacy GUI component

#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\ScreenQuad.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\iTween.cs` (10 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\GameTypeMenuManager.cs` (1 occurrences)

### Medium: \.animation\.
- **Occurrences:** 1
- **Files Affected:** 1
- **Replacement:** `.GetComponent<Animation>().`
- **Required Namespace:** `UnityEngine`
- **Description:** Animation access modernization

#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\CameraScrollController.cs` (1 occurrences)

### High: \.renderer\.
- **Occurrences:** 130
- **Files Affected:** 34
- **Replacement:** `.GetComponent<Renderer>().`
- **Required Namespace:** `UnityEngine`
- **Description:** Component access modernization

#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\NextGameView.cs` (7 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\NewGamePopup.cs` (3 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\PlayAgainCoins.cs` (2 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\PassingTarget.cs` (2 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MatchFitnessView.cs` (8 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MatchFitnessLocation.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MinigameGUI.cs` (5 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\Minigame3DText.cs` (2 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\PopupText3D.cs` (2 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\TickerTapeText.cs` (2 occurrences)
- ... and 24 more files

### High: \.rigidbody\.
- **Occurrences:** 24
- **Files Affected:** 3
- **Replacement:** `.GetComponent<Rigidbody>().`
- **Required Namespace:** `UnityEngine`
- **Description:** Rigidbody access modernization

#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\SoccerBall.cs` (12 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\Minigame.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\iTween.cs` (11 occurrences)

### High: \.active\s*=
- **Occurrences:** 17
- **Files Affected:** 7
- **Replacement:** `.SetActive()`
- **Required Namespace:** `UnityEngine`
- **Description:** GameObject activation API change

#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\SoccerBall.cs` (2 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\Stadium.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\WeatherManager.cs` (3 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\Minigame.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\AssetsManager.cs` (2 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\DisableOnHit.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\IntroView.cs` (7 occurrences)

### High: \.collider\.
- **Occurrences:** 11
- **Files Affected:** 7
- **Replacement:** `.GetComponent<Collider>().`
- **Required Namespace:** `UnityEngine`
- **Description:** Collider access modernization

#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\SettingsView.cs` (4 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\View.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\WallTarget.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MatchFitnessView.cs` (2 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\Bucket.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\GenericButton.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MapLocation.cs` (1 occurrences)

### Critical: new WWW\(
- **Occurrences:** 6
- **Files Affected:** 4
- **Replacement:** `UnityWebRequest`
- **Required Namespace:** `UnityEngine.Networking`
- **Description:** Network API modernization

#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\LeaderboardView.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\WebIntegrationManager.cs` (3 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\AssetBundleManager.cs` (1 occurrences)
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\LeaderBoardManager.cs` (1 occurrences)

### Critical: Application.LoadLevel
- **Occurrences:** 2
- **Files Affected:** 1
- **Replacement:** `SceneManager.LoadScene`
- **Required Namespace:** `UnityEngine.SceneManagement`
- **Description:** Scene loading API modernization

#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\GameData.cs` (2 occurrences)

## Migration Checklist

### Phase 1: Critical APIs (Must Fix)
- [ ] Migrate `new WWW\(` â†’ `UnityWebRequest` (6 occurrences)
- [ ] Migrate `Application.LoadLevel` â†’ `SceneManager.LoadScene` (2 occurrences)
### Phase 2: High Priority
- [ ] Migrate `\.renderer\.` â†’ `.GetComponent<Renderer>().` (130 occurrences)
- [ ] Migrate `\.rigidbody\.` â†’ `.GetComponent<Rigidbody>().` (24 occurrences)
- [ ] Migrate `\.active\s*=` â†’ `.SetActive()` (17 occurrences)
- [ ] Migrate `\.collider\.` â†’ `.GetComponent<Collider>().` (11 occurrences)
### Phase 3: Medium Priority
- [ ] Migrate `GUIText` â†’ `TextMeshPro or UI Text` (19 occurrences)
- [ ] Migrate `GUITexture` â†’ `UI Image` (12 occurrences)
- [ ] Migrate `\.animation\.` â†’ `.GetComponent<Animation>().` (1 occurrences)

## Recommended Migration Strategy

1. **Backup**: Ensure all source files are in version control
2. **Phase 1**: Fix critical API issues (required for compilation in Unity 2022)
3. **Phase 2**: Address high priority issues (major functionality)
4. **Phase 3**: Handle medium priority issues (polish and optimization)
5. **Test**: After each phase, test in Unity Editor
6. **Build**: Verify Android build after all migrations

## Automated Migration Script

To automatically apply some fixes, run:
```powershell
.\api-migration-apply.ps1 -SourcePath "c:\Users\konst\OneDrive\Desktop\playr-remaster\Source" -DryRun
```

Review changes, then apply:
```powershell
.\api-migration-apply.ps1 -SourcePath "c:\Users\konst\OneDrive\Desktop\playr-remaster\Source" -Apply
```

## Next Steps

1. Review this report
2. Create feature branch for migration
3. Apply automated fixes for straightforward replacements
4. Manually review and fix complex patterns
5. Add required using statements (SceneManagement, Networking, etc.)
6. Import code into Unity project
7. Resolve compilation errors
8. Test each game mode and minigame

---
*Generated by Unity API Migration Analyzer*
