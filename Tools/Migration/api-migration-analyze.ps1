# Unity API Migration Analyzer
# Scans decompiled source code for obsolete Unity 4.x APIs
# Generates migration report and automated fix suggestions

param(
    [string]$SourcePath = "..\Source",
    [string]$OutputPath = "..\Documentation\Architecture\migration-report.md",
    [switch]$AutoFix
)

Write-Host "=== Unity API Migration Analyzer ===" -ForegroundColor Cyan
Write-Host "Scanning: $SourcePath" -ForegroundColor Yellow

# Define obsolete API patterns and their modern replacements
$apiMigrations = @{
    'Application.LoadLevel' = @{
        'Replacement' = 'SceneManager.LoadScene'
        'Namespace' = 'UnityEngine.SceneManagement'
        'Description' = 'Scene loading API modernization'
        'Severity' = 'Critical'
    }
    'Application.LoadLevelAsync' = @{
        'Replacement' = 'SceneManager.LoadSceneAsync'
        'Namespace' = 'UnityEngine.SceneManagement'
        'Description' = 'Async scene loading modernization'
        'Severity' = 'Critical'
    }
    '\.active\s*=' = @{
        'Replacement' = '.SetActive()'
        'Namespace' = 'UnityEngine'
        'Description' = 'GameObject activation API change'
        'Severity' = 'High'
        'IsRegex' = $true
    }
    '\.renderer\.' = @{
        'Replacement' = '.GetComponent<Renderer>().'
        'Namespace' = 'UnityEngine'
        'Description' = 'Component access modernization'
        'Severity' = 'High'
        'IsRegex' = $true
    }
    '\.rigidbody\.' = @{
        'Replacement' = '.GetComponent<Rigidbody>().'
        'Namespace' = 'UnityEngine'
        'Description' = 'Rigidbody access modernization'
        'Severity' = 'High'
        'IsRegex' = $true
    }
    '\.collider\.' = @{
        'Replacement' = '.GetComponent<Collider>().'
        'Namespace' = 'UnityEngine'
        'Description' = 'Collider access modernization'
        'Severity' = 'High'
        'IsRegex' = $true
    }
    '\.audio\.' = @{
        'Replacement' = '.GetComponent<AudioSource>().'
        'Namespace' = 'UnityEngine'
        'Description' = 'AudioSource access modernization'
        'Severity' = 'Medium'
        'IsRegex' = $true
    }
    '\.animation\.' = @{
        'Replacement' = '.GetComponent<Animation>().'
        'Namespace' = 'UnityEngine'
        'Description' = 'Animation access modernization'
        'Severity' = 'Medium'
        'IsRegex' = $true
    }
    'new WWW\(' = @{
        'Replacement' = 'UnityWebRequest'
        'Namespace' = 'UnityEngine.Networking'
        'Description' = 'Network API modernization'
        'Severity' = 'Critical'
        'IsRegex' = $true
    }
    'GUIText' = @{
        'Replacement' = 'TextMeshPro or UI Text'
        'Namespace' = 'TMPro or UnityEngine.UI'
        'Description' = 'Legacy GUI component'
        'Severity' = 'Medium'
    }
    'GUITexture' = @{
        'Replacement' = 'UI Image'
        'Namespace' = 'UnityEngine.UI'
        'Description' = 'Legacy GUI component'
        'Severity' = 'Medium'
    }
}

# Scan source files
$sourceFiles = Get-ChildItem -Path $SourcePath -Filter "*.cs" -Recurse
$totalFiles = $sourceFiles.Count
$issuesFound = @{}
$fileIssues = @{}

Write-Host "`nScanning $totalFiles files..." -ForegroundColor Yellow

