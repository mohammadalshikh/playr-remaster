using System;
using System.Collections;
using UnityEngine;

public class CrowdHandler : MonoBehaviour
{
	[Serializable]
	public class CrowdAnimBlock
	{
		public GameObject[] block;
	}

	public CrowdAnimBlock[] crowd;

	public Vector3 m_move_offset;

	public float m_move_anim_length;

	public float m_start_anim_delay = 0.2f;

	private GameObject m_camera_flash;

	private float m_flash_delay = 0.08f;

	private bool m_start_anims = true;

	private float m_anim_timer;

	private int m_anim_index;

	private void StartAnim()
	{
		GameObject[] block = crowd[m_anim_index].block;
		foreach (GameObject gameObject in block)
		{
			if (gameObject.active)
			{
				CrowdTranslate crowdTranslate = gameObject.AddComponent<CrowdTranslate>();
				crowdTranslate.Offset = m_move_offset;
				crowdTranslate.AnimLength = m_move_anim_length;
			}
		}
		m_anim_index++;
	}

	public void SetCameraFlash(GameObject camera_flash, float flash_delay)
	{
		m_camera_flash = camera_flash;
		m_flash_delay = flash_delay;
	}

	private IEnumerator GenerateFlash()
	{
		while (m_camera_flash == null)
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (true)
		{
			GameObject[] block = crowd[UnityEngine.Random.Range(0, crowd.Length)].block;
			GameObject obj = block[UnityEngine.Random.Range(0, block.Length)];
			if (obj.active)
			{
				Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
				Transform flash = ((GameObject)UnityEngine.Object.Instantiate(m_camera_flash)).transform;
				int tri = 3 * (UnityEngine.Random.Range(0, mesh.triangles.Length) / 3);
				Vector3 vert_0 = mesh.vertices[mesh.triangles[tri]];
				flash.position = vert_0 + (mesh.vertices[mesh.triangles[tri + UnityEngine.Random.Range(1, 3)]] - vert_0) * UnityEngine.Random.Range(0f, 1f) + mesh.normals[0] * 1.5f;
				flash.parent = base.transform;
				yield return new WaitForSeconds(m_flash_delay);
			}
		}
	}

	private void Start()
	{
		StartAnim();
		StartCoroutine(GenerateFlash());
	}

	private void Update()
	{
		if (!m_start_anims)
		{
			return;
		}
		m_anim_timer += Time.deltaTime;
		if (m_anim_timer > m_start_anim_delay)
		{
			StartAnim();
			if (m_anim_index == crowd.Length)
			{
				m_start_anims = false;
			}
			m_anim_timer = 0f;
		}
	}

	public void DestroyCrowd()
	{
		CrowdAnimBlock[] array = crowd;
		foreach (CrowdAnimBlock crowdAnimBlock in array)
		{
			GameObject[] block = crowdAnimBlock.block;
			foreach (GameObject obj in block)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}
		base.enabled = false;
	}
}
