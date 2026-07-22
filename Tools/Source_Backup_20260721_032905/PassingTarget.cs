using System.Runtime.CompilerServices;
using UnityEngine;

public class PassingTarget : CircularTriggerOnHit
{
	public delegate void HandleTargetHit(PassingTarget target);

	public delegate void HandleTargetRemoval(PassingTarget target);

	[SerializeField]
	private GameObject _nukePrefab;

	[SerializeField]
	private GameObject _hitMark;

	private PackedSprite _sprite;

	private int _score = 1000;

	private bool _powerUpAssigned;

	public bool PowerUpAssigned
	{
		get
		{
			return _powerUpAssigned;
		}
		set
		{
			_powerUpAssigned = value;
		}
	}

	public int Score => Mathf.RoundToInt((float)_score * base.Accuracy);

	[method: MethodImpl(32)]
	public static event HandleTargetHit TargetHitEvent;

	[method: MethodImpl(32)]
	public static event HandleTargetRemoval TargetRemovalEvent;

	private void Start()
	{
		_sprite = base.transform.GetComponent<PackedSprite>();
		PassingLevel.LevelUpdateEvent += LevelUpdateDelegate;
	}

	private void OnDisable()
	{
		PassingLevel.LevelUpdateEvent -= LevelUpdateDelegate;
	}

	protected override void EnteredCircle(Collider other)
	{
		base.EnteredCircle(other);
		_sprite.SetColor(new Color(0f, 1f, 0.20392157f, 1f));
		base.IsActive = false;
		Object.Instantiate(_nukePrefab, base.transform.position + Vector3.up * 0.2f, Quaternion.identity);
		_hitMark.transform.position = base.HitPosition;
		_hitMark.renderer.material.renderQueue = 3100;
		_hitMark.renderer.enabled = true;
		if (PassingTarget.TargetHitEvent != null)
		{
			PassingTarget.TargetHitEvent(this);
		}
	}

	private void LevelUpdateDelegate()
	{
		_powerUpAssigned = false;
		if (!base.IsActive)
		{
			DestroyTarget();
		}
	}

	public void SetPosition(Vector2 newPosition)
	{
		base.transform.position = new Vector3(newPosition.y, base.transform.position.y, newPosition.x);
		_score = CalculateScore(newPosition);
	}

	private int CalculateScore(Vector2 currentPosition)
	{
		int num = (int)Mathf.Lerp(500f, 3000f, Mathf.InverseLerp(0f, 15f, Mathf.Abs(currentPosition.x))) / 2;
		int num2 = (int)Mathf.Lerp(500f, 3000f, Mathf.InverseLerp(10f, 40f, currentPosition.y)) / 2;
		return num + num2;
	}

	public void RemoveFromView()
	{
		if (PassingTarget.TargetRemovalEvent != null)
		{
			PassingTarget.TargetRemovalEvent(this);
		}
		DestroyTarget();
	}

	private void DestroyTarget()
	{
		if (base.gameObject != null)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
