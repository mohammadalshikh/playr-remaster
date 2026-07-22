public class FacebookPlayer
{
	private string _playerId = string.Empty;

	private string _name = "Carlos R";

	private int _score;

	private int _position;

	public string PlayerId => _playerId;

	public string Name => _name;

	public int Score
	{
		get
		{
			return _score;
		}
		set
		{
			_score = value;
		}
	}

	public int Position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = value;
		}
	}

	public FacebookPlayer(string playerId, string name, int score = 0, int position = 1)
	{
		_playerId = playerId;
		_name = name;
		_score = score;
		_position = position;
	}
}
