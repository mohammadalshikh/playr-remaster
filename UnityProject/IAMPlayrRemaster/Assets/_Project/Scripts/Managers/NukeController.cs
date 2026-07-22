using System.Runtime.CompilerServices;
using UnityEngine;

public class NukeController : MonoBehaviour
{
	public delegate void HandleOnExplosion();

	[method: MethodImpl(32)]
	public static event HandleOnExplosion OnExplosionEvent;

	public void FireUp()
	{
		if (NukeController.OnExplosionEvent != null)
		{
			NukeController.OnExplosionEvent();
		}
	}
}
