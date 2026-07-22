using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGamePopup : ScreenPopup
{
	[SerializeField]
	private PackedSprite gameUnlockIcon;

	[SerializeField]
	private PackedSprite locationUpgradeIcon;

	[SerializeField]
	private SpriteText newGameMessage;

	[SerializeField]
	private SpriteText rankText;

	[SerializeField]
	private SpriteText levelText;

	[SerializeField]
	private SpriteText coinRewardValueText;

	[SerializeField]
	private UIButton closeButton;

	[SerializeField]
	private ParticleSystem glitterParticles;

	[SerializeField]
	private UIPanel coinRewardPanel;

	private bool hasLevelledUp;

	private bool isGameUnlock;

	public override bool CanShow => hasLevelledUp;

	public override void Dismiss()
	{
		hasLevelledUp = false;
		_isActive = CanShow;
		DismissPanel();
		if (CanShow)
		{
			Invoke("Bring", 0.5f);
		}
		else
		{
			Reset();
		}
	}

	public override void Bring()
	{
		base.Bring();
		glitterParticles.Play();
	}

	protected override void Reset()
	{
		base.Reset();
		glitterParticles.Stop();
		glitterParticles.Clear();
		isGameUnlock = false;
		locationUpgradeIcon.GetComponent<Renderer>().enabled = false;
		gameUnlockIcon.GetComponent<Renderer>().enabled = false;
		newGameMessage.Text = string.Empty;
	}

	private void Start()
	{
		closeButton.AddInputDelegate(HandleCloseButtonInput);
		base.Panel.GetTransition(UIPanelManager.SHOW_MODE.BringInForward).AddTransitionEndDelegate(delegate
		{
			StartCoroutine(DisplayLevelUpSequence());
		});
	}

	private void OnEnable()
	{
		PlayerManager.LevelledUp += HandleLevelledUp;
	}

	private void HandleCloseButtonInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			Dismiss();
		}
	}

	private void OnDisable()
	{
		PlayerManager.LevelledUp -= HandleLevelledUp;
	}

	private void HandleLevelledUp(int level)
	{
		levelText.Text = string.Concat("Level ", Color.yellow, level.ToString(), Color.white, " Unlocked!");
		rankText.Text = string.Concat("Rank: ", Color.yellow, GameData.PlayerStats.Rank);
		coinRewardValueText.Text = GameData.PlayerStats.CoinRewardForLevelUp.ToString();
		hasLevelledUp = true;
		Dictionary<int, GameData.GAME_LOCATION> mapUpgrades = GameData.Instance.MapUpgrades;
		foreach (KeyValuePair<int, GameData.GAME_LOCATION> item in mapUpgrades)
		{
			if (item.Key == level)
			{
				isGameUnlock = false;
				locationUpgradeIcon.PlayAnim(GameData.GetProperNameForLocation(item.Value));
				newGameMessage.Text = string.Concat("The ", Color.yellow, GameData.GetProperNameForLocation(item.Value), Color.white, " has just been upgraded!");
				return;
			}
		}
		foreach (GameInfo game in GameData.PlayerStats.Games)
		{
			if (game.UnlockedAt == GameData.PlayerStats.CurrentLevel)
			{
				gameUnlockIcon.SetFrame(game.Name.ToLower(), 0);
				newGameMessage.Text = string.Concat("You've unlocked ", Color.yellow, game.GameType, Color.white, " in ", Color.yellow, GameData.FormattedGameLocation(game.Location));
				isGameUnlock = true;
			}
		}
	}

	private IEnumerator DisplayLevelUpSequence()
	{
		coinRewardPanel.BringIn();
		yield return new WaitForSeconds(1f);
		coinRewardPanel.Dismiss();
		yield return new WaitForSeconds(1f);
		PackedSprite p = ((!isGameUnlock) ? locationUpgradeIcon : gameUnlockIcon);
		if ((bool)p)
		{
			p.GetComponent<Renderer>().enabled = true;
			iTween.ValueTo(p.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", 1f, "onUpdate", (Action<object>)delegate(object x)
			{
				p.SetColor(new Color(p.Color.r, p.Color.g, p.Color.b, (float)x));
			}));
		}
	}
}
