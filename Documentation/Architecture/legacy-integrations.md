# Legacy Integration Analysis Report
Generated: 2026-07-21 03:03:39

## Summary
- **Total Files Scanned:** 330
- **Files with Legacy Code:** 32

## Integration Breakdown

### Tapjoy [High Priority]
- **Description:** Tapjoy advertising SDK
- **Files Affected:** 8
- **Stub Strategy:** Remove entirely or stub rewards
#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\BuyController.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\GameTypeMenuManager.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\LeaderboardView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\TapjoyDisplayAdSize.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\TapjoyManager.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\TapjoyPlugin.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\TapjoyPluginAndroid.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\TapjoyViewType.cs`

### Facebook [High Priority]
- **Description:** Facebook SDK integration
- **Files Affected:** 24
- **Stub Strategy:** Create empty FacebookHandler with no-op methods
#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\AddFriends.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\BeatenFriendPopup.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\ClubHouseLocation.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\DrillProgressView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\FacebookHandler.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\FacebookPlayer.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\FacebookPlayerView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\FBGiftsManager.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\FBHighscorePopup.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\GameData.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\HighscorePopup.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\IntroView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\InviteFriendsView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\LeaderBoardManager.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\LeaderboardView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MatchFitnessLocation.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MatchFitnessView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MatchUnlockedPopup.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\NavBarManager.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\NewWebPlayerView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\PlayerManager.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\ScreenPopup.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\SettingsView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\WebIntegrationManager.cs`

### AndroidJava [Low Priority]
- **Description:** Native Android calls
- **Files Affected:** 1
- **Stub Strategy:** Review and keep only essential calls
#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\TapjoyPluginAndroid.cs`

### Flurry [Medium Priority]
- **Description:** Flurry analytics
- **Files Affected:** 2
- **Stub Strategy:** Remove or replace with Unity Analytics
#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\GameData.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\PlayerManager.cs`

### WebIntegration [Medium Priority]
- **Description:** Web API integration
- **Files Affected:** 5
- **Stub Strategy:** Remove or replace with offline data
#### Affected Files:
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\FixtureView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MatchFitnessLocation.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MatchFitnessView.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\MatchUnlockedPopup.cs`
- `C:\Users\konst\OneDrive\Desktop\playr-remaster\Source\WebIntegrationManager.cs`

## Removal Strategy

### Phase 1: Facebook Integration
Facebook was the primary social feature. Strategy:

1. **Identify Core Facebook Files:**
   - FacebookHandler.cs
   - FacebookManager.cs
   - FacebookAndroid.cs
   - FacebookPlayer.cs
   - FBGiftsManager.cs
   - FBHighscorePopup.cs

2. **Create Stub Classes:**
   ```csharp
   // Stub FacebookHandler to prevent null reference errors
   public class FacebookHandler : MonoBehaviour
   {
       public static FacebookHandler Instance { get; private set; }
       
       void Awake() {
           Instance = this;
       }
       
       public bool IsLoggedIn() => false;
       public void Login(Action callback) => callback?.Invoke();
       public void Logout() { }
       public void GetFriends(Action<List<string>> callback) => callback?.Invoke(new List<string>());
       // ... other no-op methods
   }
   ```

3. **Remove UI Elements:**
   - Facebook login buttons
   - Friend invite panels
   - Social sharing features

4. **Replace Features:**
   - Leaderboards â†’ Local high scores
   - Friend challenges â†’ AI opponents or skip
   - Gifts â†’ Direct rewards or skip

### Phase 2: Tapjoy Ads
Remove all ad integration:

1. **Remove Files:**
   - TapjoyPlugin.cs
   - TapjoyManager.cs
   - Any Tapjoy-specific UI

2. **Replace Rewards:**
   - Tapjoy rewards â†’ Direct in-game currency
   - Ad-based energy â†’ Free energy or time-based refill

3. **Clean Dependencies:**
   - Remove Tapjoy DLLs from Dependencies/

### Phase 3: Analytics & Web
Remove tracking and server communication:

1. **Flurry Analytics:**
   - Stub out tracking calls
   - Or remove entirely (no gameplay impact)

2. **Web Integration:**
   - Remove P31RestKit references
   - Replace server data with local configs
   - Remove online features or make offline

### Phase 4: Verification
After removals:

1. **Code Search:**
   ```powershell
   # Search for remaining references
   Select-String -Path "c:\Users\konst\OneDrive\Desktop\playr-remaster\Source\*.cs" -Pattern "Facebook|Tapjoy|Flurry" -Recurse
   ```

2. **Dependency Check:**
   - Remove unused DLLs from Dependencies/
   - Update Assembly-CSharp.csproj references

3. **Test Compilation:**
   ```powershell
   dotnet build Assembly-CSharp.csproj
   ```

## Stub Generation
Generating stub files...

[OK] Generated: `Stubs/FacebookHandler_Stub.cs`
[OK] Generated: `Stubs/TapjoyManager_Stub.cs`

## Implementation Checklist

### Preparation
- [ ] Create feature branch: git checkout -b remove-legacy-integrations
- [ ] Review all affected files
- [ ] Backup Source directory

### Facebook Removal
- [ ] Generate Facebook stub classes
- [ ] Replace FacebookHandler references with stub
- [ ] Remove Facebook UI elements from scenes
- [ ] Test game boot without Facebook login
- [ ] Replace friend features with local alternatives
- [ ] Remove Facebook SDK DLLs

### Tapjoy Removal
- [ ] Generate Tapjoy stub classes
- [ ] Remove Tapjoy ad placements
- [ ] Replace ad rewards with direct rewards
- [ ] Remove Tapjoy SDK DLLs
- [ ] Test energy/reward systems

### Analytics Removal
- [ ] Remove Flurry tracking calls
- [ ] Remove or stub WebIntegration
- [ ] Clean up analytics events

### Verification
- [ ] Search for remaining references: Facebook|Tapjoy|Flurry
- [ ] Build project: dotnet build
- [ ] Import into Unity and test compilation
- [ ] Test all game modes without internet
- [ ] Verify no network calls are made

### Final Cleanup
- [ ] Remove unused DLLs from Dependencies/
- [ ] Update .csproj file references
- [ ] Document changes in changelog
- [ ] Commit changes

---
*Generated by Legacy Integration Analyzer*
