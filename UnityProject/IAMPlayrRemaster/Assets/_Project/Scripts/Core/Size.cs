using System;
using UnityEngine;

[Serializable]
public class Size
{
	public SIZE initialSize;

	public SIZE endSize;

	public float duration;

	public float startDelay;

	public bool loop;

	[HideInInspector]
	public float maxSize;
}
public enum SIZE
{
	DEFAULT_MEDIUM,
	MINIATURE,
	SMALL,
	LARGE,
	HUGE
}
