using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using JsonFx.Json;
using UnityEngine;

public class WebIntegrationManager : MonoSingleton<WebIntegrationManager>
{
	public delegate void HandleWebUserData(int matchFitness, int matchFitnessReq, bool isMatchFitnessDisabled);

	public delegate void HandleFixtureData(string homeTeamName, string awayTeamName);

	private const string baseLoginUrl = "https://app.iamplayr.com/";

	private const string userDataUrl = "Server/Commands/User/GetMobileData";

	private const string decryptURL = "Server/Commands/User/Decrypt";

	private const string fixtureDataUrl = "Server/Commands/User/NextFixtureDetails";

	[SerializeField]
	private RIJNDAEL_encryption encryptionObj;

	private SecretInteger secretWeatherKey;

	private string opponentName;

	private bool isMatchUnlocked;

	private bool isWebPlayer;

	private bool isWebUserDataReceived;

	public string OpponentName => opponentName;

	public bool IsMatchUnlocked => isMatchUnlocked;

	public bool IsWebPlayer => isWebPlayer;

	public bool IsWebUserDataReceived => isWebUserDataReceived;

	[method: MethodImpl(32)]
	public event HandleWebUserData WebUserDataReceived;

	[method: MethodImpl(32)]
	public event HandleFixtureData FixtureDataReceived;

	private void Awake()
	{
		MonoSingleton<WebIntegrationManager>.Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		FacebookManager.sessionOpenedEvent += RefreshData;
		GameData.PlayerStats.MatchFitnessEarnedChanged += delegate
		{
			StartCoroutine(PostData());
		};
	}

	private IEnumerator GetWebData(string commandUrl)
	{
		WWW www = new WWW("https://app.iamplayr.com/" + commandUrl + "?AccessToken=" + Facebook.instance.accessToken);
		yield return www;
		if (www.error != null)
		{
			Debug.LogError(www.error);
			yield return null;
		}
		Dictionary<string, object> jsonData = GetJSONData(www.text);
		if (jsonData != null)
		{
			switch (commandUrl)
			{
			case "Server/Commands/User/GetMobileData":
				ProcessUserData(jsonData);
				break;
			case "Server/Commands/User/NextFixtureDetails":
				ProcessFixtureData(jsonData);
				break;
			}
		}
	}

	private Dictionary<string, object> GetJSONData(string json)
	{
		JsonReaderSettings jsonReaderSettings = new JsonReaderSettings();
		jsonReaderSettings.TypeHintName = "__type";
		jsonReaderSettings.AllowNullValueTypes = true;
		jsonReaderSettings.AllowUnquotedObjectKeys = true;
		JsonReader jsonReader = new JsonReader(json, jsonReaderSettings);
		object obj = jsonReader.Deserialize();
		Dictionary<string, object> result = null;
		if (obj != null && obj is Dictionary<string, object>)
		{
			result = (Dictionary<string, object>)obj;
		}
		else
		{
			Debug.LogError("Invalid result json string");
		}
		return result;
	}

