using UnityEngine;

public class DotController : MonoBehaviour
{
	public GameObject dotPrefab;

	public UIScrollList scrollList;

	public float dotOffset = 0.5f;

	private PackedSprite[] m_dots;

	private int m_dotCount;

	private void Start()
	{
		Init();
	}

	public void Init()
	{
		m_dotCount = scrollList.sceneItems.Length;
		scrollList.AddItemSnappedDelegate(ScrollistDelegate);
		CreateDots();
	}

	private void ScrollistDelegate(IUIListObject item)
	{
		ChangeSelectedDot(scrollList.SnappedItem.Index);
	}

	public void CreateDots()
	{
		if (scrollList != null)
		{
			m_dots = new PackedSprite[m_dotCount];
			for (int i = 0; i < m_dotCount; i++)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(dotPrefab);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = new Vector3((0f - (float)m_dotCount) * dotOffset / 2f + dotOffset * (float)i, 0f, 0f);
				m_dots[i] = gameObject.GetComponent<PackedSprite>();
			}
			ChangeSelectedDot(0);
		}
	}

	private void ChangeSelectedDot(int index)
	{
		PackedSprite[] dots = m_dots;
		foreach (PackedSprite packedSprite in dots)
		{
			packedSprite.PlayAnim("empty");
		}
		m_dots[index].PlayAnim("full");
	}
}
