public class Trophy
{
	private string _title = "Achievement Unlocked";

	private string _type;

	private ITEM_STATE _state;

	private string _description;

	private string _fbDescription;

	private string _URLimageName = string.Empty;

	private int _trophyIndex;

	public string Title
	{
		get
		{
			return _title;
		}
		set
		{
			_title = value;
		}
	}

	public string URLImageName
	{
		get
		{
			return _URLimageName;
		}
		set
		{
			_URLimageName = value;
		}
	}

	public ITEM_STATE State => _state;

	public string Type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
		}
	}

	public string Description
	{
		get
		{
			return _description;
		}
		set
		{
			_description = value;
		}
	}

	public string FBDescription
	{
		get
		{
			return _fbDescription;
		}
		set
		{
			_fbDescription = value;
		}
	}

	public int TrophyIndex
	{
		get
		{
			return _trophyIndex;
		}
		set
		{
			_trophyIndex = value;
		}
	}

	public void UnLock()
	{
		_state = ITEM_STATE.UNLOCKED;
	}
}
