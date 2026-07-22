using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Tint : MonoBehaviour
{
	public enum AdjustTint
	{
		Far,
		Near,
		Both,
		off
	}

	public enum ExecuteON
	{
		Start,
		Awake,
		Trigger
	}

	public enum ExecuteONLite
	{
		Start,
		Awake,
		Trigger
	}

	public delegate void ObjectTint();

	public Color TintColor;

	public AdjustTint options;

	public ExecuteON executeTrigger;

	[method: MethodImpl(32)]
	public event ObjectTint TintUp;

	public void TintLightmap()
	{
		if (this.TintUp != null)
		{
			this.TintUp();
		}
	}

	private void Start()
	{
		if (executeTrigger == ExecuteON.Start)
		{
			AdjustLightmapTint(TintColor, options);
		}
		if (executeTrigger == ExecuteON.Trigger)
		{
			this.TintUp = (ObjectTint)Delegate.Combine(this.TintUp, new ObjectTint(Lightmap_Tint));
		}
	}

	private void Lightmap_Tint()
	{
		AdjustLightmapTint(TintColor, options);
	}

	private void Awake()
	{
		if (executeTrigger == ExecuteON.Awake)
		{
			AdjustLightmapTint(TintColor, options);
		}
	}

	public void AdjustLightmapTint(Color val)
	{
		if (base.gameObject.isStatic && base.GetComponent<Renderer>() != null)
		{
			SetTint(LightmapSettings.lightmaps[base.GetComponent<Renderer>().lightmapIndex].lightmapColor, val);
			SetTint(LightmapSettings.lightmaps[base.GetComponent<Renderer>().lightmapIndex].lightmapDir, val);
		}
	}

	public void AdjustLightmapTint(Color val, AdjustTint options)
	{
		if (base.gameObject.isStatic && base.GetComponent<Renderer>() != null)
		{
			switch (options)
			{
			case AdjustTint.Both:
				SetTint(LightmapSettings.lightmaps[base.GetComponent<Renderer>().lightmapIndex].lightmapColor, val);
				SetTint(LightmapSettings.lightmaps[base.GetComponent<Renderer>().lightmapIndex].lightmapDir, val);
				break;
			case AdjustTint.Far:
				SetTint(LightmapSettings.lightmaps[base.GetComponent<Renderer>().lightmapIndex].lightmapColor, val);
				break;
			case AdjustTint.Near:
				SetTint(LightmapSettings.lightmaps[base.GetComponent<Renderer>().lightmapIndex].lightmapDir, val);
				break;
			}
		}
	}

	private void SetTint(Texture2D tex, Color val)
	{
		if (tex != null)
		{
			Color[] pixels = tex.GetPixels();
			Color[] array = new Color[pixels.Length];
			for (int i = 0; i < pixels.Length; i++)
			{
				Color color = pixels[i] * val;
				ref Color reference = ref array[i];
				reference = new Color(color.r, color.g, color.b, color.a);
			}
			tex.SetPixels(array);
			tex.Apply();
		}
	}
}
