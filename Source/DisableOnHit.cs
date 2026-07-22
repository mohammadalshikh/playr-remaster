using System.Collections;
using UnityEngine;

public class DisableOnHit : MonoBehaviour
{
	public GameObject targetObject;

	public bool recursively;

	public float delay;

	private void Start()
	{
		if (targetObject == null)
		{
			targetObject = base.gameObject;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (delay == 0f)
		{
			Disable();
		}
		else
		{
			StartCoroutine(OnAction());
		}
	}

	public void OnCollisionEnter(Collision other)
	{
		if (delay == 0f)
		{
			Disable();
		}
		else
		{
			StartCoroutine(OnAction());
		}
	}

	private IEnumerator OnAction()
	{
		yield return new WaitForSeconds(delay);
		Disable();
	}

	private void Disable()
	{
		if (base.gameObject != null)
		{
			if (recursively)
			{
				base.gameObject.SetActiveRecursively(state: false);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
