using UnityEngine;

public class CircularTriggerOnHit : TriggerEventOnHit
{
	[SerializeField]
	protected float _radius;

	[SerializeField]
	private Color _gizmoColor = Color.white;

	protected float _accuracy;

	protected Vector3 _hitPosition;

	public float Accuracy => Mathf.Clamp(_accuracy + 0.02f, 0.1f, 1f);

	public Vector3 HitPosition => _hitPosition;

	protected virtual void EnteredCircle(Collider other)
	{
		_hitPosition = other.transform.position;
	}

	protected override void OnActionEnter(Collider other)
	{
		base.OnActionEnter(other);
		float num = _radius + other.bounds.extents.x;
		if ((other.transform.position - base.transform.position).sqrMagnitude <= num * num)
		{
			_accuracy = Mathf.Clamp01((_radius * _radius - (other.transform.position - base.transform.position).sqrMagnitude) / (_radius * _radius));
			EnteredCircle(other);
		}
	}

	public override void Reset()
	{
		base.Reset();
		_accuracy = 0f;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = _gizmoColor;
		Gizmos.DrawWireSphere(base.transform.position, _radius);
	}
}
