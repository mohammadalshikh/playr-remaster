using UnityEngine;

public class StadiumHandler : MonoBehaviour
{
	private Stadium m_stadium_cmp;

	private GameObject m_stadium;

	public GameObject m_bounding;

	public GameObject m_shadow;

	public bool m_empty;

	public Material m_crowd_riverpark;

	public GameObject m_camera_flash;

	public float m_camera_flash_delay = 0.08f;

	public Texture m_hoarding_home;

	public Texture m_hoarding_away;

	public AudioClip[] m_audio_goal;

	public AudioClip[] m_audio_miss;

	public AudioClip m_audio_foul;

	public STADIUM_ID m_stadium_debug = STADIUM_ID.N_STADIUMS;

	public bool IsLoading => m_stadium == null || m_stadium.GetComponent<Stadium>() == null;

	private Stadium StadiumData => m_stadium.GetComponent<Stadium>();

	public GameObject Bounding => m_bounding;

	public GameObject PlayerShadow => m_shadow;

	public AudioSource[] Audio => GetComponents<AudioSource>();

	public bool IsEmpty => m_empty;

	public AudioClip AudioGoal => (m_audio_goal.Length <= 0 || IsEmpty) ? null : m_audio_goal[Random.Range(0, m_audio_goal.Length)];

	public AudioClip AudioMiss => (m_audio_miss.Length <= 0 || IsEmpty) ? null : m_audio_miss[Random.Range(0, m_audio_miss.Length)];

	public AudioClip AudioFoul => m_audio_foul;

	public Material Scoreboard_Screen => StadiumData.Scoreboard_Screen;

	public Vector3 Scoreboard_FocusPoint => StadiumData.Scoreboard_FocusPoint;

	public Vector3 Scoreboard_CameraTarget => StadiumData.Scoreboard_CameraTarget;

	public Texture2D CrowdColor
	{
		set
		{
			StadiumData.CrowdColor = value;
		}
	}

	public Texture2D CrowdFlatColor
	{
		set
		{
			StadiumData.CrowdFlatColor = value;
		}
	}

	public Texture2D StadiumColor
	{
		set
		{
			StadiumData.StadiumColor = value;
		}
	}

	public Color GetShading(Vector3 pos)
	{
		if (IsLoading)
		{
			Debug.LogError("call to GetShading before the stadium is loaded");
			return Color.white;
		}
		return StadiumData.GetShading(pos);
	}

	private void Awake()
	{
		GameData.CheckInstance();
		GameData.Stadium = this;
	}

	public void SetStadium(GameObject stadium_obj)
	{
		stadium_obj.transform.parent = base.transform;
		CrowdHandler component = stadium_obj.GetComponent<CrowdHandler>();
		if (component != null)
		{
			if (IsEmpty)
			{
				component.DestroyCrowd();
			}
			else
			{
				component.SetCameraFlash(m_camera_flash, m_camera_flash_delay);
			}
		}
		AdvertsAnimator component2 = stadium_obj.GetComponent<AdvertsAnimator>();
		if (component2 != null)
		{
			component2.SetTexture((!GameData.IsHomeMatch) ? m_hoarding_away : m_hoarding_home);
			component2.m_anim_length = new Vector2(4f, 1f);
		}
		m_stadium = stadium_obj;
	}
}
