using UnityEngine;

public class GameTip : MonoBehaviour
{
	public MINIGAME_TYPE type;

	public UIPanel m_tipPanel;

	public PackedSprite m_background;

	public SpriteText m_tipDescription;

	public SpriteText m_tipTitle;

	public bool genericType;

	private void Start()
	{
		if (m_tipPanel == null)
		{
			m_tipPanel = base.transform.GetComponentInChildren<UIPanel>();
		}
	}

	public void SetDescription(string description)
	{
		if (m_tipDescription != null)
		{
			m_tipDescription.Text = description;
		}
		else
		{
			Debug.LogWarning("No tip description SpriteText has been added to " + base.gameObject.name);
		}
	}

	public void SetTitle(string title)
	{
		if (m_tipTitle != null)
		{
			m_tipTitle.Text = title;
		}
		else
		{
			Debug.LogWarning("No tip title SpriteText has been added to " + base.gameObject.name);
		}
	}

	public void SetBackgroundFrame(string animName)
	{
		if (m_background != null)
		{
			m_background.PlayAnim(animName);
		}
		else
		{
			Debug.LogWarning("No tip background PackedSprite has been added to " + base.gameObject.name);
		}
	}

	public void Show()
	{
		m_tipPanel.BringIn();
	}

	public void Dismiss()
	{
		m_tipPanel.Dismiss();
	}
}
