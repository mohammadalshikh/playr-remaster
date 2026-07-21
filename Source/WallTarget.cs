using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WallTarget : MinigameTarget
{
	public delegate void HandleStateChanged(SCORE_TYPE type, bool available);

	public const float WALL_TOP_LIMIT = 2.8f;

	private const float WALL_HEIGHT_OFFSET = 0.7f;

	private const float WALL_OFFSET = 0.03f;

	private const float WALL_VERTICAL_CENTER = 1.35f;

	private const float RESPAWN_TIME_CRITERIA = 2f;

	public TARGET_TYPE id;

	[SerializeField]
	private WallTargetTrigger _trigger;

	public SFX onAwakeAudio;

	private PackedSprite targetSprite;

	private Transform _transform;

	private ScoreSettings _scoreSettings = new ScoreSettings();

	private Activity _activity = new Activity();

	private Vector3 _initialPosition;

	private float _xMove;

	private float _yMove;

	private Rect _boundBox;

	private Vector3 _boundBoxCenter;

	private Mesh _mesh;

	private bool _pauseMovement;

	private bool _pauseAnimation;

	private Life _life;

	private bool _respawnForever;

	private bool _active;

	private float _removeDelay;

	private bool _wasHit;

	private static float[] spriteSizes = new float[5] { 1.6f, 1.2f, 1.4f, 1.8f, 2f };

	private static float[] targetSizes = new float[5] { 1.1f, 0.8f, 1f, 1.2f, 1.4f };

	private float MeshWidthOffset => _mesh.bounds.size.x * _transform.localScale.x / 2f;

	public Life Life => _life;

	public bool IsActive
	{
		get
		{
			if (!_respawnForever && _life.livesNumber == 0)
			{
				return false;
			}
			if (_life.respawnTime > 2f || _life.respawnTime == 0f)
			{
				return _active;
			}
			return true;
		}
	}

	public WallTargetTrigger Trigger => _trigger;

	[method: MethodImpl(32)]
	public static event HandleStateChanged StateChanged;

	private void Start()
	{
		_transform = base.transform;
		_mesh = base.transform.GetComponent<MeshFilter>().sharedMesh;
		targetSprite = base.transform.GetComponent<PackedSprite>();
		float num = ((!(_boundBox.width > 0f)) ? Random.Range(-8.5f, 8.5f) : Random.Range((0f - _boundBox.width) / 2f + MeshWidthOffset, _boundBox.width / 2f - MeshWidthOffset));
		float num2 = ((!(Mathf.Abs(_activity.positioning.horizontal) > 0f)) ? num : _activity.positioning.horizontal);
		base.transform.position += new Vector3(15.77f, 0.7f * (float)_activity.positioning.vertical, 0f - num2);
		_initialPosition = _transform.position;
		_boundBoxCenter = new Vector3(base.transform.position.x, 1.35f, _boundBox.x);
		if (_boundBox.width == 0f)
		{
			_boundBox.width = 17f;
		}
	}

	private void OnDisable()
	{
		StateHasChanged(available: false);
	}

	private void Update()
	{
		if (!_pauseMovement || GameData.IsPaused)
		{
			MoveTarget();
			CheckPosition();
		}
	}

	private void StateHasChanged(bool available)
	{
		if (WallTarget.StateChanged != null)
		{
			WallTarget.StateChanged(_scoreType, available);
		}
	}

	private void ResetTarget()
	{
		_pauseMovement = !(Mathf.Abs(_activity.speed.x) > 0f) && !(Mathf.Abs(_activity.speed.y) > 0f);
		_hasBeenHit = false;
		_wasHit = false;
	}

	public void SetTarget(TARGET_TYPE _id, ScoreSettings _scoreSettings, Life lf, Activity _activity)
	{
		targetSprite = base.transform.GetComponent<PackedSprite>();
		id = _id;
		SetScoreSettings(_scoreSettings);
		ActivateCollider(bEnable: true);
		SetLifeSettings(lf);
		SetActivitySettings(_activity);
		StateHasChanged(available: true);
		if (onAwakeAudio.audioClip != null)
		{
			AudioManager.PlaySFX(onAwakeAudio, 0f);
		}
	}

	private void SetScoreSettings(ScoreSettings _settings)
	{
		_scoreSettings = _settings;
		if (id == TARGET_TYPE.MULTIPLIER || _scoreSettings.minScore == 0)
		{
			_scoreSettings.minScore = _currentScore;
		}
		else
		{
			_currentScore = _scoreSettings.minScore;
		}
		if (_scoreSettings.maxScore == 0)
		{
			_scoreSettings.maxScore = _scoreSettings.minScore;
		}
	}

	private void SetLifeSettings(Life _lf)
	{
		_life = _lf;
		_respawnForever = ((_life.livesNumber == 0) ? true : false);
		if (_life.lifeTime > 0f)
		{
			_life.lifeTime += Random.Range(0.01f, 0.1f);
			RemoveTarget(_life.lifeTime);
		}
	}

	private int GetScoreAtCurrentSize(float currentSize)
	{
		if (id == TARGET_TYPE.MULTIPLIER)
		{
			return _currentScore;
		}
		float t = Mathf.InverseLerp(targetSizes[(int)_activity.size.initialSize], targetSizes[(int)_activity.size.endSize], currentSize);
		return (int)Mathf.Round(Mathf.Lerp(_scoreSettings.minScore, _scoreSettings.maxScore, t));
	}

	private bool IsRespawning()
	{
		return _life.respawnTime > 0f && (_life.livesNumber > 0 || _respawnForever);
	}

	private void Respawn()
	{
		ResetTarget();
		_life.livesNumber--;
		EnableTarget(bEnabled: true);
		iTween.Resume(base.transform.gameObject);
		if (_life.lifeTime > 0f)
		{
			RemoveTarget(_life.lifeTime);
		}
	}

	private void SetActivitySettings(Activity act)
	{
		_activity = act;
		_xMove = _activity.speed.x;
		_yMove = _activity.speed.y;
		if (_activity.positioning.boundBox != 0f)
		{
			_boundBox.width = Mathf.Clamp(_activity.positioning.boundBox, 0f, 17f);
		}
		SetSizing();
	}

	private void MoveOnePosition()
	{
		base.transform.Translate(0f, _yMove * Time.deltaTime * MinigameWall.m_game_speed, (0f - _xMove) * Time.deltaTime * MinigameWall.m_game_speed, Space.World);
	}

	private void CheckPosition()
	{
		if (_transform.position.z > _boundBoxCenter.z + _boundBox.width / 2f - _mesh.bounds.size.x * _transform.localScale.x / 2f)
		{
			_transform.position = new Vector3(_transform.position.x, _transform.position.y, _boundBoxCenter.z + _boundBox.width / 2f - _mesh.bounds.size.x * _transform.localScale.x / 2f);
			_xMove *= -1f;
		}
		if (_transform.position.z < _boundBoxCenter.z - _boundBox.width / 2f + _mesh.bounds.size.x * _transform.localScale.x / 2f)
		{
			_transform.position = new Vector3(_transform.position.x, _transform.position.y, _boundBoxCenter.z - _boundBox.width / 2f + _mesh.bounds.size.x * _transform.localScale.x / 2f);
			_xMove *= -1f;
		}
		if (_transform.position.y > 2.8f - _mesh.bounds.size.y * _transform.localScale.y / 2f)
		{
			_transform.position = new Vector3(_transform.position.x, 2.8f - _mesh.bounds.size.y * _transform.localScale.y / 2f, _transform.position.z);
			_yMove *= -1f;
		}
		if (_transform.position.y < _mesh.bounds.size.y * _transform.localScale.y / 2f)
		{
			_transform.position = new Vector3(_transform.position.x, _mesh.bounds.size.y * _transform.localScale.y / 2f, _transform.position.z);
			_yMove *= -1f;
		}
	}

	private void MoveTarget()
	{
		if (!_pauseMovement && !GameData.IsPaused)
		{
			MoveOnePosition();
		}
	}

	private void SetSizing()
	{
		if (_activity.size.endSize != _activity.size.initialSize)
		{
			base.transform.localScale = new Vector3(targetSizes[(int)_activity.size.initialSize], targetSizes[(int)_activity.size.initialSize], 0f);
			iTween.ScaleTo(base.gameObject, iTween.Hash("name", "sizeAnimation", "scale", new Vector3(targetSizes[(int)_activity.size.endSize], targetSizes[(int)_activity.size.endSize], 1f), "easeType", "linear", "loopType", (!_activity.size.loop) ? "none" : "pingPong", "time", _activity.size.duration, "ignoretimescale", false, "delay", (!(_activity.size.startDelay > 0f)) ? 0.05f : _activity.size.startDelay));
		}
		else
		{
			_trigger.SetSize(spriteSizes[(int)_activity.size.initialSize]);
			targetSprite.SetSize(spriteSizes[(int)_activity.size.initialSize], spriteSizes[(int)_activity.size.initialSize]);
		}
	}

	private void ActivateCollider(bool bEnable)
	{
		if (_trigger.collider != null)
		{
			_trigger.collider.enabled = bEnable;
		}
		_active = bEnable;
	}

	public void RemoveTarget(float delay)
	{
		StopCoroutine("RemoveFromView");
		_removeDelay = delay;
		StartCoroutine(RemoveFromView());
	}

	public void OnHitEnter(WallTargetTrigger hitTarget)
	{
		if (!_wasHit)
		{
			_currentScore = GetScoreAtCurrentSize(_transform.localScale.y);
			_wasHit = true;
			_pauseMovement = true;
			Object.Instantiate(GameData.MinigameObject.WallData.implosionPrefab, base.transform.position, Quaternion.identity);
			DisplayTargetReward(hitTarget, 0.2f + MinigameWall.HitCount * 0.8f);
			RemoveTarget(0f);
		}
	}

	private IEnumerator RemoveFromView()
	{
		iTween.Pause(base.transform.gameObject);
		yield return new WaitForSeconds(_removeDelay);
		ActivateCollider(bEnable: false);
		yield return StartCoroutine(HitAnimation());
		EnableTarget(bEnabled: false);
		if (IsRespawning())
		{
			base.transform.position = new Vector3(_initialPosition.x, _initialPosition.y, Random.Range(-10f, 10f));
			yield return new WaitForSeconds(_life.respawnTime);
			Respawn();
			yield break;
		}
		if (ParentWall().Children() == 1)
		{
			ParentWall().CreateNewChance();
		}
		Object.Destroy(base.gameObject);
	}

	private IEnumerator HitAnimation()
	{
		yield return StartCoroutine(SpriteBlinkAnimation(targetSprite, 0.5f, 1f, 0.2f));
		FadeSprite.Do(dest: new Color(targetSprite.Color.r, targetSprite.Color.b, targetSprite.Color.g, 0f), sprt: targetSprite, mode: EZAnimation.ANIM_MODE.To, interp: EZAnimation.linear, dur: 0.3f, delay: 0f, startDel: null, del: null);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator SpriteBlinkAnimation(PackedSprite sprite, float minAlpha, float maxAlpha, float duration)
	{
		Color minColor = new Color(sprite.Color.r, sprite.Color.g, sprite.Color.b, minAlpha);
		Color maxColor = new Color(sprite.Color.r, sprite.Color.g, sprite.Color.b, maxAlpha);
		float animTime3;
		for (animTime3 = 0f; animTime3 < duration; animTime3 += Time.deltaTime)
		{
			animTime3 += Time.deltaTime;
			yield return new WaitForSeconds(0.05f);
			FadeSprite.Do(targetSprite, EZAnimation.ANIM_MODE.To, minColor, EZAnimation.linear, 0.05f, 0f, null, null);
			animTime3 += Time.deltaTime;
			yield return new WaitForSeconds(0.05f);
			FadeSprite.Do(targetSprite, EZAnimation.ANIM_MODE.To, maxColor, EZAnimation.linear, 0.05f, 0f, null, null);
		}
		targetSprite.Color = maxColor;
	}

	private void EnableTarget(bool bEnabled)
	{
		if (bEnabled)
		{
			Color color = targetSprite.Color;
			color.a = 1f;
			targetSprite.Color = color;
			AudioManager.PlaySFX(onAwakeAudio, 0f);
		}
		else
		{
			AudioManager.StopSound(onAwakeAudio);
		}
		targetSprite.Hide(!bEnabled);
		ActivateCollider(bEnabled);
	}

	private WallDisplay ParentWall()
	{
		return WallDisplay.Instance;
	}

	private void DisplayTargetReward(WallTargetTrigger hitTarget, float delay)
	{
		string empty = string.Empty;
		GameData.MinigameObject.CurrentMinigame.IncrementStreakCounter();
		switch (_scoreType)
		{
		case SCORE_TYPE.POINTS:
			empty = empty + "+" + hitTarget.Points;
			GameData.MinigameObject.CurrentMinigame.AddScore(hitTarget.Points * GameData.MinigameObject.CurrentMinigame.CurrentMultiplier, ApplyStatModBonuses: true, MinigameWall.HitCount < 1f);
			break;
		case SCORE_TYPE.LIVES:
			GameData.MinigameObject.CurrentMinigame.AddLife();
			break;
		case SCORE_TYPE.SECONDS:
			empty += "5s";
			GameData.MinigameObject.CurrentMinigame.AddTimeBonus(6);
			break;
		case SCORE_TYPE.MULTIPLIER:
			GameData.MinigameObject.CurrentMinigame.IncrementMultiplier();
			_currentScore = GameData.MinigameObject.CurrentMinigame.CurrentMultiplier;
			break;
		case SCORE_TYPE.SHIELD:
			GameData.MinigameObject.CurrentMinigame.AwardShield();
			break;
		}
	}
}
