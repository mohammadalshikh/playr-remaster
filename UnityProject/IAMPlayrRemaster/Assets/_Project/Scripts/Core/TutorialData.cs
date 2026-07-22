using System;
using UnityEngine;

[Serializable]
public class TutorialData
{
	[SerializeField]
	private string _title = string.Empty;

	[SerializeField]
	private string _content = string.Empty;

	public string Title => _title;

	public string Content => _content;
}
