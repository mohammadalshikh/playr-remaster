public class InGameMessages : PopupViewController
{
	public static InGameMessages Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}
}
