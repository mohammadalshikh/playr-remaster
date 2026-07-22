using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LightmappedObjectPrefabs
{
	public GameObject obj;

	public List<string> Prefabs;

	public LightmappedObjectPrefabs()
	{
		Prefabs = new List<string>();
	}

	public LightmappedObjectPrefabs(GameObject o)
	{
		obj = o;
		Prefabs = new List<string>();
	}
}
