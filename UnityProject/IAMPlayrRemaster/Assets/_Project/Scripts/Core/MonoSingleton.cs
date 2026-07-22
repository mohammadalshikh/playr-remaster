using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			string message = typeof(T).Name + " is not attached to a gameObject (or it;s not active).\nAlso be sure to set 'Instance = this;' in your Awake() function";
			Debug.LogError(message);
			throw new NullReferenceException(message);
		}
		protected set
		{
			_instance = value;
		}
	}

	public static bool IsInitialized => _instance != null;

	protected virtual void OnApplicationQuit()
	{
		_instance = default(T);
	}
}
