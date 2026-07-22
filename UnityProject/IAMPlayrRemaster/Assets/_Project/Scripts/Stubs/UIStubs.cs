using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Comprehensive stub classes for legacy plugins (NGUI/EZ GUI, Google IAB, Flurry, etc.)
/// These provide no-op implementations to allow compilation
/// </summary>

// ===== NGUI/EZ GUI UI STUBS =====

public class SpriteText : MonoBehaviour
{
    public string text;
    public Color color = Color.white;
    public float characterSize = 1f;
    public float width = 100f;
    public float height = 20f;
    
    // Instance properties (not static!)
    public string Text { get { return text; } set { text = value; } }
    public Color Color { get { return color; } set { color = value; } }
    public Vector3 BottomRight { get { return transform.position + new Vector3(width, -height, 0); } }
    public float TotalWidth { get { return text != null ? text.Length * characterSize : 0; } }
    
    public void SetText(string newText) { text = newText; }
    public void SetColor(Color c) { color = c; }
    public void SetCharacterSize(float size) { characterSize = size; }
    public void Hide(bool tf) { gameObject.SetActive(!tf); }
    public void Hide(bool tf, bool deactivate) { gameObject.SetActive(!tf && !deactivate); }
}

public class AutoSpriteControlBase : MonoBehaviour
{
    public Color color = Color.white;
    public Color Color { get { return color; } set { color = value; } }
    public void SetColor(Color c) { color = c; }
}

public class UIButton3D : MonoBehaviour
{
    public delegate void OnClickDelegate();
    public delegate void InputDelegate(ref POINTER_INFO ptr);
    public OnClickDelegate onClick;
    public bool controlIsEnabled = true;
    public void SetControlState(bool enabled) { controlIsEnabled = enabled; }
    public void AddInputDelegate(InputDelegate del) { }
}

public class ImageEffectBase : MonoBehaviour
{
    public Material material;
    protected virtual void Start() { }
    protected virtual void OnDisable() { }
}

public interface IUIObject
{
    void Show(bool show);
}

public class UISlider : MonoBehaviour, IUIObject
{
    public delegate void ValueChangedDelegate(IUIObject obj);
    
    public float value = 0f;
    public float Value { get { return value; } set { this.value = value; } }
    public void SetValue(float val) { value = val; }
    public void AddValueChangedDelegate(ValueChangedDelegate del) { }
    public void RemoveValueChangedDelegate(ValueChangedDelegate del) { }
    public void SetValueChangedDelegate(ValueChangedDelegate del) { }
    public GameObject GetKnob() { return gameObject; }
    public void Show(bool show) { gameObject.SetActive(show); }
}

public class UIPanel : MonoBehaviour
{
    public new bool enabled = true;
    
    public void SetActive(bool active) { gameObject.SetActive(active); }
    public void StartTransition(UIPanelManager.SHOW_MODE transition) { }
    public bool IsTransitioning() { return false; }
    public void BringIn() { gameObject.SetActive(true); }
    public void Dismiss() { gameObject.SetActive(false); }
    public UIPanel GetTransition(UIPanelManager.SHOW_MODE showMode) { return this; }
    public void AddTempTransitionDelegate(System.Action callback) { callback?.Invoke(); }
    public void AddTransitionEndDelegate(System.Action callback) { callback?.Invoke(); }
}

public struct POINTER_INFO
{
    public enum INPUT_EVENT
    {
        NO_CHANGE,
        PRESS,
        RELEASE,
        RELEASE_OFF,
        TAP,
        MOVE,
        MOVE_OFF,
        MOVE_ON
    }
    
    public INPUT_EVENT evt;
    public GameObject targetObj;
    public Vector3 position;
}

public class UIButton : MonoBehaviour
{
    public delegate void OnClickDelegate();
    public delegate void InputDelegate(ref POINTER_INFO ptr);
    
    public OnClickDelegate onClick;
    public bool controlIsEnabled = true;
    public string Text { get; set; }
    public Color Color { get; set; }
    public SpriteText spriteText;
    public float width = 100f;
    public float height = 50f;
    public MonoBehaviour scriptWithMethodToInvoke;
    public string methodToInvoke;
    public bool detargetOnDisable = true;
    
    public void Hide(bool hide) { gameObject.SetActive(!hide); }
    public void Hide(bool hide, bool deactivate) { gameObject.SetActive(!hide); }
    public void SetSize(float w, float h) { width = w; height = h; }
    public void SetColor(Color c) { Color = c; }
    public void AddInputDelegate(InputDelegate del) { }
    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void OnDisable() { }
    public virtual void HandleButtonPressed(ref POINTER_INFO ptr) { }
}

