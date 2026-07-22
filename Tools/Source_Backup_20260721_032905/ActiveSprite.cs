public class ActiveSprite : PackedSprite
{
	private bool _isActive;

	public bool IsActive => _isActive;

	public void Activate(string animationName)
	{
		if (!_isActive)
		{
			PlayAnim(animationName);
			_isActive = true;
		}
	}

	public void Deactivate(string animationName)
	{
		if (_isActive)
		{
			PlayAnim(animationName);
			_isActive = false;
		}
	}
}
