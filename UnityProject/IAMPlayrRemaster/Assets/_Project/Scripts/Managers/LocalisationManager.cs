using System.Collections;
using UnityEngine;

public class LocalisationManager : MonoSingleton<LocalisationManager>
{
	[SerializeField]
	private TextAsset localistationFile;

	private Hashtable results;

	private void Awake()
	{
		MonoSingleton<LocalisationManager>.Instance = this;
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		results = (Hashtable)MiniJSON.jsonDecode(localistationFile.text);
	}

	public string GetLocalisedText(string key)
	{
		if (results.Contains(key))
		{
			return results[key].ToString();
		}
		return "NO KEY FOUND";
	}
}
