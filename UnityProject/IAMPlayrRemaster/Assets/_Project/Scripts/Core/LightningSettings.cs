using UnityEngine;

public class LightningSettings : MonoBehaviour
{
	private Color color;

	private float minBrightness;

	private float maxBrightness;

	private float sharpness;

	private float motionRate;

	private float frequency;

	private float scale;

	private bool shouldExecute;

	public void Start()
	{
		Material material = base.gameObject.GetComponent<Renderer>().material;
		color = material.GetColor("_Color");
		minBrightness = material.GetFloat("_Brightness_Min");
		maxBrightness = material.GetFloat("_Brightness_Max");
		sharpness = material.GetFloat("_ElectricitySharpness");
		motionRate = material.GetFloat("_NoiseMotionRate");
		frequency = material.GetFloat("_NoiseFrequency");
		scale = material.GetFloat("_NoiseScale");
	}

	public void Update()
	{
		if (shouldExecute)
		{
			Material material = base.gameObject.GetComponent<Renderer>().material;
			material.SetColor("_Color", color);
			material.SetFloat("_Brightness_Min", minBrightness);
			material.SetFloat("_Brightness_Max", maxBrightness);
			material.SetFloat("_ElectricitySharpness", sharpness);
			material.SetFloat("_NoiseMotionRate", motionRate);
			material.SetFloat("_NoiseFrequency", frequency);
			material.SetFloat("_NoiseScale", scale);
		}
	}

	public void DrawGUI()
	{
		GUI.Box(new Rect(Screen.width - 270, 30f, 260f, 250f), string.Empty);
		GUILayout.BeginArea(new Rect(Screen.width - 270, 30f, 260f, 250f));
		GUILayout.Label("Color:");
		GUILayout.BeginHorizontal();
		GUILayout.Label("R:");
		color.r = GUILayout.HorizontalSlider(color.r, 0f, 1f, GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("G:");
		color.g = GUILayout.HorizontalSlider(color.g, 0f, 1f, GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("B:");
		color.b = GUILayout.HorizontalSlider(color.b, 0f, 1f, GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Min Brightness:");
		minBrightness = GUILayout.HorizontalSlider(minBrightness, 0f, 3f, GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Max Brightness:");
		maxBrightness = GUILayout.HorizontalSlider(maxBrightness, 0f, 3f, GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Sharpness:");
		sharpness = GUILayout.HorizontalSlider(sharpness, 0f, 10f, GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Motion Rate:");
		motionRate = GUILayout.HorizontalSlider(motionRate, 0f, 2f, GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Frequency:");
		frequency = GUILayout.HorizontalSlider(frequency, 0f, 2f, GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Scale:");
		scale = GUILayout.HorizontalSlider(scale, 0f, 3f, GUILayout.Width(150f));
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}
}
