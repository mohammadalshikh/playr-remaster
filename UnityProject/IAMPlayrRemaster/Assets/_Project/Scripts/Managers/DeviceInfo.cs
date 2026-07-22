using UnityEngine;

public static class DeviceInfo
{
	public static bool IsHighEnd => false;

	public static bool IsTablet => Screen.height >= 768;
}
