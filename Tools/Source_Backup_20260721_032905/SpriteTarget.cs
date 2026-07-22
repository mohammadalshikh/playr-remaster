using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpriteTarget : CircularTriggerOnHit
{
	public delegate void HandleTargetHit(SpriteTarget target);

	[SerializeField]
	protected SCORE_TYPE _scoreType;

	[SerializeField]
	private int _score;

	[SerializeField]
	private SFX _hitAudioFX;

	[SerializeField]
	private Vector2 _speed;

	[SerializeField]
	private Rect _movingBounds;

	[SerializeField]
	private LOOP_TYPE _loopScale;

	private float _xDir = 1f;

	private float _yDir = 1f;

	private Transform _cachedTransform;

	private Mesh _cachedMesh;

	private float _currentSize = 1f;

	private float _initialWidth;

	private float _initialheight;

	private float _initialRadius;

	private Color _cachedColor;

	protected PackedSprite _sprite;

	private bool _canMove = true;

	private BoxCollider _boxCollider;

	public SCORE_TYPE ScoreType => _scoreType;

	public int Score
	{
		get
		{
			return _score;
		}
		set
		{
			if (value > 0)
			{
				_score = value;
			}
		}
	}

	[method: MethodImpl(32)]
	public static event HandleTargetHit TargetHit;

	public virtual void Awake()
	{
		Initialize();
	}

	public virtual void Start()
	{
		SetSizeDefaults();
		UpdateTexture();
	}

	protected void Initialize()
	{
		_cachedTransform = base.transform;
		_sprite = base.transform.GetComponent<PackedSprite>();
		_cachedColor = _sprite.Color;
		_cachedMesh = base.transform.GetComponent<MeshFilter>().sharedMesh;
		_boxCollider = base.transform.GetComponent<BoxCollider>();
	}

	public virtual void Init(Vector2 speed, int score)
	{
		SetSpeed(speed);
		Score = score;
	}

	public virtual void RemoveFromView()
	{
		if (base.gameObject != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void SetSpeed(Vector2 newSpeed)
	{
		_speed = newSpeed;
	}

	public void SetPosition(Vector3 newPosition)
	{
		base.transform.position = newPosition;
		Move();
	}

	public void SizeSettings(float initialSize, float endSize, float time, LOOP_TYPE loopType)
	{
		if (initialSize != 0f)
		{
			if (initialSize == endSize)
			{
				SetSizeDefaults();
				SetSize(initialSize);
			}
			else if (endSize != 0f)
			{
				StopScaling();
				SetSizeDefaults();
				SetSize(initialSize);
				SetSizeDefaults();
				ScaleTo(endSize, time, loopType);
			}
		}
	}

	private void SetSizeDefaults()
	{
		_initialWidth = _sprite.width;
		_initialheight = _sprite.height;
		_initialRadius = _radius;
	}

	public virtual void OnEnable()
	{
		SoccerBall.StateHasChanged += HandleBallStateChanged;
	}

	public virtual void OnDisable()
	{
		SoccerBall.StateHasChanged -= HandleBallStateChanged;
	}

	private void Update()
	{
		if (_canMove && (_speed.x != 0f || _speed.y != 0f))
		{
			Move();
		}
	}

	protected virtual void HandleBallStateChanged(SoccerBall.STATE newState)
	{
		if (newState == SoccerBall.STATE.STATIC)
		{
			Reset();
		}
	}

	protected override void EnteredCircle(Collider other)
	{
		base.EnteredCircle(other);
		ValidHit();
		_canMove = false;
		StopScaling();
		TargetHitBroadcast(this);
		ShowCelebration(other.transform.position);
	}

	public void AutoDestruct()
	{
		if (base.IsActive)
		{
			base.IsActive = false;
			_hitPosition = base.transform.position;
			_accuracy = 1f;
			ValidHit();
			_canMove = false;
			StopScaling();
			TargetHitBroadcast(this);
			ShowCelebration(base.transform.position);
		}
	}

	protected virtual void TargetHitBroadcast(SpriteTarget target)
	{
		if (SpriteTarget.TargetHit != null)
		{
			SpriteTarget.TargetHit(target);
		}
	}

	private void PlayCustomHitSound()
	{
		if (_hitAudioFX.audioClip != null)
		{
			AudioManager.PlaySFX(_hitAudioFX, 0f);
		}
	}

	protected virtual void ShowCelebration(Vector3 celebrationPosition)
	{
		_sprite.SetColor(new Color(0f, 1f, 0.20392157f, 0.5f));
	}

	public override void Reset()
	{
		base.Reset();
		if (_sprite != null)
		{
			_sprite.SetColor(_cachedColor);
		}
		_canMove = true;
	}

	public void SetBounds(Rect bounds)
	{
		_movingBounds = bounds;
	}

	private void Move()
	{
		float num = _cachedMesh.bounds.size.x * _cachedTransform.localScale.x / 2f;
		float num2 = _cachedMesh.bounds.size.y * _cachedTransform.localScale.y / 2f;
		if (_cachedTransform.position.z > _movingBounds.x + _movingBounds.width / 2f - num || _cachedTransform.position.z < _movingBounds.x - _movingBounds.width / 2f + num)
		{
			Vector3 position = _cachedTransform.position;
			position.z = Mathf.Clamp(_cachedTransform.position.z, _movingBounds.x - _movingBounds.width / 2f + num, _movingBounds.x + _movingBounds.width / 2f - num);
			_cachedTransform.position = position;
			_xDir *= -1f;
		}
		if (_cachedTransform.position.y > _movingBounds.y + _movingBounds.height / 2f - num2 || _cachedTransform.position.y < _movingBounds.y - _movingBounds.height / 2f + num2)
		{
			Vector3 position2 = _cachedTransform.position;
			position2.y = Mathf.Clamp(_cachedTransform.position.y, _movingBounds.y - _movingBounds.height / 2f + num2, _movingBounds.y + _movingBounds.height / 2f - num2);
			_cachedTransform.position = position2;
			_yDir *= -1f;
		}
		base.transform.Translate(0f, _yDir * _speed.y * Time.deltaTime, (0f - _xDir) * _speed.x * Time.deltaTime, Space.World);
	}

	private void SetSize(float newSize)
	{
		_radius = _initialRadius * newSize;
		_sprite.SetSize(_initialWidth * newSize, _initialheight * newSize);
		_boxCollider.size = new Vector3(_initialheight * newSize + 0.1f, _initialheight * newSize + 0.1f, _boxCollider.size.z);
		_currentSize = newSize;
	}

	public void ScaleTo(float toScale, float time, LOOP_TYPE loopType)
	{
		float currentSize = _currentSize;
		iTween.ValueTo(base.gameObject, iTween.Hash("name", "scale", "from", currentSize, "to", toScale, "time", time, "ignoretimescale", false, "looptype", (iTween.LoopType)loopType, "onUpdate", (Action<object>)delegate(object x)
		{
			SetSize((float)x);
		}));
	}

	private void StopScaling()
	{
		iTween.StopByName(base.gameObject, "scale");
	}

	protected void SetTexture(string animationName, int frameIndex)
	{
		_sprite.SetFrame(animationName, frameIndex);
	}

	private void UpdateTexture()
	{
		if (_sprite.animations.Length != 0)
		{
			SCORE_TYPE scoreType = _scoreType;
			if (scoreType == SCORE_TYPE.MULTIPLIER)
			{
				SetTexture(_scoreType.ToString().ToLower(), Mathf.Clamp(Score, 2, 9) - 2);
			}
			else
			{
				SetTexture(_scoreType.ToString().ToLower(), 0);
			}
			_sprite.UpdateUVs();
		}
	}

	public void UpdateColor()
	{
		switch (_scoreType)
		{
		case SCORE_TYPE.POINTS:
			_sprite.SetColor(new Color(1f, 1f, 1f, 0.5f));
			break;
		case SCORE_TYPE.MULTIPLIER:
			_sprite.SetColor(Color.white);
			_score = GameData.MinigameObject.CurrentMinigame.CurrentMultiplier + 1;
			break;
		default:
			_sprite.SetColor(Color.white);
			break;
		}
		_cachedColor = _sprite.Color;
	}

	protected void SetTexture(int animationIndex, int frameIndex)
	{
		_sprite.SetFrame(animationIndex, frameIndex);
	}
}
