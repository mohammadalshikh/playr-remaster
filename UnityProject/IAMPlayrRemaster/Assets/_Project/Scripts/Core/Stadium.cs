using System;
using System.Collections;
using UnityEngine;

public class Stadium : MonoBehaviour
{
	[Serializable]
	public class MaterialStates
	{
		public Material m_material;

		public Color m_light_colour;

		public Color m_dark_colour;
	}

	private static Stadium m_Instance;

	public Material m_crowd_material;

	public Material m_crowd_flat_material;

	public Material m_stadium_material;

	public Material m_scoreboard_screen;

	public Vector3 m_scoreboard_focus = new Vector3(65f, 20f, -0.01f);

	public Vector3 m_scoreboard_distance = new Vector3(-6f, -2f, 0f);

	public MaterialStates[] m_material_states;

	public GameObject[] m_goal_objects;

	public static Stadium Instance
	{
		get
		{
			return m_Instance;
		}
		set
		{
			m_Instance = value;
		}
	}

	public bool IsLoading => false;

	public Texture2D CrowdColor
	{
		set
		{
			if (m_crowd_material != null)
			{
				m_crowd_material.mainTexture = value;
			}
		}
	}

	public Texture2D CrowdFlatColor
	{
		set
		{
			if (m_crowd_flat_material != null)
			{
				m_crowd_flat_material.mainTexture = value;
			}
		}
	}

	public Texture2D StadiumColor
	{
		set
		{
			if (m_stadium_material != null)
			{
				m_stadium_material.mainTexture = value;
			}
		}
	}

	public Material Scoreboard_Screen => m_scoreboard_screen;

	public Vector3 Scoreboard_FocusPoint => m_scoreboard_focus;

	public Vector3 Scoreboard_CameraTarget => m_scoreboard_focus + m_scoreboard_distance;

	public Color GetShading(Vector3 pos)
	{
		if (m_Instance == null)
		{
			return Color.white;
		}
		ShadingColor[] componentsInChildren = Instance.GetComponentsInChildren<ShadingColor>();
		ShadingColor[] array = componentsInChildren;
		foreach (ShadingColor shadingColor in array)
		{
			if (shadingColor.Contains(pos))
			{
				return shadingColor.Color;
			}
		}
		return Color.white;
	}

	public virtual void ToggleDay()
	{
	}

	public virtual void ToggleNight()
	{
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("only one stadium per scenario is allowed, deleting " + base.gameObject.name);
			base.enabled = false;
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		m_scoreboard_focus = GameData.HalfTimeRotation(m_scoreboard_focus);
		m_scoreboard_distance = GameData.HalfTimeRotation(m_scoreboard_distance);
	}

	public void DarkenScene(float time)
	{
		if (m_material_states != null && m_material_states.Length > 0)
		{
			MaterialStates[] material_states = m_material_states;
			foreach (MaterialStates materialStates in material_states)
			{
				StartCoroutine(AnimColor(materialStates.m_material, materialStates.m_light_colour, materialStates.m_dark_colour, time));
			}
		}
	}

	public void LightenScene(float time)
	{
		if (m_material_states != null && m_material_states.Length > 0)
		{
			MaterialStates[] material_states = m_material_states;
			foreach (MaterialStates materialStates in material_states)
			{
				StartCoroutine(AnimColor(materialStates.m_material, materialStates.m_dark_colour, materialStates.m_light_colour, time));
			}
		}
	}

	public void SetGoalsVisible(bool visible)
	{
		if (m_goal_objects != null && m_goal_objects.Length > 0)
		{
			GameObject[] goal_objects = m_goal_objects;
			foreach (GameObject gameObject in goal_objects)
			{
				gameObject.SetActive(visible);
			}
		}
	}

	private IEnumerator AnimColor(Material mtl, Color start_col, Color end_col, float time)
	{
		float currTime = 0f;
		string color_name = "_Color";
		if (time > 0f)
		{
			mtl.SetColor(color_name, start_col);
			while (currTime <= time)
			{
				if (!mtl)
				{
					yield break;
				}
				mtl.SetColor(color_name, start_col + (end_col - start_col) * currTime / time);
				currTime += Time.deltaTime;
				yield return false;
			}
		}
		mtl.color = end_col;
		yield return false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(m_scoreboard_focus, 1f);
	}
}
