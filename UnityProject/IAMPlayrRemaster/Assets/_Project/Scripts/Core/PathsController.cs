using UnityEngine;

public class PathsController : MonoBehaviour
{
	public GameObject targetObject;

	private void Awake()
	{
		GameData.CheckInstance();
		GameData.PathManager = this;
	}

	public void FollowPath(string pathName, float time)
	{
		SetTrailTime(targetObject.GetComponent<TrailRenderer>(), time);
		iTween.MoveTo(targetObject, iTween.Hash("path", iTweenPath.GetPath(pathName), "time", 0.5, "movetopath", false, "easetype", iTween.EaseType.easeInOutSine));
	}

	public void FollowPath(GameObject target, string pathName, float time)
	{
		SetTrailTime(target.GetComponent<TrailRenderer>(), time);
		iTween.MoveTo(target, iTween.Hash("path", iTweenPath.GetPath(pathName), "time", 0.5, "movetopath", false, "easetype", iTween.EaseType.easeInOutSine));
	}

	public void ShowTrail(bool show)
	{
		if (targetObject.GetComponent<TrailRenderer>() != null)
		{
			targetObject.GetComponent<TrailRenderer>().enabled = show;
		}
	}

	private void SetTrailTime(TrailRenderer trail, float time)
	{
		if (trail != null)
		{
			trail.time = time;
		}
	}
}
