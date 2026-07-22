using System.Collections;
using UnityEngine;

public class SoccerPlayer_Sprite : MonoBehaviour
{
	private const float PHYSICS_BOX_DEPTH = 0.05f;

	protected PhysicMaterial m_phy_material;

	protected BoxCollider m_collider;

	protected bool m_generate_shadow = true;

	private Vector3 m_shadow_center = new Vector3(0.426f, 0f, -0.31f);

	protected bool m_face_camera = true;

	private PackedSprite m_sprite;

	private Transform m_sprite_transform;

	private float m_anim_time;

	public SuperSprite m_super_sprite;

	private Transform m_face_camera_transform;

	private Transform m_face_camera_target;

	protected bool m_phy_xy = true;

	private bool m_apply_half_time_rotation = true;

	private Color m_shading = new Color(1f, 1f, 1f, 1f);

	public SuperSprite Sprite => m_super_sprite;

	public void SkipHalfTimeRotation()
	{
		m_apply_half_time_rotation = false;
	}

	public void OnAnimPlay(SuperSpriteAnim anim)
	{
		m_sprite = (PackedSprite)anim.CurrentSprite;
		m_sprite_transform = m_sprite.gameObject.transform;
		m_sprite.SetColor(m_shading);
	}

	public virtual void Awake()
	{
		if ((bool)m_super_sprite)
		{
			SetSuperSprite(m_super_sprite);
		}
	}

	private IEnumerator GenerateShadow()
	{
		while (GameData.Stadium == null || GameData.Stadium.IsLoading)
		{
			yield return false;
		}
		GameObject shadow2 = GameData.Stadium.PlayerShadow;
		if (shadow2 != null)
		{
			shadow2 = (GameObject)Object.Instantiate(shadow2);
			shadow2.transform.parent = base.transform;
			shadow2.transform.localPosition = new Vector3(m_shadow_center.x, m_shadow_center.y + shadow2.transform.localPosition.y, m_shadow_center.z);
		}
		yield return true;
	}

	public virtual void Start()
	{
		if (m_apply_half_time_rotation)
		{
			base.transform.position = GameData.HalfTimeRotation(base.transform.position);
		}
		m_face_camera_transform = GameData.CameraTransform;
		StartCoroutine(SetupShading());
		if (m_generate_shadow)
		{
			StartCoroutine(GenerateShadow());
		}
	}

	private IEnumerator SetupShading()
	{
		while (GameData.Stadium.IsLoading)
		{
			yield return false;
		}
		m_shading = GameData.Stadium.GetShading(base.transform.position);
		if (m_sprite != null)
		{
			m_sprite.SetColor(m_shading);
		}
		yield return true;
	}

	public virtual void SetSuperSprite(SuperSprite sprite)
	{
		m_super_sprite = sprite;
		Transform transform = sprite.gameObject.transform;
		Vector3 position = transform.position;
		transform.parent = base.transform;
		transform.localPosition = position;
		if ((bool)m_phy_material)
		{
			m_collider = sprite.gameObject.AddComponent<BoxCollider>();
			m_collider.size = Vector3.zero;
			m_collider.material = m_phy_material;
		}
		m_face_camera_target = sprite.gameObject.transform;
		if ((bool)m_collider)
		{
			m_collider.isTrigger = true;
		}
	}

	public virtual void Update()
	{
		if (!m_sprite)
		{
			return;
		}
		if ((bool)m_collider)
		{
			if (!m_collider.isTrigger)
			{
				m_collider.center = m_sprite.GetCenterPoint() + m_sprite_transform.localPosition;
				if (m_phy_xy)
				{
					m_collider.size = new Vector3(m_sprite.width, m_sprite.height, 0.05f);
				}
				else
				{
					m_collider.size = new Vector3(0.05f, m_sprite.height, m_sprite.width);
				}
			}
			else if ((bool)GameData.PlayerControl && GameData.PlayerControl.HasKicked())
			{
				m_collider.isTrigger = false;
			}
		}
		if (m_anim_time == 0f || !m_super_sprite.IsAnimating())
		{
			return;
		}
		if (m_anim_time < 0f)
		{
			m_anim_time += Time.deltaTime;
			if (!(m_anim_time >= 0f))
			{
			}
			return;
		}
		m_anim_time -= Time.deltaTime;
		Vector3 vector;
		if (m_face_camera)
		{
			vector = (m_sprite_transform.position - m_face_camera_transform.position).normalized;
			vector.y = 0f;
		}
		else
		{
			vector = m_sprite_transform.forward;
		}
		base.transform.localPosition += vector * Time.deltaTime;
		if (m_anim_time < 0f)
		{
			m_anim_time = 0f;
		}
	}

	private void LateUpdate()
	{
		if ((bool)m_face_camera_target && m_face_camera)
		{
			Vector3 vector = m_face_camera_target.position + m_face_camera_transform.forward;
			m_face_camera_target.LookAt(new Vector3(vector.x, m_face_camera_target.position.y, vector.z));
		}
	}
}
