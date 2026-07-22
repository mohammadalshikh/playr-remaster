# Unity API Migration Auto-Fixer
# Automatically applies safe API migrations to source code
# Always creates backups and allows dry-run mode

param(
    [string]$SourcePath = "..\Source",
    [string]$BackupPath = "..\Source_Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')",
    [switch]$DryRun,
    [switch]$Apply,
    [switch]$SkipBackup
)

if (-not $DryRun -and -not $Apply) {
    Write-Host "ERROR: Must specify either -DryRun or -Apply" -ForegroundColor Red
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\api-migration-apply.ps1 -DryRun          # Preview changes"
    Write-Host "  .\api-migration-apply.ps1 -Apply           # Apply changes with backup"
    Write-Host "  .\api-migration-apply.ps1 -Apply -SkipBackup  # Apply without backup (not recommended)"
    exit 1
}

Write-Host "=== Unity API Migration Auto-Fixer ===" -ForegroundColor Cyan

if ($DryRun) {
    Write-Host "MODE: Dry Run (preview only, no changes)" -ForegroundColor Yellow
} else {
    Write-Host "MODE: Apply Changes" -ForegroundColor Green
    
    if (-not $SkipBackup) {
        Write-Host "Creating backup: $BackupPath" -ForegroundColor Yellow
        Copy-Item -Path $SourcePath -Destination $BackupPath -Recurse -Force
        Write-Host "Backup created successfully" -ForegroundColor Green
    } else {
        Write-Host "WARNING: Skipping backup (not recommended!)" -ForegroundColor Red
        Start-Sleep -Seconds 2
    }
}

# Define safe automated replacements (simple string replacements)
$simpleReplacements = @{
    'Application.LoadLevel(' = 'SceneManager.LoadScene('
    'Application.LoadLevelAsync(' = 'SceneManager.LoadSceneAsync('
    'Application.loadedLevel' = 'SceneManager.GetActiveScene().buildIndex'
    'Application.loadedLevelName' = 'SceneManager.GetActiveScene().name'
}

# Define regex-based replacements (more complex patterns)
$regexReplacements = @(
    @{
        Pattern = '(\w+)\.active\s*=\s*true'
        Replacement = '$1.SetActive(true)'
        Description = 'GameObject.active = true -> SetActive(true)'
    },
    @{
        Pattern = '(\w+)\.active\s*=\s*false'
        Replacement = '$1.SetActive(false)'
        Description = 'GameObject.active = false -> SetActive(false)'
    },
    @{
        Pattern = '(\w+)\.active\s*=\s*(\w+)'
        Replacement = '$1.SetActive($2)'
        Description = 'GameObject.active = var -> SetActive(var)'
    }
)

# Required using statements to add
$requiredUsings = @{
    'SceneManager' = 'using UnityEngine.SceneManagement;'
    'UnityWebRequest' = 'using UnityEngine.Networking;'
}

# Statistics
$stats = @{
    FilesProcessed = 0
    FilesModified = 0
    TotalReplacements = 0
    ReplacementsByType = @{}
}

# Get all C# source files
$sourceFiles = Get-ChildItem -Path $SourcePath -Filter "*.cs" -Recurse
$totalFiles = $sourceFiles.Count

Write-Host "`nProcessing $totalFiles files..." -ForegroundColor Yellow

