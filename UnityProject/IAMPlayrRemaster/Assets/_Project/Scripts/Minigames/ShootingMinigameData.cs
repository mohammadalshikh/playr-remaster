using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShootingMinigameData
{
	[SerializeField]
	private GameObject _goalCelebration;

	[SerializeField]
	private GameObject _superCelebration;

	[SerializeField]
	private GameObject _flameCelebration;

	[SerializeField]
	private List<ShootingTarget> _targetPrefabs = new List<ShootingTarget>();

	[SerializeField]
	private SFX _hitSound;

	[SerializeField]
	private Shooting3DText _text3DPrefab;

	private ParticleSystem _superSystem;

	private ParticleSystem _flameSystem;

	public Shooting3DText Text3DPrefab => _text3DPrefab;

	public SFX HitSound => _hitSound;

	public void CelebrateGoalAt(Vector3 celebratePos)
	{
		UnityEngine.Object.Instantiate(_goalCelebration, celebratePos, Quaternion.identity);
	}

	public void SuperCelebrationAt(Vector3 celebratePos)
	{
		if (_superSystem == null)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(_superCelebration, celebratePos, Quaternion.identity);
			_superSystem = gameObject.GetComponent<ParticleSystem>();
		}
		if (!_superSystem.isPlaying)
		{
			_superSystem.transform.position = celebratePos;
			_superSystem.Play();
		}
	}

	public void FlameCelebrationAt(Vector3 celebratePos)
	{
		if (_flameSystem == null)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(_flameCelebration, celebratePos, Quaternion.identity);
			_flameSystem = gameObject.GetComponent<ParticleSystem>();
		}
		if (!_flameSystem.isPlaying)
		{
			_flameSystem.transform.position = celebratePos;
			_flameSystem.Play();
		}
	}

	public GameObject InstantiateTarget(ShootingTarget.ID id)
	{
		ShootingTarget shootingTarget = _targetPrefabs.Find((ShootingTarget target) => target.Identifier == id);
		if (shootingTarget != null)
		{
			return (GameObject)UnityEngine.Object.Instantiate(shootingTarget.gameObject);
		}
		return null;
	}
}
