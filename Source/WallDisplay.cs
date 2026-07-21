using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDisplay : MonoBehaviour
{
	public const float WALL_POSITION = 15f;

	public const int FIRST_MULTIPLIER_LIST_INDEX = 9;

	public const int LAST_MULTIPLIER_LIST_INDEX = 16;

	public GameObject[] targetPrefabs;

	public Texture2D minMultiplierTexture;

	public UIPanel missedShotSprite;

	public static WallDisplay Instance;

	private static int target_render_order = 3002;

	private bool _wallHasBeenHit;

	private List<Transform> _hiddenTargets = new List<Transform>();

	private MinigameWall _minigame;

	private List<SCORE_TYPE> _onScreenTargets = new List<SCORE_TYPE>();

	public bool WallHasBeenHit => _wallHasBeenHit;

	public MinigameWall WallGame
	{
		get
		{
			return _minigame;
		}
		set
		{
			_minigame = value;
		}
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void OnEnable()
	{
		WallTarget.StateChanged += AvailableTargets;
	}

	private void OnDisable()
	{
		WallTarget.StateChanged -= AvailableTargets;
	}

	private void AvailableTargets(SCORE_TYPE type, bool bAvailable)
	{
		if (bAvailable)
		{
			_onScreenTargets.Add(type);
		}
		else
		{
			_onScreenTargets.Remove(type);
		}
	}

	public void SetTargets(List<WallTargetSettings> targets)
	{
		if (targets.Count > 0)
		{
			foreach (WallTargetSettings target in targets)
			{
				target.activity.positioning.boundBox = WallGame.CurrentWallLevel().SetGlobalBoundBox(target);
				StartCoroutine(SpawnWithDelay(target));
			}
			return;
		}
		Debug.LogError("No walltargets set yet, please add targets for the current MinigameWall");
	}

	public IEnumerator SpawnWithDelay(WallTargetSettings target)
	{
		yield return new WaitForSeconds(target.life.initialDelay);
		int index = GetPrefabIndexForTarget(target);
		if (index < targetPrefabs.Length)
		{
			GameObject t = (GameObject)Object.Instantiate(targetPrefabs[index]);
			t.transform.parent = base.transform;
			t.GetComponent<WallTarget>().SetTarget(target.id, target.scoreSettings, target.life, target.activity);
		}
	}

	private int GetPrefabIndexForTarget(WallTargetSettings target)
	{
		int num = 0;
		if (target.id <= TARGET_TYPE.MULTIPLIER)
		{
			return (target.id == TARGET_TYPE.MULTIPLIER) ? multiplierIndex() : ((int)target.id);
		}
		return (int)(7 + target.id);
	}

	private int multiplierIndex()
	{
		return Mathf.Clamp(9 + (GameData.MinigameObject.CurrentMinigame.CurrentMultiplier - 1), 9, 16);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!MinigameWall.TargetHitOnCurrentShot)
		{
			AudioManager.PlaySFX(AudioManager.Instance.audioCollection.minigameWall.missedShot, 0f);
			missedShotSprite.transform.position = collision.contacts[0].point;
			StartCoroutine(GameData.GUIManager.AnimateHUDPanel(missedShotSprite, 0f));
		}
		if (_minigame != null)
		{
			WallGame.WallHasBeenHit(collision.contacts[0].point);
		}
		_wallHasBeenHit = true;
	}

	public bool IsBonusTargetOnScreen(SCORE_TYPE type)
	{
		return _onScreenTargets.Contains(type);
	}

	public static int GetTopRenderOrderID()
	{
		return target_render_order + 1;
	}

	public void UpdateWall()
	{
		_wallHasBeenHit = false;
	}

	public int Children()
	{
		return base.transform.childCount;
	}

	public void CreateNewChance()
	{
		GameData.MinigameObject.CurrentMinigame.GenerateShootingPosition();
	}

	public void RemoveChildren()
	{
		if (!(Instance != null))
		{
			return;
		}
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public void DestroyHiddenTargets()
	{
		foreach (Transform hiddenTarget in _hiddenTargets)
		{
			if (hiddenTarget != null)
			{
				Object.Destroy(hiddenTarget.gameObject);
			}
		}
	}

	public bool ActiveTargets()
	{
		int num = 0;
		foreach (Transform item in base.transform)
		{
			if (item.GetComponent<WallTarget>().IsActive)
			{
				num++;
			}
			else
			{
				_hiddenTargets.Add(item);
			}
		}
		return num > 0;
	}

	public bool ActiveMultiplier()
	{
		int num = 2;
		foreach (Transform item in base.transform)
		{
			WallTarget component = item.GetComponent<WallTarget>();
			if (component.id == TARGET_TYPE.MULTIPLIER)
			{
				num = component.Trigger.Points;
			}
		}
		return num > 2;
	}

	public void SwapMultiplier()
	{
		foreach (Transform item in base.transform)
		{
			WallTarget component = item.GetComponent<WallTarget>();
			if (component.id == TARGET_TYPE.MULTIPLIER)
			{
				item.GetComponent<PackedSprite>().PlayAnim("multiplier_x2");
				component.CurrentScore = 2;
			}
		}
	}

	public bool ExistingMultiplier()
	{
		foreach (Transform item in base.transform)
		{
			WallTarget component = item.GetComponent<WallTarget>();
			if (component.id == TARGET_TYPE.MULTIPLIER)
			{
				return true;
			}
		}
		return false;
	}

	public void AddExtraTargets(List<WallTargetSettings> newTargets)
	{
		if (newTargets.Count != 0)
		{
			SetTargets(newTargets);
		}
	}
}
