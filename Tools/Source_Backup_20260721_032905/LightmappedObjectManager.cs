using System.Collections.Generic;
using UnityEngine;

public class LightmappedObjectManager : MonoBehaviour
{
	public List<LightmappedObjectPrefabs> LightmappedObjects;

	public LightmappedObjectPrefabs FindByGameObject(GameObject obj)
	{
		LightmappedObjectPrefabs result = null;
		foreach (LightmappedObjectPrefabs lightmappedObject in LightmappedObjects)
		{
			if (lightmappedObject.obj.GetInstanceID() == obj.GetInstanceID())
			{
				return lightmappedObject;
			}
		}
		return result;
	}
}
