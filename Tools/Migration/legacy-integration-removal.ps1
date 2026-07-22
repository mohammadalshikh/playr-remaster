# Legacy Integration Removal Tool
# Identifies and creates stubs for legacy social/ad integrations
# Removes dependencies on Facebook, Tapjoy, Flurry, etc.

param(
    [string]$SourcePath = "..\Source",
    [string]$OutputPath = "..\Documentation\Architecture\legacy-integrations.md",
    [switch]$GenerateStubs
)

Write-Host "=== Legacy Integration Analyzer ===" -ForegroundColor Cyan
Write-Host "Scanning for legacy integrations..." -ForegroundColor Yellow

# Define legacy integration patterns
$legacyIntegrations = @{
    'Facebook' = @{
        Patterns = @('Facebook', 'FB\.', 'fbAppId', 'FacebookAndroid', 'FacebookHandler')
        Description = 'Facebook SDK integration'
        StubStrategy = 'Create empty FacebookHandler with no-op methods'
        Priority = 'High'
        Files = @()
    }
    'Tapjoy' = @{
        Patterns = @('Tapjoy', 'TapjoyPlugin', 'TapjoyManager', 'TapjoyConnect')
        Description = 'Tapjoy advertising SDK'
        StubStrategy = 'Remove entirely or stub rewards'
        Priority = 'High'
        Files = @()
    }
    'Flurry' = @{
        Patterns = @('Flurry', 'FlurryAnalytics')
        Description = 'Flurry analytics'
        StubStrategy = 'Remove or replace with Unity Analytics'
        Priority = 'Medium'
        Files = @()
    }
    'WebIntegration' = @{
        Patterns = @('WebIntegration', 'P31RestKit', 'RestKit')
        Description = 'Web API integration'
        StubStrategy = 'Remove or replace with offline data'
        Priority = 'Medium'
        Files = @()
    }
    'AndroidJava' = @{
        Patterns = @('AndroidJavaObject', 'AndroidJavaClass', 'AndroidJNI')
        Description = 'Native Android calls'
        StubStrategy = 'Review and keep only essential calls'
        Priority = 'Low'
        Files = @()
    }
}

# Scan source files
$sourceFiles = Get-ChildItem -Path $SourcePath -Filter "*.cs" -Recurse
$totalFiles = $sourceFiles.Count
$stats = @{
    TotalFiles = $totalFiles
    FilesWithIntegrations = @{}
    IntegrationCounts = @{}
}

Write-Host "Scanning $totalFiles files..." -ForegroundColor Yellow

