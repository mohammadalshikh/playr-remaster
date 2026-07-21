using System.Collections;
using UnityEngine;

public class DestroyOnHit : MonoBehaviour
{
	public GameObject targetObject;

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
			Destroy();
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
			Destroy();
		}
		else
		{
			StartCoroutine(OnAction());
		}
	}

	private IEnumerator OnAction()
	{
		yield return new WaitForSeconds(delay);
		Destroy();
	}

	private void Destroy()
	{
		if (targetObject != null)
		{
			Object.Destroy(targetObject);
		}
	}
}
