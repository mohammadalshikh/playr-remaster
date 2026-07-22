using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightningManager : MonoBehaviour
{
	private List<LightningSettings> arcs;

	private int selected;

	public void Start()
	{
		arcs = (Object.FindObjectsOfType(typeof(LightningSettings)) as LightningSettings[]).ToList();
	}

	public void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width - 270, 5f, 260f, 25f));
		List<string> list = new List<string>();
		foreach (LightningSettings arc in arcs)
		{
			list.Add("Arc " + arcs.IndexOf(arc));
		}
		selected = GUILayout.Toolbar(selected, list.ToArray());
		GUILayout.EndArea();
		arcs[selected].DrawGUI();
	}
}
