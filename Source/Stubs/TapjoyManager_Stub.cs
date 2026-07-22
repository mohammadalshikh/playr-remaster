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
