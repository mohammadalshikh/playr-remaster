using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraScrollController : MonoBehaviour
{
	public delegate void HandleCameraAnimCompleted();

	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private float leftMaxOffsetX;

	[SerializeField]
	private float rightMaxOffsetX;

	[SerializeField]
	private float nearMaxOffsetZ;

	[SerializeField]
	private float farMaxOffsetZ;

	private Transform cachedCameraTransform;

	private Vector3 cameraStartingPos;

	private Quaternion camaeraStartingRot;

	private bool buttonClicked;

	private Vector2 deltaPosition;

	[SerializeField]
	private BackgroundMask backgroundMask;

	[SerializeField]
	private SunShafts sunShafts;

	[SerializeField]
	private Animation cameraAnim;

	private float origSunShaftIntensity;

	public Animation CameraAnim => cameraAnim;

	[method: MethodImpl(32)]
	public static event HandleCameraAnimCompleted CameraAnimCompleted;

	private void Awake()
	{
		cachedCameraTransform = base.transform;
		cameraStartingPos = cachedCameraTransform.position;
		camaeraStartingRot = cachedCameraTransform.rotation;
		origSunShaftIntensity = sunShafts.sunShaftIntensity;
		if (DeviceInfo.IsHighEnd && !DeviceInfo.IsTablet)
		{
			sunShafts.enabled = true;
		}
	}

	private void Start()
	{
		MapLocation.MapButtonTapped += HandleMapButtonTapped;
		backgroundMask.UnmaskBackground();
		if (GameData.Instance != null)
		{
			if (!GameData.Instance.HasIntroSeqOccured)
			{
				StartCoroutine(PlayCameraAnimation());
			}
			else
			{
				sunShafts.enabled = false;
			}
		}
	}

	private void HandleMapButtonTapped(Vector3 targetPos)
	{
		backgroundMask.MaskBackground(backgroundMask.DefaultTransitionTime);
		if ((bool)base.transform.gameObject)
		{
			iTween.MoveTo(base.transform.gameObject, iTween.Hash("position", new Vector3(targetPos.x, targetPos.y + 10f, targetPos.z + 15f), "time", 1f, "easeType", iTween.EaseType.easeOutCubic, "onComplete", (Action<object>)delegate
			{
				MapLocation.MapButtonTapped -= HandleMapButtonTapped;
				StartCoroutine(GameData.Instance.LoadLevel(SCENES.GAME_TYPE_MENU));
			}));
		}
	}

	private void Update()
	{
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && GameData.Instance.HasIntroSeqOccured && !MonoSingleton<PanelManager>.Instance.IsPanelDisplaying)
		{
			Vector2 vector = Input.GetTouch(0).deltaPosition;
			cachedCameraTransform.position = new Vector3(Mathf.Clamp(vector.x * moveSpeed + cachedCameraTransform.position.x, cameraStartingPos.x - leftMaxOffsetX, cameraStartingPos.x + rightMaxOffsetX), cachedCameraTransform.position.y, Mathf.Clamp(vector.y * moveSpeed + cachedCameraTransform.position.z, cameraStartingPos.z - farMaxOffsetZ, cameraStartingPos.z + nearMaxOffsetZ * 0.5f));
		}
	}

	private IEnumerator PlayCameraAnimation()
	{
		UIManager.instance.AddMouseTouchPtrListener((ref POINTER_INFO ptr) => SkipIntroSequence(ptr));
		sunShafts.sunShaftIntensity *= 1.5f;
		cameraAnim.Play();
		yield return new WaitForSeconds(cameraAnim.clip.length);
		UIManager.instance.RemoveMouseTouchPtrListener((ref POINTER_INFO ptr) => SkipIntroSequence(ptr));
		UIManager.instance.blockInput = false;
		sunShafts.sunShaftIntensity = origSunShaftIntensity;
		IntroSequenceCompleted();
	}

	private void IntroSequenceCompleted()
	{
		GameData.Instance.HasIntroSeqOccured = true;
		sunShafts.enabled = false;
		if (CameraScrollController.CameraAnimCompleted != null)
		{
			CameraScrollController.CameraAnimCompleted();
		}
	}

	private void SkipIntroSequence(POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			base.GetComponent<Animation>().Stop();
			ResetCamera(1f);
			IntroSequenceCompleted();
		UIManager.instance.RemoveMouseTouchPtrListener((ref POINTER_INFO ptr) => SkipIntroSequence(ptr));
		}
	}

	private void ResetCamera(float timeToReset)
	{
		iTween.MoveTo(cachedCameraTransform.gameObject, cameraStartingPos, timeToReset);
		iTween.RotateTo(cachedCameraTransform.gameObject, camaeraStartingRot.eulerAngles, timeToReset);
	}

	private void OnDisable()
	{
		MapLocation.MapButtonTapped -= HandleMapButtonTapped;
	}
}
