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
		return 2f * distance * Mathf.Tan(base.GetComponent<Camera>().fieldOfView * 0.5f * ((float)Math.PI / 180f));
	}

	private float FOVForHeightAndDistance(float height, float distance)
	{
		return 2f * Mathf.Atan(height * 0.5f / distance) * 57.29578f;
	}

	public void Init()
	{
		initialFOV = base.GetComponent<Camera>().fieldOfView;
		initHeightAtDist = FrustumHeightAtDistance(Vector3.Distance(base.transform.position, _target.position));
		isEnabled = true;
	}

	public void Stop()
	{
		isEnabled = false;
		if (initialFOV > 0f)
		{
			base.GetComponent<Camera>().fieldOfView = initialFOV;
		}
	}

	public void Enable(bool bEnable)
	{
		isEnabled = bEnable;
	}

	private void Start()
	{
		initialFOV = base.GetComponent<Camera>().fieldOfView;
	}

	private void Update()
	{
		if (isEnabled && GameData.MinigameObject.CurrentMinigame.DollyEffectActive)
		{
			base.GetComponent<Camera>().fieldOfView = FOVForHeightAndDistance(initHeightAtDist, Vector3.Distance(base.transform.position, _target.position));
		}
	}
}
