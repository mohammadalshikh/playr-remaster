# Import remaining source files in organized batches
# Run this after exiting Safe Mode with 0 compilation errors

$sourcePath = "c:\Users\konst\OneDrive\Desktop\playr-remaster\Source"
$destPath = "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project\Scripts"

# Get list of files not yet imported
$sourceFiles = Get-ChildItem $sourcePath -Filter *.cs
$destFiles = Get-ChildItem $destPath -Recurse -Filter *.cs | Select-Object -ExpandProperty Name
$remaining = $sourceFiles | Where-Object { $destFiles -notcontains $_.Name }

Write-Host "Found $($remaining.Count) files to import" -ForegroundColor Cyan
Write-Host "Will import in batches of 15 files each" -ForegroundColor Cyan
Write-Host ""

# Organize files into logical categories based on naming patterns
$categories = @{
    "UI" = $remaining | Where-Object { $_.Name -match "View|Button|Panel|Popup|Icon|Row|Info" }
    "Minigames" = $remaining | Where-Object { $_.Name -match "Target|Challenge|Phase|Blast" }
    "Social" = $remaining | Where-Object { $_.Name -match "Friend|Facebook|Social|Share|Invite" }
    "System" = $remaining | Where-Object { $_.Name -match "Helper|Handler|Controller|Manager" }
    "Effects" = $remaining | Where-Object { $_.Name -match "Animate|Effect|Fade|Blend|Blink|Sprite" }
    "Other" = $remaining | Where-Object { 
        $name = $_.Name
        -not ($name -match "View|Button|Panel|Popup|Icon|Row|Info|Target|Challenge|Phase|Blast|Friend|Facebook|Social|Share|Invite|Helper|Handler|Controller|Manager|Animate|Effect|Fade|Blend|Blink|Sprite")
    }
}

$batchNumber = 1
$totalImported = 0

foreach ($category in $categories.Keys | Sort-Object) {
    $files = $categories[$category]
    if ($files.Count -eq 0) { continue }
    
    Write-Host "=== CATEGORY: $category ($($files.Count) files) ===" -ForegroundColor Yellow
    
    # Split category into batches of 15
    for ($i = 0; $i -lt $files.Count; $i += 15) {
        $batch = $files | Select-Object -Skip $i -First 15
        
        Write-Host "`nBatch $batchNumber - Importing $($batch.Count) files:" -ForegroundColor Green
        foreach ($file in $batch) {
            Write-Host "  - $($file.Name)" -ForegroundColor Gray
        }
        
        # Copy files to appropriate subdirectory
        foreach ($file in $batch) {
            $targetDir = $destPath
            
            # Place in appropriate subdirectory
            if ($category -eq "UI") { $targetDir = Join-Path $destPath "UI" }
            elseif ($category -eq "Minigames") { $targetDir = Join-Path $destPath "Minigames" }
            elseif ($category -eq "Effects") { $targetDir = Join-Path $destPath "Core" }
            elseif ($category -eq "System") { $targetDir = Join-Path $destPath "Managers" }
            else { $targetDir = Join-Path $destPath "Core" }
            
            if (-not (Test-Path $targetDir)) {
                New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
            }
            
            Copy-Item $file.FullName -Destination $targetDir -Force
        }
        
        $totalImported += $batch.Count
        $batchNumber++
        
        Write-Host "`n[PROGRESS: $totalImported/$($remaining.Count) files imported]" -ForegroundColor Cyan
        Write-Host "Waiting 3 seconds for Unity to compile..." -ForegroundColor Yellow
        Write-Host "Check Unity Console for errors before continuing!" -ForegroundColor Yellow
        Write-Host ""
        
        # Wait for Unity to process
        Start-Sleep -Seconds 3
        
        # Prompt to continue
        if ($i + 15 -lt $files.Count -or ($categories.Keys | Sort-Object | Select-Object -Last 1) -ne $category) {
            Write-Host "Press ENTER to import next batch, or Ctrl+C to stop..." -ForegroundColor White
            Read-Host
        }
    }
}

Write-Host "`n=== IMPORT COMPLETE ===" -ForegroundColor Green
Write-Host "Total files imported: $totalImported" -ForegroundColor Cyan
Write-Host "Check Unity Console for any compilation errors!" -ForegroundColor Yellow
