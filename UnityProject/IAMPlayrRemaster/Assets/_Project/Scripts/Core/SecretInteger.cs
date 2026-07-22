using UnityEngine;

public class SecretInteger
{
	private static int[] s_store_mask;

	private int[] m_hidden_value;

	private int m_hidden_factor;

	public int m_display_value;

	public static int StoreMask => s_store_mask[0];

	public SecretInteger()
	{
		SetupSecretInteger(0);
	}

	public SecretInteger(int start_value)
	{
		SetupSecretInteger(start_value);
	}

	private void SetupSecretInteger(int start_value)
	{
		m_hidden_factor = 47 + (int)(Random.value * 340f);
		m_display_value = start_value;
		if (s_store_mask == null)
		{
			s_store_mask = new int[1];
			s_store_mask[0] = (int)(Random.value * 16777215f);
		}
		m_hidden_value = new int[1];
		m_hidden_value[0] = (start_value * m_hidden_factor) ^ s_store_mask[0];
	}

	public int GetValue()
	{
		return m_display_value;
	}

	public int GetChecksum1()
	{
		return m_hidden_factor;
	}

	public int GetChecksum2()
	{
		return m_hidden_value[0];
	}

	public void AddValue(int val)
	{
		int[] hidden_value = m_hidden_value;
		int[] array = ((!(Random.value * 10f > 5f)) ? m_hidden_value : (m_hidden_value = new int[1]));
		array[0] = ((hidden_value[0] ^ s_store_mask[0]) + val * m_hidden_factor) ^ s_store_mask[0];
		m_display_value += val;
	}
}
