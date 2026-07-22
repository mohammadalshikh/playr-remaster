using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class IndexManager : MonoBehaviour
{
	public List<ObjectIndexer> SceneObjects;

	public ArrayList cachedObjects;

	public IndexManager()
	{
		SceneObjects = new List<ObjectIndexer>();
	}

	public ObjectIndexer FindByID(int id)
	{
		ObjectIndexer result = null;
		foreach (ObjectIndexer sceneObject in SceneObjects)
		{
			if (sceneObject.ID == id)
			{
				return sceneObject;
			}
		}
		return result;
	}

	public ObjectIndexer FindByGameObject(GameObject obj)
	{
		ObjectIndexer result = null;
		foreach (ObjectIndexer sceneObject in SceneObjects)
		{
			if (sceneObject.obj.GetInstanceID() == obj.GetInstanceID())
			{
				return sceneObject;
			}
		}
		return result;
	}
}
