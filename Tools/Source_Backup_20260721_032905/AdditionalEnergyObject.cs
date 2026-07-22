using UnityEngine;

public class AdditionalEnergyObject : MonoBehaviour
{
	public UIPanel m_PackedSpritePanel;

	public UIPanel m_SpriteTextPanel;

	public SpriteText m_AdditionalEnergyText;

	public static AdditionalEnergyObject Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void UpdateEnergy()
	{
	}
}
