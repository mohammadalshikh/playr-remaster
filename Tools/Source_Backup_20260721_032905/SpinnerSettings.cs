public class SpinnerSettings
{
	public SpriteText sprite;

	public int currentVal;

	public int targetVal;

	public float speedVal;

	public void Setup(SpriteText _sprite, int _curVal, int _targetVal, float _speed = 3f)
	{
		sprite = _sprite;
		currentVal = _curVal;
		targetVal = _targetVal;
		speedVal = _speed;
	}
}