public interface IUIListObject
{
    Transform transform { get; }
    void SetData(object data);
    void SetInputDelegate(UIButton.InputDelegate del);
}

public class UIScrollList : MonoBehaviour
{
    public delegate void ItemSnappedDelegate(UIScrollList list, int index);
    
    public Vector2 viewableArea = new Vector2(500, 500);
    public List<GameObject> items = new List<GameObject>();
    public List<GameObject> sceneItems = new List<GameObject>();
    public Vector2 viewableAreaSize = new Vector2(500, 500);
    public int Count { get { return items.Count; } }
    public GameObject SnappedItem { get; set; }
    
    public void Start() { sceneItems = items; }
    public IUIListObject CreateItem(GameObject prefab) 
    { 
        GameObject obj = Instantiate(prefab); 
        items.Add(obj); 
        sceneItems.Add(obj);
        return obj.GetComponent<IUIListObject>();
    }
    public GameObject GetItem(int index) { return index >= 0 && index < items.Count ? items[index] : null; }
    public void ClearList(bool destroy) { if (destroy) items.ForEach(i => Destroy(i)); items.Clear(); sceneItems.Clear(); }
    public void AddItem(GameObject item) { items.Add(item); sceneItems.Add(item); }
    public void CalcClippingRect() { }
    public void ScrollToItem(int index, float time) { if (index >= 0 && index < items.Count) SnappedItem = items[index]; }
    public void AddItemSnappedDelegate(ItemSnappedDelegate del) { }
}

public class PackedSprite : MonoBehaviour
{
    public string spriteName;
    public Color Color { get; set; } = Color.white;
    public float width = 1f;
    public float height = 1f;
    public object[] animations = new object[0];
    
    public void SetFrame(int frame) { }
    public void SetFrame(int frameId, int index) { }
    public void SetFrame(string frameName, int frameNum) { }
    public void SetFrame(int frame, bool resetAnim) { }
    public void SetColor(Color c) { Color = c; }
    public void SetSize(float w, float h) { width = w; height = h; }
    public void SetUVs(Rect uvs) { }
    public void SetTexture(Texture2D texture) { }
    public void PlayAnim(int anim) { }
    public void PlayAnim(string animName) { }
    public void UpdateUVs() { }
    public void Hide(bool tf) { gameObject.SetActive(!tf); }
    public void Hide(bool tf, bool deactivate) { gameObject.SetActive(!tf && !deactivate); }
    public void TruncateTop(float amount) { }
    public void TruncateRight(float amount) { }
    public object GetAnim(string animName) { return null; }
    public object GetAnim(int index) { return index >= 0 && index < animations.Length ? animations[index] : null; }
    public Vector3 GetCenterPoint() { return transform.position; }
}

public class SuperSprite : MonoBehaviour
{
    public string spriteName;
    public Color color = Color.white;
    public bool hidden = false;
    
    public void Hide(bool hide) { hidden = hide; gameObject.SetActive(!hide); }
    public bool IsAnimating() { return false; }
}

public class SuperSpriteAnim : MonoBehaviour
{
    public PackedSprite CurrentSprite;
    
    public void Play(string animName) { }
    public void Stop() { }
}

public class SimpleSprite : MonoBehaviour
{
    public string spriteName;
    public Color color = Color.white;
    
    public void SetColor(Color c) { color = c; }
    public void SetTexture(Texture2D texture) { }
}

public class Button9Slice : MonoBehaviour
{
    public Color color = Color.white;
    public bool isEnabled = true;
    public bool controlIsEnabled = true;
    
    public void SetColor(Color c) { color = c; }
    public void SetEnabled(bool enabled) { isEnabled = enabled; controlIsEnabled = enabled; gameObject.SetActive(enabled); }
}

public class UIProgressBar : MonoBehaviour
{
    public float value = 0f;
    public float width = 100f;
    public float Value { get { return value; } set { this.value = value; } }
    public Vector3 UnclippedBottomRight { get { return transform.position + new Vector3(width, 0, 0); } }
    
    public void SetValue(float val) { value = Mathf.Clamp01(val); }
    public void Hide(bool tf) { gameObject.SetActive(!tf); }
}

// ===== EFFECTS =====

