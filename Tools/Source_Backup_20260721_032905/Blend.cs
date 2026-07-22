using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Blend : MonoBehaviour
{
	public enum OperateON
	{
		GPU,
		CPU
	}

	public delegate void TextureBlend();

	public OperateON operateOn = OperateON.CPU;

	public bool useSharedMaterials;

	private List<Material> mats;

	public bool useCurve = true;

	public AnimationCurve curve = new AnimationCurve();

	public bool loopCurve = true;

	public bool startOnFirstKey = true;

	public float timeScale = 1f;

	private float currentCurve;

	private float currentCurveVal;

	public Texture2D BlendTexture;

	public float level;

	private float last;

	public Brightness.AdjustLightmap options;

	public Brightness.ExecuteON executeTrigger;

	public float Min = -1f;

	public float Max = 1f;

	private bool IsExecuting;

	[method: MethodImpl(32)]
	public event TextureBlend blendNow;

	private void LoadMaterials()
	{
		mats = new List<Material>();
		Material[] array = null;
		array = ((!useSharedMaterials) ? base.renderer.materials : base.renderer.sharedMaterials);
		Material[] array2 = array;
		foreach (Material material in array2)
		{
			Debug.Log(material.shader.name);
			if (material.shader.name != "LightmappingEffects/BlendDiffuse")
			{
				material.shader = Shader.Find("LightmappingEffects/BlendDiffuse");
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
		if (executeTrigger == Brightness.ExecuteON.Start)
		{
			if (!IsExecuting)
			{
				IsExecuting = true;
				StartCoroutine(updateValues());
			}
			if (useCurve)
			{
				AdjustBlendingCurve(options, BlendTexture);
			}
			else
			{
				AdjustBlending(level, options, BlendTexture);
			}
		}
		else if (executeTrigger == Brightness.ExecuteON.Trigger)
		{
			this.blendNow = (TextureBlend)Delegate.Combine(this.blendNow, new TextureBlend(Blend_blendNow));
		}
		else if (IsExecuting)
		{
			AdjustBlending(level, options, BlendTexture);
		}
	}

	private void FixedUpdate()
	{
		if (executeTrigger == Brightness.ExecuteON.FixedUpdate)
		{
			if (!IsExecuting)
			{
				IsExecuting = true;
				StartCoroutine(updateValues());
			}
			if (useCurve)
			{
				AdjustBlendingCurve(options, BlendTexture);
			}
			else
			{
				AdjustBlending(level, options, BlendTexture);
			}
		}
	}

	private void Awake()
	{
		if (executeTrigger == Brightness.ExecuteON.Awake)
		{
			if (!IsExecuting)
			{
				IsExecuting = true;
				StartCoroutine(updateValues());
			}
			if (useCurve)
			{
				AdjustBlendingCurve(options, BlendTexture);
			}
			else
			{
				AdjustBlending(level, options, BlendTexture);
			}
		}
	}

	private void Blend_blendNow()
	{
		IsExecuting = true;
		StartCoroutine(updateValues());
	}

	public void BlendTextures()
	{
		if (this.blendNow != null)
		{
			this.blendNow();
		}
	}

	private void Update()
	{
		if (executeTrigger == Brightness.ExecuteON.Update)
		{
			if (!IsExecuting)
			{
				IsExecuting = true;
				StartCoroutine(updateValues());
			}
			if (useCurve)
			{
				AdjustBlendingCurve(options, BlendTexture);
			}
			else
			{
				AdjustBlending(level, options, BlendTexture);
			}
		}
		else if (IsExecuting)
		{
			if (useCurve)
			{
				AdjustBlendingCurve(options, BlendTexture);
			}
			else
			{
				AdjustBlending(level, options, BlendTexture);
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

	public void AdjustBlending(float val, Brightness.AdjustLightmap options, Texture2D dest)
	{
		if (!(BlendTexture != null))
		{
			return;
		}
		if (last + val > Min && last + val < Max)
		{
			if (base.gameObject.isStatic && base.renderer != null)
			{
				switch (options)
				{
				case Brightness.AdjustLightmap.Both:
					BlendNow(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapFar, dest, val);
					BlendNow(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapNear, dest, val);
					break;
				case Brightness.AdjustLightmap.Far:
					BlendNow(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapFar, dest, val);
					break;
				case Brightness.AdjustLightmap.Near:
					BlendNow(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapNear, dest, val);
					break;
				}
			}
			else
			{
				Debug.Log("Lightmap Manager requires a static gameObject with a renderer to do this job.");
			}
		}
		else
		{
			IsExecuting = false;
		}
	}

	public void AdjustBlendingCurve(Brightness.AdjustLightmap options, Texture2D dest)
	{
		if (base.gameObject.isStatic && base.renderer != null)
		{
			switch (options)
			{
			case Brightness.AdjustLightmap.Both:
				BlendNow(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapFar, dest, currentCurveVal);
				BlendNow(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapNear, dest, currentCurveVal);
				break;
			case Brightness.AdjustLightmap.Far:
				BlendNow(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapFar, dest, currentCurveVal);
				break;
			case Brightness.AdjustLightmap.Near:
				BlendNow(LightmapSettings.lightmaps[base.renderer.lightmapIndex].lightmapNear, dest, currentCurveVal);
				break;
			}
		}
		else
		{
			Debug.Log("Lightmap Manager requires a static gameObject with a renderer to do this job.");
		}
	}

	private void BlendNow(Texture2D tex1, Texture2D tex2, float val)
	{
		if (operateOn == OperateON.CPU)
		{
			if (tex1.width == tex2.width && tex1.height == tex2.height)
			{
				Color[] pixels = tex1.GetPixels();
				Color[] array = new Color[pixels.Length];
				Color[] pixels2 = tex2.GetPixels();
				for (int i = 0; i < pixels.Length; i++)
				{
					Color color = pixels[i];
					Color color2 = pixels2[i];
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					float num;
					if (color.r < color2.r)
					{
						num = color.r + val;
						flag = true;
					}
					else
					{
						num = color.r - val;
						flag = false;
					}
					float num2;
					if (color.g < color2.g)
					{
						num2 = color.g + val;
						flag2 = true;
					}
					else
					{
						num2 = color.g - val;
						flag2 = false;
					}
					float num3;
					if (color.b < color2.b)
					{
						num3 = color.b + val;
						flag3 = true;
					}
					else
					{
						num3 = color.b - val;
						flag3 = false;
					}
					float num4;
					if (color.a < color2.a)
					{
						num4 = color.a + val;
						flag4 = true;
					}
					else
					{
						num4 = color.a - val;
						flag4 = false;
					}
					if ((num > color2.r && flag) || (num < color2.r && !flag))
					{
						num = color2.r;
					}
					if ((num2 > color2.g && flag2) || (num2 < color2.g && !flag2))
					{
						num2 = color2.g;
					}
					if ((num3 > color2.b && flag3) || (num3 < color2.b && !flag3))
					{
						num3 = color2.b;
					}
					if ((num4 > color2.a && flag4) || (num4 < color2.a && !flag4))
					{
						num4 = color2.a;
					}
					ref Color reference = ref array[i];
					reference = new Color(num, num2, num3, num4);
				}
				tex1.SetPixels(array);
				tex1.Apply();
			}
			else
			{
				Debug.Log("Lightmap Manager requires the blended textures to have the same size in order to blend them.");
			}
			return;
		}
		if (mats == null)
		{
			LoadMaterials();
		}
		foreach (Material mat in mats)
		{
			mat.SetFloat("_Adjust", last + val);
			mat.SetTexture("_BlendTex", tex2);
		}
	}
}
