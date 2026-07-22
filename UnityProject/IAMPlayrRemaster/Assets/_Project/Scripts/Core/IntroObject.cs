using UnityEngine;

public class IntroObject : MonoBehaviour
{
	public PackedSprite mainTexture;

	private void Awake()
	{
		if (mainTexture == null && base.transform.GetComponent<PackedSprite>() != null)
		{
			mainTexture = base.transform.GetComponent<PackedSprite>();
		}
	}

	public void HideTexture()
	{
		mainTexture.Hide(tf: true);
	}
}