$progress = 0
foreach ($file in $sourceFiles) {
    $progress++
    Write-Progress -Activity "Analyzing source files" -Status "$progress/$totalFiles" -PercentComplete (($progress / $totalFiles) * 100)
    
    $content = Get-Content -Path $file.FullName -Raw
    $relPath = $file.FullName.Replace("$SourcePath\", "")
    
    foreach ($pattern in $apiMigrations.Keys) {
        $migration = $apiMigrations[$pattern]
        $matches = @()
        
        if ($migration.IsRegex) {
            if ($content -match $pattern) {
                $matches = ([regex]::Matches($content, $pattern))
            }
        } else {
            if ($content.Contains($pattern)) {
                $matches = [regex]::Matches($content, [regex]::Escape($pattern))
            }
        }
        
        if ($matches.Count -gt 0) {
            if (-not $issuesFound.ContainsKey($pattern)) {
                $issuesFound[$pattern] = @{
                    'Count' = 0
                    'Files' = @()
                    'Migration' = $migration
                }
            }
            
            $issuesFound[$pattern].Count += $matches.Count
            $issuesFound[$pattern].Files += @{
                'File' = $relPath
                'Matches' = $matches.Count
            }
            
            if (-not $fileIssues.ContainsKey($relPath)) {
                $fileIssues[$relPath] = @()
            }
            $fileIssues[$relPath] += @{
                'Pattern' = $pattern
                'Count' = $matches.Count
                'Migration' = $migration
            }
        }
    }
}

Write-Progress -Activity "Analyzing source files" -Completed

# Generate report
$report = @"
# Unity API Migration Report
Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

## Summary
- **Total Files Scanned:** $totalFiles
- **Files with Issues:** $($fileIssues.Count)
- **Total Issues Found:** $(($issuesFound.Values | Measure-Object -Property Count -Sum).Sum)
- **Critical Issues:** $(($issuesFound.Values | Where-Object { $_.Migration.Severity -eq 'Critical' } | Measure-Object -Property Count -Sum).Sum)
- **High Priority:** $(($issuesFound.Values | Where-Object { $_.Migration.Severity -eq 'High' } | Measure-Object -Property Count -Sum).Sum)
- **Medium Priority:** $(($issuesFound.Values | Where-Object { $_.Migration.Severity -eq 'Medium' } | Measure-Object -Property Count -Sum).Sum)

## Issues by API Pattern

"@

# Sort by severity and count
$sortedIssues = $issuesFound.GetEnumerator() | Sort-Object { $_.Value.Migration.Severity }, { $_.Value.Count } -Descending

foreach ($issue in $sortedIssues) {
    $pattern = $issue.Key
    $data = $issue.Value
    $mig = $data.Migration
    
    $report += @"

### $($mig.Severity): $pattern
- **Occurrences:** $($data.Count)
- **Files Affected:** $($data.Files.Count)
- **Replacement:** ``$($mig.Replacement)``
- **Required Namespace:** ``$($mig.Namespace)``
- **Description:** $($mig.Description)

#### Affected Files:
"@
    
    foreach ($file in ($data.Files | Sort-Object -Property Matches -Descending | Select-Object -First 10)) {
        $report += "`n- ``$($file.File)`` ($($file.Matches) occurrences)"
    }
    
    if ($data.Files.Count -gt 10) {
        $report += "`n- ... and $($data.Files.Count - 10) more files"
    }
    
    $report += "`n"
}

# Add migration checklist
$report += @"

## Migration Checklist

### Phase 1: Critical APIs (Must Fix)
"@

foreach ($issue in ($sortedIssues | Where-Object { $_.Value.Migration.Severity -eq 'Critical' })) {
    $report += "`n- [ ] Migrate ``$($issue.Key)`` → ``$($issue.Value.Migration.Replacement)`` ($($issue.Value.Count) occurrences)"
}

$report += @"

### Phase 2: High Priority
"@

foreach ($issue in ($sortedIssues | Where-Object { $_.Value.Migration.Severity -eq 'High' })) {
    $report += "`n- [ ] Migrate ``$($issue.Key)`` → ``$($issue.Value.Migration.Replacement)`` ($($issue.Value.Count) occurrences)"
}

$report += @"

### Phase 3: Medium Priority
"@

foreach ($issue in ($sortedIssues | Where-Object { $_.Value.Migration.Severity -eq 'Medium' })) {
    $report += "`n- [ ] Migrate ``$($issue.Key)`` → ``$($issue.Value.Migration.Replacement)`` ($($issue.Value.Count) occurrences)"
}

$report += @"


## Recommended Migration Strategy

1. **Backup**: Ensure all source files are in version control
2. **Phase 1**: Fix critical API issues (required for compilation in Unity 2022)
3. **Phase 2**: Address high priority issues (major functionality)
4. **Phase 3**: Handle medium priority issues (polish and optimization)
5. **Test**: After each phase, test in Unity Editor
6. **Build**: Verify Android build after all migrations

## Automated Migration Script

To automatically apply some fixes, run:
``````powershell
.\api-migration-apply.ps1 -SourcePath "$SourcePath" -DryRun
``````

Review changes, then apply:
``````powershell
.\api-migration-apply.ps1 -SourcePath "$SourcePath" -Apply
``````

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
"@

# Save report
$outputDir = Split-Path -Parent $OutputPath
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
}

$report | Out-File -FilePath $OutputPath -Encoding UTF8

Write-Host "`n=== Analysis Complete ===" -ForegroundColor Green
Write-Host "Report saved to: $OutputPath" -ForegroundColor Yellow
Write-Host "`nSummary:" -ForegroundColor Cyan
Write-Host "  Files scanned: $totalFiles" -ForegroundColor White
Write-Host "  Files with issues: $($fileIssues.Count)" -ForegroundColor White
Write-Host "  Total issues: $(($issuesFound.Values | Measure-Object -Property Count -Sum).Sum)" -ForegroundColor White

if ($issuesFound.Count -gt 0) {
    Write-Host "`nTop Issues:" -ForegroundColor Yellow
    $top5 = $sortedIssues | Select-Object -First 5
    foreach ($issue in $top5) {
        Write-Host "  $($issue.Value.Migration.Severity): $($issue.Key) - $($issue.Value.Count) occurrences" -ForegroundColor $(
            switch ($issue.Value.Migration.Severity) {
                'Critical' { 'Red' }
                'High' { 'Yellow' }
                default { 'White' }
            }
        )
    }
}

Write-Host "`nNext: Review migration report and run api-migration-apply.ps1" -ForegroundColor Green
