# Delete duplicate and stub files
$ErrorActionPreference = "SilentlyContinue"

$filesToDelete = @(
    "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project\Scripts\Stubs\FacebookHandler_Stub.cs",
    "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project\Scripts\Stubs\TapjoyManager_Stub.cs",
    "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project\Scripts\Core\BeatenFriendPopup.cs",
    "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project\Scripts\Core\FacebookPlayerView.cs",
    "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project\Scripts\Core\InviteFriendsView.cs",
    "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project\Scripts\UI\TargetInfo.cs",
    "c:\Users\konst\OneDrive\Desktop\playr-remaster\UnityProject\IAMPlayrRemaster\Assets\_Project\Scripts\UI\PopupViewController.cs"
)

foreach ($file in $filesToDelete) {
    if (Test-Path $file) {
        Remove-Item $file -Force
        Write-Host "✓ Deleted: $(Split-Path $file -Leaf)" -ForegroundColor Green
    } else {
        Write-Host "- Not found: $(Split-Path $file -Leaf)" -ForegroundColor Yellow
    }
}

Write-Host "`n=== Cleanup Complete ===" -ForegroundColor Cyan
