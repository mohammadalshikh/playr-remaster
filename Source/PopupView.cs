public class PopupView
{
	private string _title = "Name";

	private string _message = "Name";

	public string Title => _title;

	public string Message => _message;

	public PopupView(string title, string message)
	{
		_title = title;
		_message = message;
	}
}
