using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Brightness : MonoBehaviour
{
	public enum OperateON
	{
		GPU,
		CPU
	}

	public enum AdjustLightmap
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
		Update,
		FixedUpdate,
		Trigger
	}

	public enum ExecuteONLite
	{
		Start,
		Awake,
		Trigger
	}

	public delegate void ObjectBrightness();

	public OperateON operateOn = OperateON.CPU;

	public float level;

	public bool useSharedMaterials;

	private List<Material> mats;

	public bool useCurve = true;

	public AnimationCurve curve = new AnimationCurve();

	public bool loopCurve = true;

	public bool startOnFirstKey = true;

	public float timeScale = 1f;

	private float currentCurve;

	private float currentCurveVal;

	private float last;

	public AdjustLightmap options;

	public ExecuteON executeTrigger;

	public float Min = -1f;

	public float Max = 1f;

	private bool IsExecuting;

	[method: MethodImpl(32)]
	public event ObjectBrightness BrightUp;

	public void Bright()
	{
		if (this.BrightUp != null)
		{
			this.BrightUp();
		}
	}

	private void LoadMaterials()
	{
		mats = new List<Material>();
		Material[] array = null;
		array = ((!useSharedMaterials) ? base.renderer.materials : base.renderer.sharedMaterials);
		Material[] array2 = array;
		foreach (Material material in array2)
		{
			Debug.Log(material.shader.name);
			if (material.shader.name != "LightmappingEffects/Diffuse")
			{
				material.shader = Shader.Find("LightmappingEffects/Diffuse");
			}
			mats.Add(material);
		}
	}

	private void Start()
	{
		if (operateOn == OperateON.GPU)
		{
			LoadMaterials();
		}
		if (startOnFirstKey && useCurve)
		{
			currentCurve = curve.keys[0].time;
		}
		if (executeTrigger == ExecuteON.Start)
		{
			if (!IsExecuting)
			{
				IsExecuting = true;
				StartCoroutine(updateValues());
			}
			if (useCurve)
			{
				AdjustBrightnessCurve(options);
			}
			else
			{
				AdjustBrightness(level, options);
			}
		}
		if (executeTrigger == ExecuteON.Trigger)
		{
			this.BrightUp = (ObjectBrightness)Delegate.Combine(this.BrightUp, new ObjectBrightness(Brightness_BrightUp));
		}
	}

	private void Brightness_BrightUp()
	{
		IsExecuting = true;
		StartCoroutine(updateValues());
	}

	private void Awake()
	{
		if (executeTrigger == ExecuteON.Awake)
		{
			if (!IsExecuting)
			{
				IsExecuting = true;
				StartCoroutine(updateValues());
			}
			if (useCurve)
			{
				AdjustBrightnessCurve(options);
			}
			else
			{
				AdjustBrightness(level, options);
			}
		}
	}

	private void Update()
	{
		if (executeTrigger == ExecuteON.Update)
		{
			if (!IsExecuting)
			{
				IsExecuting = true;
				StartCoroutine(updateValues());
			}
			if (useCurve)
			{
				AdjustBrightnessCurve(options);
			}
			else
			{
				AdjustBrightness(level, options);
			}
		}
		else if (IsExecuting)
		{
			if (useCurve)
			{
				AdjustBrightnessCurve(options);
			}
			else
			{
				AdjustBrightness(level, options);
			}
		}
	}

	private void FixedUpdate()
	{
		if (executeTrigger == ExecuteON.FixedUpdate)
		{
			if (!IsExecuting)
			{
				IsExecuting = true;
				StartCoroutine(updateValues());
			}
			if (useCurve)
			{
				AdjustBrightnessCurve(options);
			}
			else
			{
				AdjustBrightness(level, options);
			}
		}
	}

	public void AdjustBrightness(float val)
	{
		if (last + val > Min && last + val < Max)
		{
			if (base.gameObject.isStatic && base.renderer != null)
			{
				SetBrightness(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapFar, val);
				SetBrightness(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapNear, val);
			}
		}
		else
		{
			IsExecuting = false;
		}
	}

	public void AdjustBrightness(float val, AdjustLightmap options)
	{
		if (last + val > Min && last + val < Max)
		{
			if (base.gameObject.isStatic && base.renderer != null)
			{
				switch (options)
				{
				case AdjustLightmap.Both:
					SetBrightness(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapFar, val);
					SetBrightness(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapNear, val);
					break;
				case AdjustLightmap.Far:
					SetBrightness(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapFar, val);
					break;
				case AdjustLightmap.Near:
					SetBrightness(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapNear, val);
					break;
				}
			}
		}
		else
		{
			IsExecuting = false;
		}
	}

	public void AdjustBrightnessCurve(AdjustLightmap options)
	{
		if (base.gameObject.isStatic && base.renderer != null)
		{
			switch (options)
			{
			case AdjustLightmap.Both:
				SetBrightness(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapFar, currentCurveVal);
				SetBrightness(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapNear, currentCurveVal);
				break;
			case AdjustLightmap.Far:
				SetBrightness(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapFar, currentCurveVal);
				break;
			case AdjustLightmap.Near:
				SetBrightness(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapNear, currentCurveVal);
				break;
			}
		}
	}

	private IEnumerator updateValues()
	{
		if (!useCurve)
		{
			while (last + level > Min && last + level < Max)
			{
				last += level;
				yield return new WaitForSeconds(0.05f);
			}
			yield break;
		}
		while (currentCurve < (float)curve.length || loopCurve)
		{
			if (currentCurve >= (float)curve.length)
			{
				if (startOnFirstKey)
				{
					currentCurve = curve.keys[0].time;
				}
				else
				{
					currentCurve = 0f;
				}
			}
			currentCurveVal = curve.Evaluate(currentCurve);
			currentCurve += 0.05f / timeScale;
			yield return new WaitForSeconds(0.05f / timeScale);
		}
	}

	private void SetBrightness(Texture2D tex, float val)
	{
		if (operateOn == OperateON.CPU)
		{
			if (!(tex != null))
			{
				return;
			}
			Color[] pixels = tex.GetPixels();
			Color[] array = new Color[pixels.Length];
			for (int i = 0; i < pixels.Length; i++)
			{
				Color color = pixels[i];
				float num = color.r + val;
				float num2 = color.g + val;
				float num3 = color.b + val;
				if (num > 1f)
				{
					num = 1f;
				}
				else if (num < 0f)
				{
					num = 0f;
				}
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				else if (num2 < 0f)
				{
					num2 = 0f;
				}
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				else if (num3 < 0f)
				{
					num3 = 0f;
				}
				ref Color reference = ref array[i];
				reference = new Color(num, num2, num3, color.a);
			}
			tex.SetPixels(array);
			tex.Apply();
			return;
		}
		if (mats == null)
		{
			LoadMaterials();
		}
		foreach (Material mat in mats)
		{
			mat.SetFloat("_Adjust", last + val);
		}
	}
}
