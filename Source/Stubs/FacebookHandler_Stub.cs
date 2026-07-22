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
