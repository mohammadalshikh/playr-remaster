using System;
using UnityEngine;

[Serializable]
public class UpgradePath
{
	[SerializeField]
	private int levelToUpgradeAt;

	[SerializeField]
	private MeshRenderer[] upgradeMeshs;

	public int LevelToUpgradeAt => levelToUpgradeAt;

	public MeshRenderer[] UpgradeMeshs => upgradeMeshs;
}
