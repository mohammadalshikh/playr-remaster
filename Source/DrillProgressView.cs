using System;
using System.Collections;
using UnityEngine;

public class DrillProgressView : MonoBehaviour
{
	[SerializeField]
	private UIProgressBar[] progressBars;

	[SerializeField]
	private SpriteText[] thresholdTexts;

	[SerializeField]
	private PackedSprite[] trophies;

	[SerializeField]
	private Transform scoreArrowTransform;

	[SerializeField]
	private Transform highScoreArrowTransform;

	[SerializeField]
	private SpriteText highScoreText;

	[SerializeField]
	private SimpleSprite currentPlayerFBPic;

	[SerializeField]
	private Renderer currentPlayerFBPicBorder;

	[SerializeField]
	private ParticleSystem arrowParticles;

	[SerializeField]
	private ParticleSystem celebrationParticles;

	private Transform celebrationParticlesTransform;

	private Vector3 arrowStartPos;

	private void Start()
	{
		if (FacebookHandler.IsSessionValid())
		{
			StartCoroutine(SetCurrentPlayerTexture());
		}
		else
		{
			currentPlayerFBPic.renderer.enabled = false;
			currentPlayerFBPicBorder.enabled = false;
		}
		celebrationParticlesTransform = celebrationParticles.transform;
		arrowStartPos = highScoreArrowTransform.localPosition;
	}

	public void SetProgressBars(GameInfo gameInfo, DrillSettings drillSettings, float score)
	{
		Reset();
		int scoreForMedal = drillSettings.GetScoreForMedal(MEDAL.BRONZE);
		int scoreForMedal2 = drillSettings.GetScoreForMedal(MEDAL.SILVER);
		int scoreForMedal3 = drillSettings.GetScoreForMedal(MEDAL.GOLD);
		int[] array = new int[3] { scoreForMedal, scoreForMedal2, scoreForMedal3 };
		float num = gameInfo.HighScore;
		if (score != 0f)
		{
			arrowParticles.Play();
		}
		if (score < (float)scoreForMedal)
		{
			FillProgressBars(MEDAL.BRONZE, score, array);
			if ((bool)scoreArrowTransform)
			{
				PositionArrows(scoreArrowTransform, 1, score, scoreForMedal);
			}
			if ((bool)highScoreArrowTransform)
			{
				PositionArrows(highScoreArrowTransform, 1, num, scoreForMedal);
			}
		}
		else if (score > (float)scoreForMedal && score <= (float)scoreForMedal2)
		{
			FillProgressBars(MEDAL.SILVER, score, array);
			if ((bool)scoreArrowTransform)
			{
				PositionArrows(scoreArrowTransform, 2, score, scoreForMedal2);
			}
			if ((bool)highScoreArrowTransform)
			{
				PositionArrows(highScoreArrowTransform, 2, num, scoreForMedal2);
			}
		}
		else if (score > (float)scoreForMedal2 && score <= (float)scoreForMedal3)
		{
			FillProgressBars(MEDAL.GOLD, score, array);
			if ((bool)scoreArrowTransform)
			{
				PositionArrows(scoreArrowTransform, 3, score, scoreForMedal3);
			}
			if ((bool)highScoreArrowTransform)
			{
				PositionArrows(highScoreArrowTransform, 3, num, scoreForMedal3);
			}
		}
		else
		{
			int num2 = progressBars.Length;
			FillProgressBars(MEDAL.GOLD, score, array);
			GameData.AnimateBarToValue(progressBars[num2 - 1], score / (float)(scoreForMedal3 * 2), (float)(num2 - 1) * 1f + 0.6f);
			if ((bool)scoreArrowTransform)
			{
				PositionArrows(scoreArrowTransform, num2, score, scoreForMedal3 * 2);
			}
			if ((bool)highScoreArrowTransform)
			{
				PositionArrows(highScoreArrowTransform, num2, num, scoreForMedal3 * 2);
			}
		}
		int i = 0;
		for (int num3 = thresholdTexts.Length; i < num3; i++)
		{
			thresholdTexts[i].Text = GameData.FormatScore(array[i]);
		}
		if ((bool)highScoreText)
		{
			if (score != num)
			{
				highScoreText.Text = "Highscore\n" + GameData.FormatScore((int)num);
			}
			else
			{
				highScoreText.renderer.enabled = false;
			}
		}
	}

