using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
	public enum STATE
	{
		STATIC,
		IN_AIR,
		BOUNCING
	}

	public enum BALL_EFFECTS
	{
		NONE,
		FIREBALL
	}

	public delegate void HandleBallStateChanged(STATE newState);

	public delegate void HandleCollision(Collision collision);

	private const float ANGLE_COMPENSATION = 0.75f;

	[SerializeField]
	private TrailRenderer[] _trailEffects;

	[SerializeField]
	private ParticleSystem[] _particleEffects;

	[SerializeField]
	private Material _lavaMaterial;

	[SerializeField]
	private SFX _shotSound = new SFX();

	[SerializeField]
	private SFX _shotFireSound = new SFX();

	[SerializeField]
	private FollowObject _shadowFollowObj;

	[SerializeField]
	private GameObject _fireBallEffect;

	protected STATE _state;

	private BALL_EFFECTS _currentEffects;

	private bool _isKicked;

	private bool _isFalling;

	private Material _cachedMaterial;

	private GameObject _currentVisualEffect;

	private TrailRenderer _currentTrail;

	private ParticleSystem _currentParticleEffect;

	private Transform _cachedTransform;

	public Transform CachedTransform => _cachedTransform;

	public STATE State => _state;

	public bool IsFalling => _isFalling;

	[method: MethodImpl(32)]
	public event Action OnBooster;

	[method: MethodImpl(32)]
	public event Action OnSlowDown;

	[method: MethodImpl(32)]
	public static event HandleBallStateChanged StateHasChanged;

	[method: MethodImpl(32)]
	public static event HandleCollision BallFirstCollision;

	private void Awake()
	{
		GameData.CheckInstance();
		GameData.Ball = this;
		_cachedMaterial = base.renderer.material;
		_currentTrail = _trailEffects[0];
		_currentEffects = BALL_EFFECTS.NONE;
		_cachedTransform = base.transform;
	}

	private void Start()
	{
		_shadowFollowObj.m_target = _cachedTransform;
	}

	public void Reset()
	{
		SwitchState(STATE.STATIC);
		base.rigidbody.velocity = Vector3.zero;
		base.rigidbody.angularVelocity = Vector3.zero;
		StopCurl();
		_isFalling = false;
		_isKicked = false;
	}

	protected virtual void BallStateHasChanged(STATE newState)
	{
		if (SoccerBall.StateHasChanged != null)
		{
			SoccerBall.StateHasChanged(newState);
		}
	}

	private void SwitchState(STATE newState)
	{
		if (_state != newState)
		{
			_state = newState;
			BallStateHasChanged(_state);
			switch (_state)
			{
			case STATE.STATIC:
				_isKicked = false;
				break;
			case STATE.IN_AIR:
				break;
			case STATE.BOUNCING:
				StopCurl();
				break;
			}
		}
	}

	public void ShootBall(Vector2 touchStart, Vector2 touchEnd)
	{
		Ray ray = Camera.mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
		float magnitude = (touchEnd - touchStart).magnitude;
		float value = Vector2.Angle(Vector2.up, touchEnd - touchStart);
		value = Mathf.LerpAngle(0f, GameData.MinigameObject.CurrentMinigame.MaxShootAngle(), Mathf.InverseLerp(0f, 45f, value));
		value = ((!(Vector3.Cross(Vector2.up, touchEnd - touchStart).z > 0f)) ? value : (0f - value));
		value *= 0.75f;
		float num = GameData.MinigameObject.CurrentMinigame.BallShootingHeight(magnitude);
		float num2 = GameData.MinigameObject.CurrentMinigame.BallShootingSpeed(magnitude, num);
		if (_currentTrail != null)
		{
			_currentTrail.enabled = true;
		}
		Quaternion quaternion = Quaternion.AngleAxis(value, Vector3.up);
		Vector3 direction = ray.direction;
		direction = quaternion * direction;
		direction.y = num;
		base.rigidbody.velocity = direction * num2;
		PlayShotSound();
		_isKicked = true;
	}

	public void AddCurl(Vector2 dir, float magnitude)
	{
		if (_state == STATE.BOUNCING)
		{
			StopCurl();
			return;
		}
		if (_cachedTransform.position.y < 0.3f || base.rigidbody.velocity.sqrMagnitude < 0.01f || base.constantForce.force.sqrMagnitude > magnitude * magnitude)
		{
			base.constantForce.force = Vector3.zero;
			return;
		}
		dir = dir.normalized;
		dir.x = 0f - dir.x;
		base.constantForce.force = new Vector3(0f, dir.y, dir.x * magnitude);
		base.constantForce.torque = new Vector3(0f, 0f - dir.x * magnitude, dir.y);
	}

	public void ApplyBooster(float deltaBooster)
	{
		Vector3 force = base.rigidbody.velocity.normalized * GameData.MinigameObject.CurrentMinigame.FlickBoosterFactor * Mathf.Sign(deltaBooster);
		force.y = 0f;
		base.rigidbody.AddForce(force, ForceMode.Impulse);
		if (deltaBooster > 0f)
		{
			BoosterEvents();
		}
		else
		{
			SlowDownEvents();
		}
	}

	private void BoosterEvents()
	{
		if (this.OnBooster != null)
		{
			this.OnBooster();
		}
	}

	private void SlowDownEvents()
	{
		if (this.OnSlowDown != null)
		{
			this.OnSlowDown();
		}
	}

	public void SetEffects(BALL_EFFECTS effects)
	{
		if (_currentEffects != effects)
		{
			_currentEffects = effects;
			if (effects == BALL_EFFECTS.FIREBALL)
			{
				_currentVisualEffect = _fireBallEffect;
				Transform transform = _currentVisualEffect.transform;
				transform.localRotation = _cachedTransform.rotation;
				transform.position = _cachedTransform.position;
				_currentVisualEffect.active = true;
				base.renderer.material = _lavaMaterial;
				_currentTrail = _trailEffects[(int)effects];
				_currentTrail.enabled = true;
				_currentParticleEffect = _particleEffects[0];
				_currentParticleEffect.Play();
			}
			else
			{
				base.renderer.material = _cachedMaterial;
				_currentTrail = _trailEffects[(int)effects];
				_currentParticleEffect = null;
				_currentVisualEffect.active = false;
			}
		}
	}

	private void PlayShotSound()
	{
		BALL_EFFECTS currentEffects = _currentEffects;
		if (currentEffects == BALL_EFFECTS.FIREBALL)
		{
			AudioManager.PlaySFX(_shotFireSound, 0f);
		}
		else
		{
			AudioManager.PlaySFX(_shotSound, 0f);
		}
	}

	private void StopCurl()
	{
		base.constantForce.constantForce.force = Vector3.zero;
		base.constantForce.torque = Vector3.zero;
	}

	private void CalculateHitPosition(float shootingAngle)
	{
		float magnitude = base.rigidbody.velocity.magnitude;
		float num = Mathf.Sin(base.rigidbody.velocity.y / base.rigidbody.velocity.magnitude);
		float num2 = magnitude * Mathf.Cos(num);
		float num3 = magnitude * Mathf.Sin(num);
		float num4 = 2f * num3 / 9.81f;
		float num5 = num2 * num4 * 0.63f;
		MonoBehaviour.print(" --- distance = " + num5);
		float num6 = magnitude * magnitude * Mathf.Sin(2f * num);
		num6 /= 9.81f;
		float num7 = Mathf.Cos(shootingAngle * ((float)Math.PI / 180f)) * num5;
		float num8 = (0f - Mathf.Sin(shootingAngle * ((float)Math.PI / 180f))) * num5;
		num7 += GameData.MinigameObject.CurrentMinigame.BallOffset.x;
		num8 += GameData.MinigameObject.CurrentMinigame.BallOffset.z;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (GameData.MinigameObject.CurrentMinigame != null)
		{
			GameData.MinigameObject.CurrentMinigame.OnBallCollision(collision, this);
		}
		if (_currentParticleEffect != null && _state == STATE.STATIC)
		{
			_currentParticleEffect.enableEmission = true;
		}
		if (_state == STATE.IN_AIR)
		{
			BallHasHitCollider(collision);
		}
		if (_currentTrail != null && _currentTrail.enabled)
		{
			_currentTrail.enabled = false;
		}
	}

	private void BallHasHitCollider(Collision collision)
	{
		if (SoccerBall.BallFirstCollision != null)
		{
			SoccerBall.BallFirstCollision(collision);
		}
		if (_currentParticleEffect != null)
		{
			_currentParticleEffect.enableEmission = false;
		}
		SwitchState(STATE.BOUNCING);
	}

	private void FixedUpdate()
	{
		switch (_state)
		{
		case STATE.IN_AIR:
			if (base.rigidbody.velocity.y <= 0f && !_isFalling)
			{
				_isFalling = true;
			}
			break;
		case STATE.BOUNCING:
			if (base.rigidbody.velocity.magnitude < GameData.MinigameObject.CurrentMinigame.EndShotBallSpeed && GameData.PlayerControl.HasKicked() && !Minigame.NextChanceCalled && _cachedTransform.position.y <= 0.3f)
			{
				GameData.MinigameObject.NextChance();
				base.rigidbody.velocity = Vector3.zero;
			}
			break;
		case STATE.STATIC:
			if (_isKicked && _cachedTransform.position.y >= 0.3f)
			{
				SwitchState(STATE.IN_AIR);
			}
			break;
		}
	}
}