	private void ProcessUserData(Dictionary<string, object> userData)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<string, object> userDatum in userData)
		{
			if (userDatum.Key == "Result")
			{
				dictionary = (Dictionary<string, object>)userDatum.Value;
			}
		}
		int num = 0;
		int num2 = 0;
		bool isMatchFitnessDisabled = false;
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			if (item.Key == "MatchFitness")
			{
				num = Convert.ToInt32(item.Value);
			}
			if (item.Key == "_MatchFitnessThreshold")
			{
				num2 = Convert.ToInt32(item.Value);
			}
			if (item.Key == "_MobileWeather")
			{
				secretWeatherKey = new SecretInteger(Convert.ToInt32(item.Value));
			}
			if (item.Key == "_MobileMatchFitnessDisabled")
			{
				isMatchFitnessDisabled = item.Value.ToString() == "0";
			}
			if (item.Key == "_Week")
			{
				isWebPlayer = Convert.ToInt32(item.Value) != 0;
				if (isWebPlayer)
				{
					PlayerManager.SendFeedback("PlayedWebMatch", string.Empty, string.Empty);
				}
			}
		}
		if (num != 0 && num >= num2)
		{
			isMatchUnlocked = true;
		}
		else
		{
			isMatchUnlocked = false;
		}
		if (this.WebUserDataReceived != null)
		{
			this.WebUserDataReceived(num, num2, isMatchFitnessDisabled);
		}
		isWebUserDataReceived = true;
	}

	private IEnumerator RefreshSecretKey()
	{
		WWW www = new WWW("https://app.iamplayr.com/Server/Commands/User/GetMobileData?AccessToken=" + Facebook.instance.accessToken);
		yield return www;
		if (www.error != null)
		{
			Debug.LogError(www.error);
			yield return null;
		}
		Dictionary<string, object> userData = GetJSONData(www.text);
		if (userData == null)
		{
			yield break;
		}
		Dictionary<string, object> results = new Dictionary<string, object>();
		foreach (KeyValuePair<string, object> kvp2 in userData)
		{
			if (kvp2.Key == "Result")
			{
				results = (Dictionary<string, object>)kvp2.Value;
			}
		}
		foreach (KeyValuePair<string, object> kvp in results)
		{
			if (kvp.Key == "_MobileWeather")
			{
				secretWeatherKey = new SecretInteger(Convert.ToInt32(kvp.Value));
			}
		}
	}

	private void ProcessFixtureData(Dictionary<string, object> fixtureData)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<string, object> fixtureDatum in fixtureData)
		{
			if (fixtureDatum.Key == "Result")
			{
				dictionary = (Dictionary<string, object>)fixtureDatum.Value;
			}
		}
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			if (item.Key == "HomeClub")
			{
				dictionary2 = (Dictionary<string, object>)item.Value;
			}
			if (item.Key == "AwayClub")
			{
				dictionary3 = (Dictionary<string, object>)item.Value;
			}
		}
		Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
		foreach (KeyValuePair<string, object> item2 in dictionary2)
		{
			if (item2.Key == "ClubName")
			{
				dictionary4 = (Dictionary<string, object>)item2.Value;
			}
		}
		Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
		foreach (KeyValuePair<string, object> item3 in dictionary3)
		{
			if (item3.Key == "ClubName")
			{
				dictionary5 = (Dictionary<string, object>)item3.Value;
			}
		}
		string text = string.Empty;
		string text2 = string.Empty;
		foreach (KeyValuePair<string, object> item4 in dictionary4)
		{
			if (item4.Key == "UntranslatedText")
			{
				text = item4.Value.ToString();
			}
		}
		foreach (KeyValuePair<string, object> item5 in dictionary5)
		{
			if (item5.Key == "UntranslatedText")
			{
				text2 = item5.Value.ToString();
			}
		}
		if (text != "River Park FC")
		{
			opponentName = text;
		}
		if (text2 != "River Park FC")
		{
			opponentName = text2;
		}
		if (this.FixtureDataReceived != null)
		{
			this.FixtureDataReceived(text, text2);
		}
	}

	private IEnumerator UpdateMatchFitness()
	{
		Dictionary<string, object> jsonDictionary = new Dictionary<string, object>();
		SecretInteger secretMatchFitness = new SecretInteger(GameData.PlayerStats.MatchFitnessEarned);
		jsonDictionary.Add("MatchFitness", secretMatchFitness.GetValue());
		secretWeatherKey.AddValue(-GameData.PlayerStats.MatchFitnessEarned);
		int[] checksumData = new int[4]
		{
			SecretInteger.StoreMask,
			(secretMatchFitness.GetChecksum1() << 10) + secretWeatherKey.GetChecksum1(),
			secretMatchFitness.GetChecksum2(),
			secretWeatherKey.GetChecksum2()
		};
		jsonDictionary.Add("checksum", ConvertArray(checksumData));
		string writtenData = JsonWriter.Serialize(jsonDictionary);
		string encryptedData = encryptionObj.package_data(writtenData.Length, writtenData);
		WWWForm form = new WWWForm();
		form.AddField("data", encryptedData);
		form.AddField("Type", "MobileDrill");
		form.AddField("AccessToken", Facebook.instance.accessToken);
		secretWeatherKey = null;
		WWW www = new WWW("https://app.iamplayr.com/Server/Commands/User/Decrypt", form);
		yield return www;
		if (www.error != null)
		{
			Debug.LogError(www.error);
			yield return null;
		}
	}

	private IEnumerator PostData()
	{
		if (FacebookHandler.IsSessionValid())
		{
			yield return StartCoroutine(RefreshSecretKey());
			if (secretWeatherKey != null)
			{
				StartCoroutine(UpdateMatchFitness());
			}
		}
	}

	public void RefreshData()
	{
		if (Application.internetReachability != 0 && FacebookHandler.IsSessionValid())
		{
			StartCoroutine(GetWebData("Server/Commands/User/GetMobileData"));
			StartCoroutine(GetWebData("Server/Commands/User/NextFixtureDetails"));
		}
	}

	private string ConvertArray(int[] array)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 998527;
		foreach (int num2 in array)
		{
			stringBuilder.Append((((num2 >> 16) ^ num) & 0xFF).ToString("x2"));
			num += 254731;
			stringBuilder.Append((((num2 >> 8) ^ num) & 0xFF).ToString("x2"));
			num += 254731;
			stringBuilder.Append(((num2 ^ num) & 0xFF).ToString("x2"));
			num += 254731;
			stringBuilder.Append((((num2 >> 24) ^ num) & 0xFF).ToString("x2"));
			num += 254731;
		}
		return stringBuilder.ToString();
	}
}
