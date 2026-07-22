using UnityEngine;

public class SantaController : MonoBehaviour
{
	public Texture2D onSantaTexture;

	public Texture2D offSantaTexture;

	public Renderer santaRenderer;

	public SFX celebrationSound;

	private bool m_endGame;

	public static SantaController Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void TurnOn()
	{
		ChangeMainTexture(onSantaTexture);
		if (!m_endGame)
		{
			AudioManager.PlaySFX(celebrationSound, 0f);
			m_endGame = true;
		}
	}

	public void TurnOff()
	{
		ChangeMainTexture(offSantaTexture);
	}

	private void ChangeMainTexture(Texture2D texture)
	{
		santaRenderer.material.mainTexture = texture;
	}

	private void Start()
	{
		Invoke("TurnOff", 4f);
	}
}
