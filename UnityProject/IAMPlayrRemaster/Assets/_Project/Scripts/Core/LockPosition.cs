using UnityEngine;

public class LockPosition : MonoBehaviour
{
	[SerializeField]
	private bool _x;

	[SerializeField]
	private bool _y;

	[SerializeField]
	private bool _z;

	private Transform _cachedTransform;

	private float _positionX;

	private float _positionY;

	private float _positionZ;

	private void Start()
	{
		_cachedTransform = base.transform;
		_positionX = base.transform.localPosition.x;
		_positionY = base.transform.localPosition.y;
		_positionZ = base.transform.localPosition.z;
	}

	private void Update()
	{
		if (_x)
		{
			_cachedTransform.localPosition = new Vector3(_positionX, _cachedTransform.localPosition.x, _cachedTransform.localPosition.z);
		}
		if (_y)
		{
			_cachedTransform.localPosition = new Vector3(_cachedTransform.localPosition.x, _positionY, _cachedTransform.localPosition.z);
		}
		if (_z)
		{
			_cachedTransform.localPosition = new Vector3(_cachedTransform.localPosition.x, _cachedTransform.localPosition.y, _positionZ);
		}
	}
}
