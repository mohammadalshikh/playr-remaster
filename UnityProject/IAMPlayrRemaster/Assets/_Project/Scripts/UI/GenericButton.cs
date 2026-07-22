using System.Collections.Generic;
using UnityEngine;

public class GenericButton : UIButton
{
	private const float pressedScaleFactor = 1.1f;

	private const float releasedScaleFactor = 0.91f;

	private PackedSprite[] sprites;

	private SpriteText[] texts;

	private List<Color> origColors = new List<Color>();

	public virtual void Start()
	{
		base.Start();
		sprites = GetComponentsInChildren<PackedSprite>();
		texts = GetComponentsInChildren<SpriteText>();
		PackedSprite[] array = sprites;
		foreach (PackedSprite packedSprite in array)
		{
			origColors.Add(packedSprite.Color);
		}
		this.AddInputDelegate(HandleButtonPressed);
	}

	protected virtual void HandleButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			ButtonPressedEffect();
		}
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE || ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF || ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			ButtonReleasedEffect();
		}
	}

	private void ButtonPressedEffect()
	{
		this.SetSize(this.width * 1.1f, this.height * 1.1f);
		PackedSprite[] array = sprites;
		foreach (PackedSprite packedSprite in array)
		{
			packedSprite.SetSize(packedSprite.width * 1.1f, packedSprite.height * 1.1f);
			packedSprite.SetColor(new Color(255f, 255f, 255f, 255f));
		}
		SpriteText[] array2 = texts;
		foreach (SpriteText spriteText in array2)
		{
			spriteText.SetCharacterSize(spriteText.characterSize * 1.1f);
		}
	}

	private void ButtonReleasedEffect()
	{
		this.SetSize(this.width * 0.91f, this.height * 0.91f);
		int i = 0;
		for (int num = sprites.Length; i < num; i++)
		{
			sprites[i].SetSize(sprites[i].width * 0.91f, sprites[i].height * 0.91f);
			sprites[i].SetColor(origColors[i]);
		}
		SpriteText[] array = texts;
		foreach (SpriteText spriteText in array)
		{
			spriteText.SetCharacterSize(spriteText.characterSize * 0.91f);
		}
	}

	public void DisplayButton(bool isVisible)
	{
		base.transform.GetComponent<Renderer>().enabled = isVisible;
		controlIsEnabled = isVisible;
		base.GetComponent<Collider>().enabled = isVisible;
		if (sprites != null)
		{
			PackedSprite[] array = sprites;
			foreach (PackedSprite packedSprite in array)
			{
				packedSprite.GetComponent<Renderer>().enabled = isVisible;
			}
		}
		if (texts != null)
		{
			SpriteText[] array2 = texts;
			foreach (SpriteText spriteText in array2)
			{
				spriteText.GetComponent<Renderer>().enabled = isVisible;
			}
		}
	}
}