$progress = 0
foreach ($file in $sourceFiles) {
    $progress++
    Write-Progress -Activity "Processing files" -Status "$progress/$totalFiles" -PercentComplete (($progress / $totalFiles) * 100)
    
    $stats.FilesProcessed++
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    $fileModified = $false
    $replacementsInFile = 0
    
    # Apply simple string replacements
    foreach ($old in $simpleReplacements.Keys) {
        $new = $simpleReplacements[$old]
        if ($content.Contains($old)) {
            $count = ([regex]::Matches($content, [regex]::Escape($old))).Count
            $content = $content.Replace($old, $new)
            $replacementsInFile += $count
            $fileModified = $true
            
            if (-not $stats.ReplacementsByType.ContainsKey($old)) {
                $stats.ReplacementsByType[$old] = 0
            }
            $stats.ReplacementsByType[$old] += $count
            
            if ($DryRun) {
                Write-Host "  [DRY RUN] $($file.Name): $old -> $new ($count times)" -ForegroundColor Cyan
            }
        }
    }
    
    # Apply regex replacements
    foreach ($replacement in $regexReplacements) {
        $matches = [regex]::Matches($content, $replacement.Pattern)
        if ($matches.Count -gt 0) {
            $content = [regex]::Replace($content, $replacement.Pattern, $replacement.Replacement)
            $replacementsInFile += $matches.Count
            $fileModified = $true
            
            $desc = $replacement.Description
            if (-not $stats.ReplacementsByType.ContainsKey($desc)) {
                $stats.ReplacementsByType[$desc] = 0
            }
            $stats.ReplacementsByType[$desc] += $matches.Count
            
            if ($DryRun) {
                Write-Host "  [DRY RUN] $($file.Name): $desc ($($matches.Count) times)" -ForegroundColor Cyan
            }
        }
    }
    
    # Check if we need to add using statements
    if ($fileModified) {
        foreach ($api in $requiredUsings.Keys) {
            if ($content.Contains($api) -and -not $content.Contains($requiredUsings[$api])) {
                # Find the position after existing usings
                if ($content -match '(using\s+[\w.]+;\s*)+') {
                    $usingBlock = $matches[0]
                    $newUsing = "$($requiredUsings[$api])`n"
                    $content = $content.Replace($usingBlock, "$usingBlock$newUsing")
                    
                    if ($DryRun) {
                        Write-Host "  [DRY RUN] $($file.Name): Add $($requiredUsings[$api])" -ForegroundColor Magenta
                    }
                }
            }
        }
    }
    
    # Save changes if not dry run
    if ($fileModified) {
        $stats.FilesModified++
        $stats.TotalReplacements += $replacementsInFile
        
        if ($Apply) {
            Set-Content -Path $file.FullName -Value $content -NoNewline -Encoding UTF8
            Write-Host "  [MODIFIED] $($file.Name): $replacementsInFile changes applied" -ForegroundColor Green
        }
    }
}

Write-Progress -Activity "Processing files" -Completed

# Print summary
Write-Host "`n=== Migration Summary ===" -ForegroundColor Cyan
Write-Host "Files processed: $($stats.FilesProcessed)" -ForegroundColor White
Write-Host "Files modified: $($stats.FilesModified)" -ForegroundColor $(if ($stats.FilesModified -gt 0) { 'Yellow' } else { 'White' })
Write-Host "Total replacements: $($stats.TotalReplacements)" -ForegroundColor $(if ($stats.TotalReplacements -gt 0) { 'Green' } else { 'White' })

if ($stats.ReplacementsByType.Count -gt 0) {
    Write-Host "`nReplacements by type:" -ForegroundColor Yellow
    foreach ($type in ($stats.ReplacementsByType.GetEnumerator() | Sort-Object -Property Value -Descending)) {
        Write-Host "  $($type.Key): $($type.Value)" -ForegroundColor White
    }
}

if ($DryRun) {
    Write-Host "`n[DRY RUN] No files were modified." -ForegroundColor Yellow
    Write-Host "To apply these changes, run:" -ForegroundColor Cyan
    Write-Host "  .\api-migration-apply.ps1 -Apply" -ForegroundColor White
} else {
    Write-Host "`nChanges applied successfully!" -ForegroundColor Green
    if (-not $SkipBackup) {
        Write-Host "Backup location: $BackupPath" -ForegroundColor Yellow
    }
}

# Warning about manual fixes needed
if ($stats.TotalReplacements -gt 0) {
    Write-Host "`n⚠ IMPORTANT: Review changes before committing!" -ForegroundColor Yellow
    Write-Host "Some patterns require manual fixing:" -ForegroundColor White
    Write-Host "  - .renderer → GetComponent<Renderer>()" -ForegroundColor Gray
    Write-Host "  - .rigidbody → GetComponent<Rigidbody>()" -ForegroundColor Gray
    Write-Host "  - .collider → GetComponent<Collider>()" -ForegroundColor Gray
    Write-Host "  - WWW class → UnityWebRequest" -ForegroundColor Gray
    Write-Host "  - GUIText/GUITexture → UI components" -ForegroundColor Gray
}

Write-Host "`nNext Steps:" -ForegroundColor Cyan
Write-Host "1. Review changes in your source control" -ForegroundColor White
Write-Host "2. Run migration analyzer again to check remaining issues" -ForegroundColor White
Write-Host "3. Import code into Unity project" -ForegroundColor White
Write-Host "4. Fix compilation errors" -ForegroundColor White
Write-Host "5. Test gameplay functionality" -ForegroundColor White
