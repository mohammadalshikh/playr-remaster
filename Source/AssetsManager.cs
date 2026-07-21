using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsManager : MonoBehaviour
{
	private class GameAsset
	{
		public UnityEngine.Object obj;

		public bool bReady;
	}

	public const bool STADIUM_ASSET_BUNDLE_ISSUE = true;

	private const string COLOR_CROWD_PREFIX = "crowd_";

	private const string COLOR_CROWD_FLAT_PREFIX = "crowd_flat_";

	private const string ASSETS_PREFIX_DEFENDER = "DEF_";

	private const string ASSETS_PREFIX_GOALIE = "GK_";

	private static readonly string[] ASSET_BUNDLE_FOLDER = new string[7] { "Teams/", "GoalKeepers/", "Stadiums/", "Teams/", "Stadiums/", "SpecialCharacters/", "Videos" };

	private static readonly string[][] ASSET_BUNDLE_NAME = new string[7][]
	{
		new string[16]
		{
			"Birmingham", "Bournemouth", "Bristol", "Cardiff", "Chester", "Hartlepool", "Leeds", "Leicester", "Norwich", "Nottingham",
			"Oxford", "Plymouth", "RiverPark", "Sheffield", "SouthLondon", "York"
		},
		new string[6] { "GK_1_Blue", "GK_1_Orange", "GK_1_Red", "GK_3_Blue", "GK_3_Orange", "GK_3_Red" },
		new string[7] { "Stadium_RiverPark", "Stadium_Training", "Stadium_Oval", "Stadium_Away", "Basketball_Court", "Soccer_Academy", "Liverpool_Arena" },
		new string[2] { "RiverPark_Home", "RiverPark_Away" },
		new string[7] { "../Stadium_Away/Away_Colors_", null, "Oval_Colors_", "Away_Colors_", null, null, null },
		new string[1] { "Gary_Mac_Sprites" },
		new string[5] { "Gary_Mac_Power_Intro", "Gary_Mac_Curl_Intro", "Gary_Mac_Volley_Intro", "Gary_Mac_Deceive_Intro", "Gary_Mac_Corner_Intro" }
	};

	private static string[] RPC_MEMBER_NAME = new string[4] { "Dean", "Extra", "Laidlow", "Gary Mac" };

	public static string[] RPC_MEMBER_ANIM_NAME = new string[21]
	{
		"idle", "pass", "goal", "annoyed", "signal", "bkheel", "jump", "duck", "shoot_bk", "pass_bk",
		"pass_fw", "pass_left", "idle_forward_arms_behind", "idle_forward_crossed_arms", "idle_right_look_camera", "idle_left", "flickup_fwd", "runup", "pass_fwd", "coaching_clap_1",
		"coaching_clap_2"
	};

	private static AssetsManager m_Instance = null;

	private Dictionary<int, GameAsset> m_team_player_data = new Dictionary<int, GameAsset>();

	private Dictionary<int, GameAsset> m_rpfc_player_data = new Dictionary<int, GameAsset>();

	private int m_asset_request;

	public static AssetsManager Instance => m_Instance;

	public static int Requests => Instance.m_asset_request;

	public static RPFC_MEMBER_ID RPFC_MEMBER_ANIM_2_ID(RPFC_MEMBER_ANIM anim)
	{
		return anim switch
		{
			RPFC_MEMBER_ANIM.IDLE => RPFC_MEMBER_ID.RANDOM, 
			RPFC_MEMBER_ANIM.PASS => RPFC_MEMBER_ID.DEAN, 
			RPFC_MEMBER_ANIM.GOAL => RPFC_MEMBER_ID.DEAN, 
			RPFC_MEMBER_ANIM.ANNOYED => RPFC_MEMBER_ID.DEAN, 
			RPFC_MEMBER_ANIM.SIGNAL => RPFC_MEMBER_ID.EXTRA, 
			RPFC_MEMBER_ANIM.BACK_HEEL => RPFC_MEMBER_ID.LAIDLOW, 
			RPFC_MEMBER_ANIM.JUMP => RPFC_MEMBER_ID.LAIDLOW, 
			RPFC_MEMBER_ANIM.DUCK => RPFC_MEMBER_ID.LAIDLOW, 
			RPFC_MEMBER_ANIM.SHOOT_BK => RPFC_MEMBER_ID.LAIDLOW, 
			RPFC_MEMBER_ANIM.PASS_BK => RPFC_MEMBER_ID.LAIDLOW, 
			RPFC_MEMBER_ANIM.PASS_FW => RPFC_MEMBER_ID.LAIDLOW, 
			RPFC_MEMBER_ANIM.PASS_LEFT => RPFC_MEMBER_ID.LAIDLOW, 
			RPFC_MEMBER_ANIM.IDLE_FWD_1 => RPFC_MEMBER_ID.GARY_MAC, 
			RPFC_MEMBER_ANIM.IDLE_FWD_2 => RPFC_MEMBER_ID.GARY_MAC, 
			RPFC_MEMBER_ANIM.IDLE_RIGHT => RPFC_MEMBER_ID.GARY_MAC, 
			RPFC_MEMBER_ANIM.IDLE_LEFT => RPFC_MEMBER_ID.GARY_MAC, 
			RPFC_MEMBER_ANIM.FLICKUP => RPFC_MEMBER_ID.GARY_MAC, 
			RPFC_MEMBER_ANIM.RUNUP => RPFC_MEMBER_ID.GARY_MAC, 
			RPFC_MEMBER_ANIM.PASS_FWD => RPFC_MEMBER_ID.GARY_MAC, 
			RPFC_MEMBER_ANIM.COACHING_CLAP_1 => RPFC_MEMBER_ID.GARY_MAC, 
			RPFC_MEMBER_ANIM.COACHING_CLAP_2 => RPFC_MEMBER_ID.GARY_MAC, 
			_ => RPFC_MEMBER_ID.RANDOM, 
		};
	}

	private static bool CheckBundleCache(ASSET_BUNDLE_ID id)
	{
		return id == ASSET_BUNDLE_ID.GK || id == ASSET_BUNDLE_ID.STADIUM || id == ASSET_BUNDLE_ID.RPFC;
	}

	public void Initialize()
	{
		if (m_Instance != null)
		{
			Debug.LogError("AssetsManager can be instantiated only once");
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			m_Instance = this;
			base.gameObject.AddComponent<AssetBundleManager>().Initialize(7);
		}
	}

	public static string GetTeamName(int team_id)
	{
		return ASSET_BUNDLE_NAME[0][team_id];
	}

	public static void Reset()
	{
		Instance.m_team_player_data.Clear();
		Instance.m_rpfc_player_data.Clear();
		Instance.m_asset_request = 0;
	}

	public static void ReleaseBundles()
	{
		AssetBundleManager.Unload(-1, unloadAllLoadedObjects: true);
	}

	public static void SetData(string json_string)
	{
		AssetBundleManager.SetData(json_string);
	}

	public static void LoadRPFCMember(RPFC_MEMBER_ID member_id, SoccerPlayer_Sprite obj_ref)
	{
		Instance.m_asset_request++;
		Instance.StartCoroutine(Instance.RequestRPFCMember(member_id, obj_ref));
	}

	public static void LoadDefender(DEFENDER_ID defender_id, SoccerPlayer_Sprite obj_ref, bool bUnique)
	{
		Instance.m_asset_request++;
		Instance.StartCoroutine(Instance.RequestDefender(defender_id, obj_ref, bUnique));
	}

	public static void LoadGoalKeeper(SoccerPlayer_Sprite obj_ref)
	{
		Instance.m_asset_request++;
		Instance.StartCoroutine(Instance.RequestGoalKeeper(obj_ref));
	}

	public static void LoadStadium()
	{
		Debug.Log("load stadium");
		Instance.m_asset_request++;
		if (ASSET_BUNDLE_NAME[4][GameData.StadiumID] != null)
		{
			Debug.Log("load stadium colors");
			Instance.m_asset_request++;
			Instance.StartCoroutine(Instance.RequestStadiumColor());
		}
		LoadBundle(ASSET_BUNDLE_ID.STADIUM, GameData.StadiumID);
		Instance.StartCoroutine(Instance.RequestStadium(GameData.StadiumID));
	}

	public static void LoadBundle(ASSET_BUNDLE_ID bundle_id, int asset_id)
	{
		switch (bundle_id)
		{
		case ASSET_BUNDLE_ID.STADIUM_COLORS:
		{
			string text = ASSET_BUNDLE_NAME[4][GameData.StadiumID];
			if (text != null && asset_id >= 0)
			{
				AssetBundleManager.Load((int)bundle_id, ASSET_BUNDLE_FOLDER[(int)bundle_id] + ASSET_BUNDLE_NAME[2][GameData.StadiumID] + "/" + text + ASSET_BUNDLE_NAME[0][asset_id], CheckBundleCache(bundle_id));
			}
			break;
		}
		case ASSET_BUNDLE_ID.GK:
			AssetBundleManager.Load((int)bundle_id, ASSET_BUNDLE_FOLDER[(int)bundle_id] + GameData.GoalkeeperID, CheckBundleCache(bundle_id));
			break;
		default:
			AssetBundleManager.Load((int)bundle_id, ASSET_BUNDLE_FOLDER[(int)bundle_id] + ASSET_BUNDLE_NAME[(int)bundle_id][asset_id], CheckBundleCache(bundle_id));
			break;
		}
	}

	private IEnumerator LoadAsset(ASSET_BUNDLE_ID bundle_id, string name, Type type, GameAsset asset)
	{
		Debug.Log("LoadAsset (" + name + ")");
		if (AssetBundleManager.IsLoading((int)bundle_id))
		{
			yield return false;
		}
		while (AssetBundleManager.IsLoading((int)bundle_id))
		{
			yield return false;
		}
		if (!AssetBundleManager.IsLoaded((int)bundle_id))
		{
			Debug.LogError(string.Concat("LoadAsset: asset bundle load failed (", bundle_id, "), aborting = ", name));
			asset.obj = null;
			asset.bReady = true;
		}
		else
		{
			AssetBundleRequest request = AssetBundleManager.GetBundle((int)bundle_id).LoadAsync(name, type);
			yield return request;
			asset.obj = request.asset;
			asset.bReady = true;
			yield return true;
		}
	}

	private void InstantiateSuperSprite(GameObject obj_player, SoccerPlayer_Sprite obj_ref)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(obj_player);
		obj_ref.SetSuperSprite(gameObject.GetComponent<SuperSprite>());
	}

	private IEnumerator WaitSuperSprite(GameAsset asset_data, SoccerPlayer_Sprite obj_ref)
	{
		while (!asset_data.bReady)
		{
			yield return false;
		}
		if (asset_data.obj != null)
		{
			InstantiateSuperSprite((GameObject)asset_data.obj, obj_ref);
		}
		m_asset_request--;
		yield return true;
	}

	private IEnumerator RequestSuperSprite(ASSET_BUNDLE_ID bundle_id, string name, SoccerPlayer_Sprite obj_ref, GameAsset asset_data)
	{
		GameAsset asset = new GameAsset();
		yield return StartCoroutine(LoadAsset(bundle_id, name, typeof(GameObject), asset));
		if (asset_data != null)
		{
			asset_data.obj = asset.obj;
			asset_data.bReady = true;
		}
		if (asset.obj == null)
		{
			Debug.LogError("RequestSuperSprite (" + obj_ref.gameObject.name + ") : request asset not found = " + name);
			obj_ref.gameObject.active = false;
			m_asset_request--;
		}
		else
		{
			InstantiateSuperSprite((GameObject)asset.obj, obj_ref);
			m_asset_request--;
			yield return true;
		}
	}

	private void LoadSuperSprite(Dictionary<int, GameAsset> data_list, SoccerPlayer_Sprite obj_ref, ASSET_BUNDLE_ID bundle_id, int id, string asset_name)
	{
		if (data_list.TryGetValue(id, out var value))
		{
			if (value.bReady)
			{
				if (value.obj != null)
				{
					InstantiateSuperSprite((GameObject)value.obj, obj_ref);
				}
				else
				{
					obj_ref.gameObject.active = false;
				}
				m_asset_request--;
			}
			else
			{
				StartCoroutine(WaitSuperSprite(value, obj_ref));
			}
		}
		else
		{
			value = new GameAsset();
			data_list.Add(id, value);
			StartCoroutine(RequestSuperSprite(bundle_id, asset_name, obj_ref, value));
		}
	}

	private IEnumerator RequestRPFCMember(RPFC_MEMBER_ID member_id, SoccerPlayer_Sprite obj_ref)
	{
		while (GameData.GoalkeeperID == string.Empty)
		{
			yield return false;
		}
		int id = ((member_id != RPFC_MEMBER_ID.RANDOM) ? ((int)member_id) : UnityEngine.Random.Range(0, 3));
		string asset_name = ((id >= 3) ? RPC_MEMBER_NAME[id] : (GameData.RpfcKitAssetName + "_" + RPC_MEMBER_NAME[id]));
		ASSET_BUNDLE_ID bundle_id = ((id >= 3) ? ASSET_BUNDLE_ID.SPECIAL_CHARACTERS : ASSET_BUNDLE_ID.RPFC);
		LoadSuperSprite(m_rpfc_player_data, obj_ref, bundle_id, id, asset_name);
	}

	private IEnumerator RequestDefender(DEFENDER_ID defender_id, SoccerPlayer_Sprite obj_ref, bool bUnique)
	{
		yield return true;
	}

	private IEnumerator RequestGoalKeeper(SoccerPlayer_Sprite obj_ref)
	{
		while (GameData.GoalkeeperID == string.Empty)
		{
			yield return false;
		}
		StartCoroutine(RequestSuperSprite(ASSET_BUNDLE_ID.GK, GameData.GoalkeeperID, obj_ref, null));
	}

	private IEnumerator RequestStadium(int stadium_id)
	{
		Debug.Log("request stadium");
		GameAsset asset = new GameAsset();
		yield return StartCoroutine(LoadAsset(ASSET_BUNDLE_ID.STADIUM, ASSET_BUNDLE_NAME[2][stadium_id], typeof(GameObject), asset));
		if (asset.obj == null)
		{
			Debug.LogError("RequestStadium (" + stadium_id + ") [ " + ASSET_BUNDLE_NAME[2][stadium_id] + " ] : stadium not found");
			m_asset_request--;
			yield break;
		}
		while (GameData.Stadium == null)
		{
			yield return false;
		}
		GameData.Stadium.SetStadium((GameObject)UnityEngine.Object.Instantiate(asset.obj));
		if (GameData.Ball != null)
		{
			Rigidbody ball_rb = GameData.Ball.rigidbody;
			ball_rb.isKinematic = false;
			ball_rb.WakeUp();
		}
		AssetBundleManager.Unload(2, unloadAllLoadedObjects: false);
		m_asset_request--;
		yield return true;
	}

	private IEnumerator RequestStadiumColor()
	{
		yield return true;
	}
}
