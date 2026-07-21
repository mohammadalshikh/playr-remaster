using System.Collections.Generic;
using UnityEngine;

public class Tutorials : MonoBehaviour
{
	private int m_nCurrentItem;

	public UIButton m_NextButton;

	public UIButton m_PreviousButton;

	public List<UIScrollList> m_tutorials;

	private UIScrollList m_currentList;

	public static Tutorials Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		SetupType(0);
		iTween.FadeTo(m_NextButton.gameObject, iTween.Hash("alpha", 1f, "time", 0.5f, "amount", 0.5f, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutBounce, "ignoretimescale", true));
		iTween.FadeTo(m_PreviousButton.gameObject, iTween.Hash("alpha", 1f, "time", 0.5f, "amount", 0.5f, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutBounce, "ignoretimescale", true));
		PreviousButtonColor();
	}

	private void PreviousButtonColor()
	{
		if (m_nCurrentItem == 0)
		{
			m_PreviousButton.Color = new Color(1f, 1f, 1f, 0f);
		}
		else
		{
			m_PreviousButton.Color = new Color(1f, 1f, 1f, 1f);
		}
	}

	private void PreviousItem()
	{
		if (m_nCurrentItem != 0)
		{
			m_nCurrentItem--;
			BringCurrentItem();
		}
	}

	private void NextItem()
	{
		m_nCurrentItem++;
		if (m_nCurrentItem < m_currentList.Count)
		{
			BringCurrentItem();
		}
	}

	private void BringCurrentItem()
	{
		PreviousButtonColor();
		m_currentList.ScrollToItem(m_nCurrentItem, 0.5f);
	}

	public void ResetPosition()
	{
		m_nCurrentItem = 0;
		BringCurrentItem();
		PreviousButtonColor();
	}

	public void SetupType(int index)
	{
		index = Mathf.Clamp(index, 0, m_tutorials.Count);
		if (m_currentList != null)
		{
			m_currentList.transform.GetComponent<UIPanel>().Dismiss();
		}
		m_currentList = m_tutorials[index];
		if (m_currentList.transform.GetComponent<UIPanel>() != null)
		{
			m_currentList.transform.GetComponent<UIPanel>().BringIn();
		}
		ResetPosition();
	}

	public void BringIn()
	{
		base.transform.GetComponent<UIPanel>().BringIn();
	}

	private void Update()
	{
		if ((bool)m_currentList && m_currentList.SnappedItem != null && m_currentList.SnappedItem.Index != m_nCurrentItem)
		{
			m_nCurrentItem = m_currentList.SnappedItem.Index;
			PreviousButtonColor();
		}
	}
}