public class BlurEffect : MonoBehaviour
{
    public float blurAmount = 0f;
    public int iterations = 2;
}

// ===== TOUCH/INPUT =====

public interface ICRTouchListener
{
    void OnTouchBegan(List<Touch> touches);
    void OnTouchMoved(List<Touch> touches);
    void OnTouchEnded(List<Touch> touches);
}

public static class CR_TouchManager
{
    public static bool IsInitialized { get { return true; } }
    public static bool IsActive { get; set; } = true;
    
    public static void AddListener(ICRTouchListener listener) { }
    public static void RemoveListener(ICRTouchListener listener) { }
}

// ===== UI MANAGER =====

public static class UIPanelManager
{
    public enum SHOW_MODE
    {
        BringInForward = 0,
        BringInBack = 1,
        DismissForward = 2,
        DismissBack = 3
    }
    
    public const int SHOW = 0;
    public const int HIDE = 1;
}

// ===== ANIMATION/FADE =====

public delegate void EZAnimation();

public static class EZAnimationHelper
{
    public enum ANIM_MODE { To, From, By, TO, FROM, BY }
    public static float linear(float time, float start, float delta, float duration) { return start + delta * (time / duration); }
}

public static class FadeSprite
{
    public static void Fade(PackedSprite sprite, float from, float to, float duration, EZAnimation onComplete, EZAnimation onCancel) { }
    public static void Do(PackedSprite sprite, EZAnimationHelper.ANIM_MODE mode, float start, float end, float duration, float delay, EZAnimation onComplete, System.Func<float, float, float, float, float> easeFunc) 
    { 
        onComplete?.Invoke(); 
    }
    public static void Do(PackedSprite sprite, EZAnimationHelper.ANIM_MODE mode, Color color, System.Func<float, float, float, float, float> interp, float dur, float delay, EZAnimation startDel, EZAnimation del)
    {
        if (sprite != null) sprite.Color = color;
        startDel?.Invoke();
        del?.Invoke();
    }
    public static void Do(Color dest, PackedSprite sprt, EZAnimationHelper.ANIM_MODE mode, System.Func<float, float, float, float, float> interp, float dur, float delay, EZAnimation startDel, EZAnimation del)
    {
        if (sprt != null) sprt.Color = dest;
        startDel?.Invoke();
        del?.Invoke();
    }
}

// ===== GOOGLE PLAY IN-APP BILLING STUBS =====

public class GoogleSkuInfo
{
    public string productId;
    public string price;
    public string title;
    public string description;
}

public class GooglePurchase
{
    public enum GooglePurchaseState { Purchased = 0 }
    
    public string productId;
    public string orderId;
    public string signature;
    public GooglePurchaseState purchaseState;
}

public static class GoogleIABManager
{
    public static event Action<GooglePurchase> purchaseSucceededEvent;
    public static event Action<string> purchaseFailedEvent;
    public static event Action<List<GooglePurchase>, List<GoogleSkuInfo>> queryInventorySucceededEvent;
    public static event Action<string> queryInventoryFailedEvent;
    public static event Action<GooglePurchase> consumePurchaseSucceededEvent;
    public static event Action<string> consumePurchaseFailedEvent;
    public static event Action billingSupportedEvent;
}

public static class GoogleIAB
{
    public static void init(string publicKey) { }
    public static void purchaseProduct(string sku) { }
    public static void consumeProduct(string sku) { }
    public static void consumeProducts(string[] skus) { }
    public static void queryInventory(string[] skus) { }
}

// ===== FLURRY ANALYTICS STUBS =====

public static class FlurryAndroid
{
    public static void onStartSession(string apiKey) { }
    public static void onEndSession() { }
    public static void logEvent(string eventName) { }
    public static void logEvent(string eventName, Dictionary<string, string> parameters) { }
}

// ===== ETCETERA ANDROID UTILITIES STUBS =====

public static class EtceteraAndroid
{
    public static void showAlert(string title, string message) { }
    public static void showAlert(string title, string message, string button) { }
    public static void showAlert(string title, string message, string button, string extraData) { }
    public static void showAlertWithTitleMessageAndButtons(string title, string message, string[] buttons) { }
    public static int scheduleNotification(int secondsFromNow, string title, string message, string extraData, string sound) { return secondsFromNow; }
    public static void cancelNotification(int notificationId) { }
    public static bool isSMSComposerAvailable() { return false; }
    public static void showSMSComposer(string message) { }
}

