using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Switch : MonoBehaviour
{
	public enum TextureSwitchOptions
	{
		Index,
		Range,
		All
	}

	public delegate void SwitchTexture();

	public Texture2D[] SwitchTexturesFar;

	public Texture2D[] SwitchTexturesNear;

	[HideInInspector]
	public Brightness.AdjustLightmap options;

	[HideInInspector]
	public Brightness.ExecuteONLite executeTrigger;

	[HideInInspector]
	public TextureSwitchOptions switchOptions;

	[HideInInspector]
	public int startIndexFar;

	[HideInInspector]
	public int endIndexFar;

	[HideInInspector]
	public int startIndexNear;

	[HideInInspector]
	public int endIndexNear;

	[HideInInspector]
	public int timestamp;

	private Texture2D originalFar;

	private Texture2D originalNear;

	[method: MethodImpl(32)]
	public event SwitchTexture SwitchNow;

	private void Start()
	{
		if (executeTrigger == Brightness.ExecuteONLite.Start)
		{
			AdjustSwitching(startIndexFar, startIndexNear);
		}
		else if (executeTrigger == Brightness.ExecuteONLite.Trigger)
		{
			this.SwitchNow = (SwitchTexture)Delegate.Combine(this.SwitchNow, new SwitchTexture(Switch_SwitchNow));
		}
	}

	private void Awake()
	{
		if (executeTrigger == Brightness.ExecuteONLite.Awake)
		{
			AdjustSwitching(startIndexFar, startIndexNear);
		}
	}

	private void Switch_SwitchNow()
	{
		AdjustSwitching(startIndexFar, startIndexNear);
	}

	public void SwitchTextures()
	{
		if (this.SwitchNow != null)
		{
			this.SwitchNow();
		}
	}

	public void AdjustSwitching(int indexfar, int indexnear)
	{
		if (base.gameObject.isStatic && base.GetComponent<Renderer>() != null)
		{
			if (options == Brightness.AdjustLightmap.Both)
			{
				SwitchLightmaps(indexfar, indexnear);
			}
			else if (options == Brightness.AdjustLightmap.Far)
			{
				SwitchLightmaps(indexfar, -1);
			}
			else if (options == Brightness.AdjustLightmap.Near)
			{
				SwitchLightmaps(-1, indexnear);
			}
		}
		else
		{
			Debug.Log("Lightmap Manager requires a static gameObject with a renderer to do this job.");
		}
	}

	public static void SwitchLightmaps(GameObject obj, Texture2D far, Texture2D near)
	{
		if (obj.GetComponent<Renderer>() != null && obj.isStatic)
		{
			SwitchLM.switchmaps(obj.GetComponent<Renderer>().lightmapIndex, 0, 0, new Texture2D[] { far }, new Texture2D[] { near });
		}
		else
		{
			Debug.Log("Lightmap Manager requires a static gameObject with a renderer to do this job.");
		}
	}

	public void SwitchLightmaps(int indexfar, int indexnear)
	{
		SwitchLM.switchmaps(indexfar, indexnear, base.GetComponent<Renderer>().lightmapIndex, SwitchTexturesFar, SwitchTexturesNear);
	}
}
