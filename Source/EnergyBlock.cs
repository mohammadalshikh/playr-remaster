using UnityEngine;

public class EnergyBlock : MonoBehaviour
{
	private void Hide()
	{
		base.gameObject.renderer.enabled = false;
	}

	private void Show()
	{
		base.gameObject.renderer.enabled = true;
	}

	public void SetProgress(float current, float max)
	{
		float num = current / max;
		PackedSprite component = GetComponent<PackedSprite>();
		component.TruncateTop(num);
		component.Color = new Color(1f, 1f * num, 0f, 1f);
	}
}
