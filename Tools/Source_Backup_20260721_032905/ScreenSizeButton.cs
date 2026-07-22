using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ScreenSizeButton : MonoBehaviour
{
	[SerializeField]
	private UIPanel m_mainPanel;

	private bool m_dismissOnTap = true;

	public static ScreenSizeButton Instance;

	[method: MethodImpl(32)]
	public event Action OnTap;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void Reset()
	{
		this.OnTap = null;
		m_dismissOnTap = true;
	}

	public void SetupAndBring(Action callBack, bool dismissOnTap = true)
	{
		SetCallBack(callBack);
		m_dismissOnTap = dismissOnTap;
		BringIn();
	}

	public void SetCallBack(Action callBack)
	{
		this.OnTap = (Action)Delegate.Combine(this.OnTap, callBack);
	}

	public void RemoveCallBack(Action callBack)
	{
		this.OnTap = (Action)Delegate.Remove(this.OnTap, callBack);
	}

	public void OnTapEvent()
	{
		if (this.OnTap != null)
		{
			this.OnTap();
		}
		if (m_dismissOnTap)
		{
			Dismiss();
		}
	}

	public void BringIn()
	{
		m_mainPanel.BringIn();
	}

	public void Dismiss()
	{
		m_mainPanel.Dismiss();
		Reset();
	}
}
