using System;
using UnityEngine;

public class TickerTapeText : UIScrollList
{
	private SpriteText textObj;

	public SpriteText TextObj => textObj;

	protected override void Start()
	{
		base.Start();
		textObj = items[0].gameObject.GetComponent<SpriteText>();
		textObj.renderer.enabled = false;
	}

	public void StartTicker(float speed)
	{
		RefreshPosition();
		textObj.renderer.enabled = true;
		iTween.MoveTo(textObj.gameObject, iTween.Hash("x", (viewableArea.x - textObj.TotalWidth) * 1.2f, "speed", speed, "onUpdate", (Action<object>)delegate
		{
			CalcClippingRect();
		}, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.linear, "onComplete", (Action<object>)delegate
		{
			RefreshPosition();
		}));
	}

	private void RefreshPosition()
	{
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localPosition2 = textObj.transform.localPosition;
		textObj.transform.localPosition = new Vector3(localPosition.x - textObj.TotalWidth * 1.2f, localPosition2.y, localPosition2.z);
	}
}
