using UnityEngine;

public class SwitchController : MonoBehaviour
{
	public bool m_bOn;

	public SpriteText On;

	public SpriteText Off;

	private UISlider m_Switch;

	private void UpdateDrag(IUIObject obj)
	{
	}

	private void Awake()
	{
		m_Switch = base.transform.GetComponent<UISlider>();
		m_Switch.SetValueChangedDelegate(UpdateDrag);
	}

	private void ONOFF()
	{
		if (m_bOn)
		{
			On.Hide(tf: true);
			Off.Hide(tf: false);
		}
		else
		{
			On.Hide(tf: false);
			Off.Hide(tf: true);
		}
	}

	private void Start()
	{
		ONOFF();
	}
}
