using UnityEngine;

[RequireComponent(typeof(MovePosition))]
public class MinigameTriggerPoints : MonoBehaviour
{
	private MovePosition positionController;

	private SpriteText textLabel;

	private void Awake()
	{
		positionController = base.transform.GetComponent<MovePosition>();
		textLabel = base.transform.GetComponent<SpriteText>();
	}

	public void StartAnimation()
	{
		positionController.StartMoving();
	}

	public void SetText(int points)
	{
		if (textLabel != null)
		{
			textLabel.Text = string.Empty + points;
		}
	}
}