$progress = 0
foreach ($file in $sourceFiles) {
    $progress++
    Write-Progress -Activity "Scanning files" -Status "$progress/$totalFiles" -PercentComplete (($progress / $totalFiles) * 100)
    
    $content = Get-Content -Path $file.FullName -Raw
    $relPath = $file.FullName.Replace("$SourcePath\", "")
    $fileIntegrations = @()
    
    foreach ($integration in $legacyIntegrations.Keys) {
        $patterns = $legacyIntegrations[$integration].Patterns
        $found = $false
        
        foreach ($pattern in $patterns) {
            if ($content -match $pattern) {
                $found = $true
                break
            }
        }
        
        if ($found) {
            $fileIntegrations += $integration
            $legacyIntegrations[$integration].Files += $relPath
            
            if (-not $stats.IntegrationCounts.ContainsKey($integration)) {
                $stats.IntegrationCounts[$integration] = 0
            }
            $stats.IntegrationCounts[$integration]++
        }
    }
    
    if ($fileIntegrations.Count -gt 0) {
        $stats.FilesWithIntegrations[$relPath] = $fileIntegrations
    }
}

Write-Progress -Activity "Scanning files" -Completed

# Generate report
$report = @"
# Legacy Integration Analysis Report
Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

## Summary
- **Total Files Scanned:** $totalFiles
- **Files with Legacy Code:** $($stats.FilesWithIntegrations.Count)

## Integration Breakdown

"@

foreach ($integration in ($legacyIntegrations.GetEnumerator() | Sort-Object { $_.Value.Priority })) {
    $name = $integration.Key
    $data = $integration.Value
    $fileCount = $data.Files.Count
    
    $report += @"

### $name [$($data.Priority) Priority]
- **Description:** $($data.Description)
- **Files Affected:** $fileCount
- **Stub Strategy:** $($data.StubStrategy)

"@
    
    if ($fileCount -gt 0) {
        $report += "#### Affected Files:`n"
        foreach ($file in ($data.Files | Sort-Object)) {
            $report += "- ``$file```n"
        }
    } else {
        $report += "_No files found with this integration_`n"
    }
}

# Add removal strategy
$report += @"

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
   ``````csharp
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
   ``````

3. **Remove UI Elements:**
   - Facebook login buttons
   - Friend invite panels
   - Social sharing features

4. **Replace Features:**
   - Leaderboards → Local high scores
   - Friend challenges → AI opponents or skip
   - Gifts → Direct rewards or skip

### Phase 2: Tapjoy Ads
Remove all ad integration:

1. **Remove Files:**
   - TapjoyPlugin.cs
   - TapjoyManager.cs
   - Any Tapjoy-specific UI

2. **Replace Rewards:**
   - Tapjoy rewards → Direct in-game currency
   - Ad-based energy → Free energy or time-based refill

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
   ``````powershell
   # Search for remaining references
   Select-String -Path "$SourcePath\*.cs" -Pattern "Facebook|Tapjoy|Flurry" -Recurse
   ``````

2. **Dependency Check:**
   - Remove unused DLLs from Dependencies/
   - Update Assembly-CSharp.csproj references

3. **Test Compilation:**
   ``````powershell
   dotnet build Assembly-CSharp.csproj
   ``````

## Stub Generation

"@

if ($GenerateStubs) {
    $report += "Generating stub files...`n`n"
    
    # Generate stub directory
    $stubDir = Join-Path $SourcePath "Stubs"
    if (-not (Test-Path $stubDir)) {
        New-Item -ItemType Directory -Path $stubDir -Force | Out-Null
    }
    
    # Facebook stub
    $facebookStub = @'
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stub implementation of FacebookHandler for offline play
/// Replaces original Facebook SDK integration
/// </summary>
public class FacebookHandler : MonoBehaviour
{
    public static FacebookHandler Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public bool IsLoggedIn() => false;
    public string GetUserId() => "offline_user";
    public string GetUserName() => "Player";
    
    public void Login(Action<bool> callback = null)
    {
        Debug.Log("[FacebookHandler] Stub: Login called (offline mode)");
        callback?.Invoke(false);
    }
    
    public void Logout()
    {
        Debug.Log("[FacebookHandler] Stub: Logout called (offline mode)");
    }
    
    public void GetFriends(Action<List<FacebookPlayer>> callback)
    {
        Debug.Log("[FacebookHandler] Stub: GetFriends called (offline mode)");
        callback?.Invoke(new List<FacebookPlayer>());
    }
    
    public void PostScore(int score, Action<bool> callback = null)
    {
        Debug.Log("[FacebookHandler] Stub: PostScore called (offline mode)");
        callback?.Invoke(true);
    }
    
    public void InviteFriends(Action callback = null)
    {
        Debug.Log("[FacebookHandler] Stub: InviteFriends called (offline mode)");
        callback?.Invoke();
    }
}

public class FacebookPlayer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
}
'@
    
    $stubPath = Join-Path $stubDir "FacebookHandler_Stub.cs"
    Set-Content -Path $stubPath -Value $facebookStub
    $report += "[OK] Generated: ``Stubs/FacebookHandler_Stub.cs```n"
    
    # Tapjoy stub
    $tapjoyStub = @'
using System;
using UnityEngine;

/// <summary>
/// Stub implementation for Tapjoy ads - all rewards given for free
/// </summary>
public class TapjoyManager : MonoBehaviour
{
    public static TapjoyManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ShowRewardedAd(Action<bool> onComplete)
    {
        Debug.Log("[TapjoyManager] Stub: Rewarded ad - giving reward for free (offline mode)");
        onComplete?.Invoke(true); // Always succeed, no ad shown
    }
    
    public void ShowInterstitial()
    {
        Debug.Log("[TapjoyManager] Stub: Interstitial ad skipped (offline mode)");
    }
    
    public bool IsRewardedAdAvailable() => true; // Always available
}
'@
    
    $stubPath = Join-Path $stubDir "TapjoyManager_Stub.cs"
    Set-Content -Path $stubPath -Value $tapjoyStub
    $report += "[OK] Generated: ``Stubs/TapjoyManager_Stub.cs```n"
    
    Write-Host "`nGenerated stub files in: $stubDir" -ForegroundColor Green
} else {
    $report += "To generate stub files, run with ``-GenerateStubs`` flag`n"
}

$report += @"

## Implementation Checklist

### Preparation
- [ ] Create feature branch: `git checkout -b remove-legacy-integrations`
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
- [ ] Search for remaining references: `Facebook|Tapjoy|Flurry`
- [ ] Build project: `dotnet build`
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
"@

# Save report
$outputDir = Split-Path -Parent $OutputPath
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
}

$report | Out-File -FilePath $OutputPath -Encoding UTF8

Write-Host "`n=== Analysis Complete ===" -ForegroundColor Green
Write-Host "Report saved to: $OutputPath" -ForegroundColor Yellow

Write-Host "`nLegacy Integrations Found:" -ForegroundColor Cyan
foreach ($integration in ($stats.IntegrationCounts.GetEnumerator() | Sort-Object -Property Value -Descending)) {
    $priority = $legacyIntegrations[$integration.Key].Priority
    $color = switch ($priority) {
        'High' { 'Red' }
        'Medium' { 'Yellow' }
        default { 'White' }
    }
    Write-Host "  [$priority] $($integration.Key): $($integration.Value) files" -ForegroundColor $color
}

Write-Host "`nNext Steps:" -ForegroundColor Green
Write-Host "1. Review report: $OutputPath" -ForegroundColor White
Write-Host "2. Generate stubs: .\legacy-integration-removal.ps1 -GenerateStubs" -ForegroundColor White
Write-Host "3. Begin phased removal starting with high priority" -ForegroundColor White