	private void FillProgressBars(MEDAL medal, float highScore, int[] medalThresholds)
	{
		int burstIndex = 0;
		float num = 1f;
		int i = 0;
		for (int num2 = progressBars.Length; i < num2; i++)
		{
			if (i < (int)medal)
			{
				GameData.AnimateBarToValue(progressBars[i], highScore / (float)medalThresholds[i], num * (float)i);
				if (highScore / (float)medalThresholds[i] >= 1f)
				{
					iTween.PunchScale(trophies[i].gameObject, iTween.Hash("amount", new Vector3(1.5f, 1.5f, 1.5f), "time", num, "delay", num * (float)i + num * 0.6f, "onstart", (Action<object>)delegate
					{
						celebrationParticlesTransform.localPosition = trophies[burstIndex].transform.localPosition;
						burstIndex++;
						celebrationParticles.Play();
					}));
				}
			}
			else
			{
				progressBars[i].Value = 0f;
			}
		}
	}

	public void Reset()
	{
		if ((bool)highScoreArrowTransform)
		{
			highScoreArrowTransform.localPosition = arrowStartPos;
		}
		if ((bool)scoreArrowTransform)
		{
			scoreArrowTransform.localPosition = arrowStartPos;
		}
		iTween.Stop(base.transform.gameObject, includechildren: true);
		UIProgressBar[] array = progressBars;
		foreach (UIProgressBar uIProgressBar in array)
		{
			uIProgressBar.Value = 0f;
		}
	}

	public void StopParticles()
	{
		celebrationParticles.Stop();
		arrowParticles.Stop();
		celebrationParticles.Clear();
		arrowParticles.Clear();
	}

	private void PositionArrows(Transform arrowTrans, int medalRef, float score, int medalScore)
	{
		float num = score / (float)medalScore;
		float x = progressBars[medalRef - 1].transform.localPosition.x - progressBars[medalRef - 1].UnclippedBottomRight.x + progressBars[medalRef - 1].width * num;
		Vector3 localPosition = arrowTrans.localPosition;
		Vector3 vector = (((double)num > 0.5) ? new Vector3(x, localPosition.y, localPosition.z) : ((!((double)num < 0.5)) ? new Vector3(x, localPosition.y, localPosition.z) : new Vector3(x, localPosition.y, localPosition.z)));
		arrowTrans.localPosition = arrowStartPos;
		iTween.MoveTo(arrowTrans.gameObject, iTween.Hash("position", vector, "time", 1.525f, "islocal", true, "easetype", iTween.EaseType.linear));
	}

	private IEnumerator SetCurrentPlayerTexture()
	{
		yield return StartCoroutine(MonoSingleton<LeaderBoardManager>.Instance.FetchCurrentPlayerFBPic());
		currentPlayerFBPic.SetTexture((Texture2D)GameData.m_MyPicture);
	}

	private void HandleSessionOpenedEvent()
	{
		currentPlayerFBPic.renderer.enabled = true;
		currentPlayerFBPicBorder.enabled = true;
		StartCoroutine(SetCurrentPlayerTexture());
	}

	private void HandleFacebookLoggedOut()
	{
		currentPlayerFBPicBorder.enabled = false;
		currentPlayerFBPic.renderer.enabled = false;
	}

	private void OnEnable()
	{
		FacebookManager.sessionOpenedEvent += HandleSessionOpenedEvent;
		FacebookHandler.FacebookLoggedOut += HandleFacebookLoggedOut;
	}

	private void OnDisable()
	{
		FacebookManager.sessionOpenedEvent -= HandleSessionOpenedEvent;
		FacebookHandler.FacebookLoggedOut -= HandleFacebookLoggedOut;
	}
}
