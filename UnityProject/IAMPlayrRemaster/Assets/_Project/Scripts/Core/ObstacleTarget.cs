using System.Runtime.CompilerServices;
using UnityEngine;

public class ObstacleTarget : MinigameTarget
{
	public enum TYPE
	{
		GO_THROUGH,
		BOUNCE_OFF
	}

	public enum ID
	{
		NONE,
		WALL_2,
		WALL_3,
		WALL_5
	}

	public delegate void HandleObstacleHit(ObstacleTarget obstacle);

	[SerializeField]
	private ID _id;

	[SerializeField]
	private TYPE _type;

	[SerializeField]
	private bool _lookCamera;

	[SerializeField]
	private GameObject _popupMessage;

	private GameObject _currentMessage;

	private GridUnit _gridUnit = new GridUnit();

	public ID Identifier => _id;

	public TYPE Type => _type;

	public GridUnit Grid => _gridUnit;

	[method: MethodImpl(32)]
	public static event HandleObstacleHit ObstacleHit;

	private void OnEnable()
	{
		ObstacleManager.ObstacleUpdate += UpdateTarget;
	}

	private void OnDisable()
	{
		ObstacleManager.ObstacleUpdate -= UpdateTarget;
	}

	public override void OnActionCollision(Collider collider)
	{
		Action(TYPE.BOUNCE_OFF);
	}

	public override void OnActionTrigger(Collider collider)
	{
		Action(TYPE.GO_THROUGH);
	}

	private void Action(TYPE type)
	{
		if (_type == type && ObstacleTarget.ObstacleHit != null)
		{
			ObstacleTarget.ObstacleHit(this);
		}
	}

	private void FaceCamera()
	{
		base.transform.LookAt(GameData.Ball.transform.position);
		base.transform.rotation *= Quaternion.AngleAxis(180f, Vector3.up);
	}

	public virtual void UpdateTarget()
	{
		if (_lookCamera)
		{
			FaceCamera();
		}
	}

	public virtual void SetGridPosition(GridUnit gridUnit)
	{
		_gridUnit = gridUnit;
		base.transform.position = new Vector3(_gridUnit.WorldPosition.x, base.transform.position.y, 0f - _gridUnit.WorldPosition.z);
	}

	private void ShowMessage(string message, Color textColor)
	{
		if (_currentMessage == null)
		{
			_currentMessage = (GameObject)Object.Instantiate(_popupMessage, base.transform.localPosition, base.transform.localRotation);
		}
		_currentMessage.transform.parent = base.transform;
		PopupText3D component = _currentMessage.GetComponent<PopupText3D>();
		if (component != null)
		{
			component.Show(message, textColor);
		}
	}
}
