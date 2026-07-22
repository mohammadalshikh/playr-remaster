using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class View : MonoBehaviour
{
	public delegate void HandlePanelTransitioned(bool isPanelDisplaying);

	[SerializeField]
	private bool hideAtStart;

	[SerializeField]
	private UIPanel panel;

	[SerializeField]
	private UIButton closeButton;

	private bool isDisplaying;

	protected Renderer[] renderers;

	private UIButton[] buttons;

	public UIPanel Panel => panel;

	public bool IsDisplaying => isDisplaying;

	[method: MethodImpl(32)]
	public static event HandlePanelTransitioned PanelTransitioned;

	private void Awake()
	{
		if (!panel)
		{
			renderers = GetComponentsInChildren<Renderer>();
			buttons = GetComponentsInChildren<UIButton>();
		}
		if ((bool)closeButton)
		{
			closeButton.AddInputDelegate(HandleCloseButtonPressed);
		}
		if (hideAtStart)
		{
			DisplayView(isVisible: false);
		}
	}

	public virtual void DisplayView(bool isVisible)
	{
		isDisplaying = isVisible;
		if ((bool)panel)
		{
			if (isVisible)
			{
				panel.BringIn();
			}
			else
			{
				panel.Dismiss();
			}
			if (View.PanelTransitioned != null)
			{
				View.PanelTransitioned(MonoSingleton<PanelManager>.Instance.IsPanelDisplaying);
			}
		}
		else
		{
			ToggleViewElements(isVisible);
		}
	}

	private void ToggleViewElements(bool isVisible)
	{
		if ((bool)panel)
		{
			return;
		}
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = isVisible;
		}
		UIButton[] array2 = buttons;
		foreach (UIButton uIButton in array2)
		{
			uIButton.controlIsEnabled = isVisible;
			uIButton.detargetOnDisable = isVisible;
			if ((bool)uIButton.GetComponent<Collider>())
			{
				uIButton.GetComponent<Collider>().enabled = isVisible;
			}
		}
	}

	public virtual void HandleCloseButtonPressed(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			DisplayView(isVisible: false);
		}
	}
}
