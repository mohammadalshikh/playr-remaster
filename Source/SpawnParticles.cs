using UnityEngine;

public class SpawnParticles : HitEvent
{
	public GameObject particlesPrefab;

	[SerializeField]
	private bool _autoDestroy;

	[SerializeField]
	private float _destroyDelay;

	public Vector3 m_spawnOffset;

	private GameObject _currentParticles;

	public GameObject CurrentParticles => _currentParticles;

	protected override void OnActionEnter(Collider other)
	{
		if (particlesPrefab == null)
		{
			Debug.LogError("No particles prefab has been assigned to " + base.name);
		}
		else
		{
			Spawn();
		}
	}

	public void EmitParticles()
	{
		Spawn();
	}

	private void Spawn()
	{
		_currentParticles = (GameObject)Object.Instantiate(particlesPrefab, base.transform.position + m_spawnOffset, Quaternion.identity);
		_num_hits++;
		if (_currentParticles != null && _autoDestroy)
		{
			Object.Destroy(_currentParticles.gameObject, _destroyDelay);
		}
	}
}
