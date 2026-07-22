using System;
using UnityEngine;

public class DollyEffect : MonoBehaviour
{
	[SerializeField]
	private Transform _target;

	private float initHeightAtDist;

	private bool isEnabled;

	private Vector3 initialPos;

	private float initialFOV;

	public static DollyEffect Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void SetTargetPosition(Vector3 position)
	{
		_target.position = position;
	}

	private float FrustumHeightAtDistance(float distance)
	{
		return 2f * distance * Mathf.Tan(base.camera.fieldOfView * 0.5f * ((float)Math.PI / 180f));
	}

	private float FOVForHeightAndDistance(float height, float distance)
	{
		return 2f * Mathf.Atan(height * 0.5f / distance) * 57.29578f;
	}

	public void Init()
	{
		initialFOV = base.camera.fieldOfView;
		initHeightAtDist = FrustumHeightAtDistance(Vector3.Distance(base.transform.position, _target.position));
		isEnabled = true;
	}

	public void Stop()
	{
		isEnabled = false;
		if (initialFOV > 0f)
		{
			base.camera.fieldOfView = initialFOV;
		}
	}

	public void Enable(bool bEnable)
	{
		isEnabled = bEnable;
	}

	private void Start()
	{
		initialFOV = base.camera.fieldOfView;
	}

	private void Update()
	{
		if (isEnabled && GameData.MinigameObject.CurrentMinigame.DollyEffectActive)
		{
			base.camera.fieldOfView = FOVForHeightAndDistance(initHeightAtDist, Vector3.Distance(base.transform.position, _target.position));
		}
	}
}
