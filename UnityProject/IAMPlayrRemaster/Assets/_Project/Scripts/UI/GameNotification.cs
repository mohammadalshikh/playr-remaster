using UnityEngine;

public class GameNotification : MonoBehaviour
{
	public UIPanel m_mainPanel;

	public SpriteText m_title;

	public SpriteText m_description1;

	public SpriteText m_description2;

	public UIButton m_button;

	public PackedSprite m_background;

	public static GameNotification Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void SetNotification(string title, string description1, string description2, string buttonText, MonoBehaviour button1callbackObject, string button1callbackMethod)
	{
		Reset();
		if (title != null)
		{
			m_title.Text = title;
			m_title.transform.GetComponent<Renderer>().enabled = true;
		}
		if (description1 != null)
		{
			m_description1.Text = description1;
			m_description1.transform.GetComponent<Renderer>().enabled = true;
		}
		if (description2 != null)
		{
			m_description2.Text = description2;
			m_description2.transform.GetComponent<Renderer>().enabled = true;
		}
		if (buttonText != null)
		{
			m_button.Text = buttonText;
			m_button.scriptWithMethodToInvoke = button1callbackObject;
			m_button.methodToInvoke = button1callbackMethod;
			m_button.transform.GetComponent<Renderer>().enabled = true;
		}
	}

	public void Reset()
	{
		m_title.transform.GetComponent<Renderer>().enabled = false;
		m_description1.transform.GetComponent<Renderer>().enabled = false;
		m_description2.transform.GetComponent<Renderer>().enabled = false;
		m_button.transform.GetComponent<Renderer>().enabled = false;
	}

	public void BringIn()
	{
		m_mainPanel.BringIn();
	}

	public void Dismiss()
	{
		Reset();
		m_mainPanel.Dismiss();
	}
}
