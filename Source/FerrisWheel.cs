using UnityEngine;

public class FerrisWheel : MonoBehaviour
{
	[SerializeField]
	private UIButton3D ferrisWheelButton;

	[SerializeField]
	private Animation ferrisWheelAnim;

	private void Start()
	{
		if ((bool)ferrisWheelButton)
		{
			ferrisWheelButton.AddInputDelegate(HandleFerrisWheelButtonPressed);
		}
	}

	private void HandleFerrisWheelButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (ferrisWheelAnim.isPlaying)
			{
				ferrisWheelAnim.Stop();
			}
			else
			{
				ferrisWheelAnim.Play();
			}
		}
	}
}
