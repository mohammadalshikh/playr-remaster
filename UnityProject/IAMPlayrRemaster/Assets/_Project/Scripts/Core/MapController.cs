using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MapController : MonoSingleton<MapController>
{
	[SerializeField]
	private CameraScrollController cameraScrollController;

	[SerializeField]
	private Camera mapCamera;

	[SerializeField]
	private Camera navBarCamera;

	[SerializeField]
	private List<MapLocation> mapLocations;

	public static GameData.GAME_LOCATION gameLocation;

	public float CamAnimationTime => cameraScrollController.CameraAnim.clip.length;

	[method: MethodImpl(32)]
	public static event Action CamOrthgraphicSizeChanged;

	private void Awake()
	{
		MonoSingleton<MapController>.Instance = this;
	}

	private void Start()
	{
		if (DeviceInfo.IsTablet)
		{
			mapCamera.fieldOfView = 50f;
			if (!GameData.Instance.HasIntroSeqOccured)
			{
				navBarCamera.orthographicSize = 108f;
			}
			else
			{
				navBarCamera.orthographicSize = 104f;
			}
			MapController.CamOrthgraphicSizeChanged();
		}
		if (GameData.Instance.MapUpgrades.Count != 0)
		{
			return;
		}
		foreach (MapLocation mapLocation in mapLocations)
		{
			UpgradePath[] upgradePaths = mapLocation.UpgradePaths;
			foreach (UpgradePath upgradePath in upgradePaths)
			{
				if (mapLocation.GameLocation != GameData.GAME_LOCATION.ShootingDrills)
				{
					GameData.Instance.MapUpgrades.Add(upgradePath.LevelToUpgradeAt, mapLocation.GameLocation);
				}
			}
		}
	}
}
