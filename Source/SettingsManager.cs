using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
	private Dictionary<string, object> jsonData = new Dictionary<string, object>();

	private void Start()
	{
		jsonData = (Dictionary<string, object>)(object)Resources.Load("JSONFiles/Settings.json", typeof(Dictionary<string, object>));
	}

	public Dictionary<string, object> GetJSONSettings()
	{
		foreach (KeyValuePair<string, object> jsonDatum in jsonData)
		{
			if (jsonDatum.Key == "settings")
			{
				Debug.Log((Dictionary<string, object>)jsonDatum.Value);
				return (Dictionary<string, object>)jsonDatum.Value;
			}
		}
		return null;
	}
}
