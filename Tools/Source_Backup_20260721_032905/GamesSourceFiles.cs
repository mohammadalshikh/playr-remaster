using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GamesSourceFiles
{
	public List<TextAsset> gameTextFiles;

	private int m_file_index;

	public int FileIndex
	{
		get
		{
			return m_file_index;
		}
		set
		{
			m_file_index = value;
		}
	}

	private TextAsset GameWithName(string name)
	{
		foreach (TextAsset gameTextFile in gameTextFiles)
		{
			if (gameTextFile.name.Equals(name))
			{
				return gameTextFile;
			}
		}
		Debug.LogError("No game was found with name " + name);
		return null;
	}

	public string GetFile(string name)
	{
		if (gameTextFiles.Count == 0)
		{
			Debug.LogError("No TextAssets have been assigned in the inspector for gameSources");
			return string.Empty;
		}
		return GameWithName(name).text;
	}
}
