using UnityEngine;

public class LandingIndicator : MonoBehaviour
{
	public INDICATOR_STATE currentState;

	public float activePosY = 1f;

	public Transform[] renderers;

	private void Awake()
	{
		EnableRenderers(currentState == INDICATOR_STATE.ACTIVE);
	}

	private void Reset()
	{
		EnableRenderers(bEnabled: false);
	}

	private void EnableRenderers(bool bEnabled)
	{
		Transform[] array = renderers;
		foreach (Transform transform in array)
		{
			if (transform.renderer != null)
			{
				transform.renderer.enabled = bEnabled;
			}
		}
	}

	private void Update()
	{
		if (currentState != 0)
		{
		}
	}

	public void Activate()
	{
		SoccerBall ball = GameData.Ball;
		base.transform.position = new Vector3(ball.transform.position.x, activePosY, ball.transform.position.z);
		EnableRenderers(bEnabled: true);
		currentState = INDICATOR_STATE.ACTIVE;
	}

	public void DeActivate()
	{
		EnableRenderers(bEnabled: false);
		currentState = INDICATOR_STATE.INACTIVE;
	}
}
