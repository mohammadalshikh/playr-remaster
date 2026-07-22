using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class RIJNDAEL_encryption : MonoBehaviour
{
	public delegate void HandleEncryptionToolInitialised();

	private const int DEFAULT_KEY_LENGTH = 32;

	private const int DEFAULT_INITIALIZATION_VECTOR_LENGTH = 16;

	private const int DEFAULT_SEED = 69;

	private const int DEFAULT_DATA_SIZE_LENGTH = 10;

	private const PaddingMode DEFAULT_PADDING_MODE = PaddingMode.Zeros;

	private const string IAMPLAYR_MOBILE_KEY = "4dfjsdaDFGas8gh4jgd9h4aTGJ5423y0";

	private const string TEST_KEY = "wers top secret data key abcdefg";

	private const string TEST_INITIALIZATION_VECTOR16 = "1234567890123456";

	private const string TEST_INITIALIZATION_VECTOR32 = "12345678901234567890123456789012";

	private const string TEST_PLAINTEXT = "lets test a really big string to see if it breaks - blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah";

	private const string TEST_CYPHERTEXT = "b7I2ko2MpaY6tSHJfFqXVA==";

	private const bool NONE_RANDOM_IV = false;

	private const bool RANDOM_IV = true;

	private int m_key_length;

	private int m_initialization_vector_length;

	private int m_seed;

	private byte[] m_key;

	private int m_cypher_block_size;

	private CipherMode m_block_mode;

	private byte[] m_initialization_vector;

	private int m_data_size_length;

	private PaddingMode m_padding_mode;

	[method: MethodImpl(32)]
	public event HandleEncryptionToolInitialised EncryptionToolInitialised;

	private void Start()
	{
		m_key_length = 32;
		m_initialization_vector_length = 16;
		m_seed = 69;
		m_cypher_block_size = 128;
		m_block_mode = CipherMode.CBC;
		m_data_size_length = 10;
		m_padding_mode = PaddingMode.Zeros;
		set_key("4dfjsdaDFGas8gh4jgd9h4aTGJ5423y0");
		if (this.EncryptionToolInitialised != null)
		{
			this.EncryptionToolInitialised();
		}
	}

	private void set_block_mode(CipherMode block_mode)
	{
		m_block_mode = block_mode;
	}

	private void set_key(string new_key)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		m_key = uTF8Encoding.GetBytes(new_key);
	}

	private void set_cypher_block_size(int cypher_block_size)
	{
		m_cypher_block_size = cypher_block_size;
	}

	private void set_initialization_vector(string new_initialization_vector)
	{
		m_initialization_vector_length = new_initialization_vector.Length;
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		m_initialization_vector = uTF8Encoding.GetBytes(new_initialization_vector);
	}

	private void set_padding_mode(PaddingMode new_padding_mode)
	{
		m_padding_mode = new_padding_mode;
	}

	public string convert_to_hex(string input_string, int byte_count)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(input_string);
		string text = string.Empty;
		for (int i = 0; i < byte_count; i++)
		{
			int num = bytes[i];
			text += $"{Convert.ToUInt32(num.ToString()):x2}";
		}
		return text;
	}

	public void test_base64()
	{
		byte[] array = new byte[1];
		byte[] byte_array;
		for (int i = 0; i < 256; i++)
		{
			array[0] = (byte)i;
			string text = base64_encode(array);
			byte_array = base64_decode(text);
			Debug.Log("index = \"" + i + "\", test[0] = \"" + array[0] + "\", encoded = \"" + text + "\", decoded = \"" + byte_array_to_string(byte_array));
		}
		byte_array = base64_decode("U3lzdGVtLkJ5dGVbXTAwMDAwMDAwMDVvsjaSjYylpjq1Icl8WpdU");
		Debug.Log("decoded = \"" + byte_array);
	}

	public string base64_encode(byte[] byte_array)
	{
		return Convert.ToBase64String(byte_array);
	}

	public byte[] base64_decode(string base64_string)
	{
		return Convert.FromBase64String(base64_string);
	}

	public byte[] string_to_byte_array(string input_string)
	{
		byte[] array = new byte[input_string.Length];
		for (int i = 0; i < input_string.Length; i++)
		{
			array[i] = (byte)input_string[i];
		}
		return array;
	}

	public string byte_array_to_string(byte[] byte_array)
	{
		string text = string.Empty;
		for (int i = 0; i < byte_array.Length; i++)
		{
			text += Convert.ToChar(byte_array[i]);
		}
		return text;
	}

	public void test()
	{
		set_block_mode(CipherMode.CBC);
		set_cypher_block_size(256);
		set_key("wers top secret data key abcdefg");
		set_initialization_vector("12345678901234567890123456789012");
		set_padding_mode(PaddingMode.Zeros);
		print_settings();
		byte[] plaintext = string_to_byte_array("lets test a really big string to see if it breaks - blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah");
		string base64_string = base64_encode(encrypt(plaintext, random_iv: false));
		plaintext = decrypt(base64_decode(base64_string));
		set_block_mode(CipherMode.CBC);
		set_cypher_block_size("1234567890123456".Length * 8);
		set_key("wers top secret data key abcdefg");
		set_initialization_vector("1234567890123456");
		set_padding_mode(PaddingMode.Zeros);
		print_settings();
		plaintext = string_to_byte_array("lets test a really big string to see if it breaks - blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah, blah");
		byte[] array = encrypt(plaintext, random_iv: false);
		dump_byte_array(array);
		base64_string = base64_encode(array);
		plaintext = decrypt(base64_decode(base64_string));
		set_key("4dfjsdaDFGas8gh4jgd9h4aTGJ5423y0");
	}

	public string package_data(int data_length, string plaintext)
	{
		byte[] array = encrypt(string_to_byte_array(plaintext), random_iv: true);
		byte[] array2 = new byte[m_initialization_vector_length + 10 + array.Length];
		array2 = concatenate_byte_array(m_initialization_vector, string_to_byte_array(int_to_padded_string(data_length)));
		array2 = concatenate_byte_array(array2, array);
		return base64_encode(array2);
	}

	public byte[] concatenate_byte_array(byte[] original_array, byte[] added_array)
	{
		byte[] array = new byte[original_array.Length + added_array.Length];
		original_array.CopyTo(array, 0);
		added_array.CopyTo(array, original_array.Length);
		return array;
	}

	public string unpackage_data(string base64_packaged_data)
	{
		byte[] array = base64_decode(base64_packaged_data);
		byte[] array2 = new byte[m_initialization_vector_length];
		Array.Copy(array, 0, array2, 0, m_initialization_vector_length);
		m_initialization_vector = array2;
		byte[] array3 = new byte[m_initialization_vector_length];
		Array.Copy(array, m_initialization_vector_length, array3, 0, m_data_size_length);
		int length = padded_string_to_int(byte_array_to_string(array3));
		byte[] array4 = new byte[array.Length - (m_initialization_vector_length + m_data_size_length)];
		Array.Copy(array, m_initialization_vector_length + m_data_size_length, array4, 0, array.Length - (m_initialization_vector_length + m_data_size_length));
		string text = byte_array_to_string(decrypt(array4));
		return text.Substring(0, length);
	}

	public void test_encrypt()
	{
		set_block_mode(CipherMode.CBC);
		set_cypher_block_size(128);
		set_key("wers top secret data key abcdefg");
		set_initialization_vector("1234567890123456");
		set_padding_mode(PaddingMode.Zeros);
		print_settings();
	}

	public void test_decrypt()
	{
		set_block_mode(CipherMode.CBC);
		set_cypher_block_size(128);
		set_key("wers top secret data key abcdefg");
		set_initialization_vector("1234567890123456");
		set_padding_mode(PaddingMode.Zeros);
		print_settings();
	}

	public byte[] encrypt(byte[] plaintext, bool random_iv)
	{
		//Discarded unreachable code: IL_0120, IL_015b
		byte[] result = new byte[0];
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.KeySize = m_key.Length * 8;
		rijndaelManaged.Key = m_key;
		rijndaelManaged.Mode = CipherMode.CBC;
		rijndaelManaged.Padding = m_padding_mode;
		if (!random_iv)
		{
			rijndaelManaged.IV = m_initialization_vector;
		}
		else
		{
			m_initialization_vector = rijndaelManaged.IV;
		}
		rijndaelManaged.BlockSize = m_initialization_vector.Length * 8;
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(m_key, rijndaelManaged.IV), CryptoStreamMode.Write))
			{
				using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
				{
					streamWriter.Write(byte_array_to_string(plaintext));
					streamWriter.Close();
				}
				cryptoStream.Close();
			}
			result = memoryStream.ToArray();
			memoryStream.Close();
			return result;
		}
		catch (CryptographicException ex)
		{
			Console.WriteLine("A Cryptographic error occurred: {0}", ex.Message);
			Debug.Log("A Cryptographic error occurred: \"" + ex.Message + "\"");
			return null;
		}
		catch (UnauthorizedAccessException ex2)
		{
			Console.WriteLine("A file error occurred: {0}", ex2.Message);
			Debug.Log("A file error occurred: \"" + ex2.Message + "\"");
			return null;
		}
		catch (Exception ex3)
		{
			Console.WriteLine("An error occurred: {0}", ex3.Message);
			Debug.Log("An error occurred: \"" + ex3.Message + "\"");
			return result;
		}
		finally
		{
			rijndaelManaged.Clear();
		}
	}

	public byte[] decrypt(byte[] encoded)
	{
		//Discarded unreachable code: IL_013b, IL_0176
		string input_string = null;
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.KeySize = m_key.Length * 8;
		rijndaelManaged.Key = m_key;
		rijndaelManaged.BlockSize = m_initialization_vector.Length * 8;
		rijndaelManaged.IV = m_initialization_vector;
		rijndaelManaged.Mode = CipherMode.CBC;
		rijndaelManaged.Padding = m_padding_mode;
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			string text = byte_array_to_string(encoded);
			memoryStream.Write(encoded, 0, text.Length);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			dump_stream_characteristics(memoryStream);
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(m_key, m_initialization_vector), CryptoStreamMode.Read))
			{
				StreamReader streamReader = new StreamReader(cryptoStream);
				try
				{
					input_string = streamReader.ReadLine();
				}
				catch (Exception ex)
				{
					Console.WriteLine("An error occurred: {0}", ex.Message);
					Debug.Log("An error occurred reading from stream" + ex.Message);
				}
				cryptoStream.Close();
			}
			memoryStream.Close();
		}
		catch (CryptographicException ex2)
		{
			Console.WriteLine("A Cryptographic error occurred: {0}", ex2.Message);
			Debug.Log("A Cryptographic error occurred: \"" + ex2.Message + "\"");
			return null;
		}
		catch (UnauthorizedAccessException ex3)
		{
			Console.WriteLine("A file error occurred: {0}", ex3.Message);
			Debug.Log("A file error occurred: \"" + ex3.Message + "\"");
			return null;
		}
		catch (Exception ex4)
		{
			Console.WriteLine("An error occurred: {0}", ex4.Message);
			Debug.Log("An error occurred: \"" + ex4.Message + "\"");
		}
		finally
		{
			rijndaelManaged.Clear();
		}
		return string_to_byte_array(input_string);
	}

	public string ConvertToHex(byte[] ascii_array, int byte_count)
	{
		string text = string.Empty;
		for (int i = 0; i < byte_count; i++)
		{
			int num = ascii_array[i];
			text += $"{Convert.ToUInt32(num.ToString()):x2}";
		}
		return text;
	}

	public string int_to_padded_string(int number)
	{
		return number.ToString("D10");
	}

	public int padded_string_to_int(string number_string)
	{
		int.TryParse(number_string, out var result);
		return result;
	}

	private void print_settings()
	{
		Debug.Log("\n");
		Debug.Log("key_length = \"" + m_key_length + "\"");
		Debug.Log("initialization_vector_length = \"" + m_initialization_vector_length + "\"");
		Debug.Log("seed = \"" + m_seed + "\"");
		Debug.Log("cypher_block_size = \"" + m_cypher_block_size + "\"");
		Debug.Log("data_size_length = \"" + m_data_size_length + "\"");
		Debug.Log(string.Concat("block_mode = \"", m_block_mode, "\""));
		Debug.Log(string.Concat("padding_mode = \"", m_padding_mode, "\""));
		string text = byte_array_to_string(m_key);
		Debug.Log("key = \"" + text + "\"");
		string text2 = byte_array_to_string(m_initialization_vector);
		Debug.Log("initialization_vector = \"" + text2 + "\"");
		Debug.Log("\n");
	}

	private void dump_byte_array(byte[] bytes)
	{
		for (int i = 0; i < bytes.Length; i++)
		{
			Debug.Log("bytes[" + i + "] = \"" + bytes[i] + "\"");
		}
	}

	private void dump_stream_characteristics(MemoryStream memory_stream)
	{
	}
}