public static class EtceteraAndroidManager
{
    public static event Action<string> alertButtonClickedEvent;
}

// ===== ADDITIONAL UI MANAGER STUBS =====

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public bool blockInput;
    
    void Awake() { if (instance == null) instance = this; }
    
    public void AddMouseTouchPtrListener(UIButton.InputDelegate del) { }
    public void RemoveMouseTouchPtrListener(UIButton.InputDelegate del) { }
    
    public static void ShowPanel(string panelName) { }
    public static void HidePanel(string panelName) { }
}

public static class FacebookManager
{
    public static event Action sessionOpenedEvent;
    public static event Action<string> loginFailedEvent;
    
    public static void PostToFeed(string message, Action<bool> callback = null) { callback?.Invoke(true); }
    public static void ShowInviteDialog(Action callback = null) { callback?.Invoke(); }
}

public class UIRadioBtn : MonoBehaviour
{
    public bool isSelected;
    public string groupId;
    public bool controlIsEnabled = true;
    public Color Color { get; set; } = Color.white;
    
    public void SetSelected(bool selected) { isSelected = selected; }
    public void SetGroup(string group) { groupId = group; }
    public void SetState(int state) { isSelected = (state == 1); }
    public void SetColor(Color c) { Color = c; }
}

// ===== PRIME31 PLUGIN STUBS =====

namespace Prime31
{
    public static class GameCenterBinding
    {
        public static void showLeaderboard(string leaderboardId) { }
        public static void reportScore(long score, string leaderboardId) { }
    }
    
    public static class PlayGameServices
    {
        public static void showLeaderboard(string leaderboardId) { }
        public static void reportScore(long score, string leaderboardId) { }
    }
    
    public static class FacebookAndroid
    {
        public static void init() { }
        public static void loginWithReadPermissions(string[] permissions) { }
        public static void loginWithPublishPermissions(string[] permissions) { }
        public static void logout() { }
        public static bool isSessionValid() { return false; }
        public static void showDialog(string dialogType, Dictionary<string, string> parameters) { }
        public static string getAccessToken() { return string.Empty; }
    }
}

// ===== UNITY IMAGE EFFECTS STUBS =====

public class SunShafts : MonoBehaviour
{
    public Transform sunTransform;
    public int radialBlurIterations = 2;
    public Color sunColor = Color.white;
    public float sunShaftBlurRadius = 2.5f;
    public float sunShaftIntensity = 1.15f;
    public float maxRadius = 0.75f;
    public bool useDepthTexture = true;
}

// ===== FACEBOOK SDK STUBS =====

public enum HTTPVerb { GET, POST, DELETE }

public static class Facebook
{
    public static FacebookHandler instance { get { return FacebookHandler.Instance; } }
    
    public static void API(string endpoint, HTTPVerb verb, Dictionary<string, string> parameters, Action<string> callback) 
    { 
        callback?.Invoke("{\"data\":[]}"); 
    }
    
    public static void graphRequest(string endpoint, HTTPVerb verb, Action<string, object> callback)
    {
        callback?.Invoke(null, new Dictionary<string, object>());
    }
    
    public static void getAppAccessToken(string appId, string appSecret, Action callback)
    {
        callback?.Invoke();
    }
    
    public static void postScore(string userId, int score, Action<string> callback)
    {
        callback?.Invoke(null);
    }
    
    public static void getAllScores(string appId, Action<string, object> callback)
    {
        callback?.Invoke(null, new Dictionary<string, object> { { "data", new object[0] } });
    }
}

// ===== JSON PARSING STUBS =====

public static class Json
{
    public static T Deserialize<T>(string json) where T : new() { return new T(); }
    public static Dictionary<string, object> Deserialize(string json) { return new Dictionary<string, object>(); }
    public static string Serialize(object obj) { return "{}"; }
}

public static class MiniJSON
{
    public static object Deserialize(string json) { return new Dictionary<string, object>(); }
    public static string Serialize(object obj) { return "{}"; }
    public static object jsonDecode(string json) { return new Hashtable(); }
}

// ===== LIGHTMAP SWITCHING STUBS =====

public static class SwitchLM
{
    public static void switchmaps(int indexfar, int indexnear, int currentIndex, Texture2D[] texFar, Texture2D[] texNear) { }
}

// Note: Some errors reference global variables like 'width', 'height', 'currentBackground', 'viewableArea', 'items'
// These are member variables accessed without 'this.' prefix in decompiled code and need manual fixing.
