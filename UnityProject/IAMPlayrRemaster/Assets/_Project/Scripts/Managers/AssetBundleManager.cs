using System;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using UnityEngine;

public class AssetBundleManager : MonoBehaviour
{
	private enum ASSET_BUNDLE_STATUS
	{
		LOADING,
		LOADED,
		INVALID
	}

	private class AssetBundleData
	{
		public AssetBundle bundle;

		public ASSET_BUNDLE_STATUS status;
	}

	public delegate void OnAssetBundleLoaded(int bundle_id, bool bLoaded);

	private const string ASSET_BUNDLES_EXT = ".assetbundle";

	private const string ASSET_BUNDLES_FOLDER_WEB = "unity/AssetBundles/";

	private const string ASSET_BUNDLES_FOLDER_LOCAL = "/StreamingAssetsLocal/";

	private const int ASSET_BUNDLE_DOWNLOAD_TRIES = 3;

	private static int ASSET_BUNDLE_VERSION = 2;

	private static string CDN_PREFIX = string.Empty;

	private static string SERVER_PREFIX = string.Empty;

	private static AssetBundleManager m_Instance;

	private AssetBundleData[] m_asset_bundles;

	public static AssetBundleManager Instance => m_Instance;

	private bool IsValidBundle(int bundle_id)
	{
		return m_asset_bundles != null && m_asset_bundles.Length > bundle_id;
	}

	public static bool IsLoading(int bundle_id)
	{
		return Instance.IsValidBundle(bundle_id) && Instance.m_asset_bundles[bundle_id].status == ASSET_BUNDLE_STATUS.LOADING;
	}

	public static bool IsLoaded(int bundle_id)
	{
		return Instance.IsValidBundle(bundle_id) && Instance.m_asset_bundles[bundle_id].status == ASSET_BUNDLE_STATUS.LOADED;
	}

	public static AssetBundle GetBundle(int bundle_id)
	{
		return (!IsLoaded(bundle_id)) ? null : Instance.m_asset_bundles[bundle_id].bundle;
	}

	public void Initialize(int n_asset_bundles)
	{
		if (m_Instance != null)
		{
			Debug.LogError("AssetBundleManager can be instantiated only once");
			UnityEngine.Object.Destroy(this);
			return;
		}
		m_Instance = this;
		m_asset_bundles = new AssetBundleData[n_asset_bundles];
		for (int num = n_asset_bundles - 1; num >= 0; num--)
		{
			m_asset_bundles[num] = new AssetBundleData();
			m_asset_bundles[num].status = ASSET_BUNDLE_STATUS.LOADING;
			m_asset_bundles[num].bundle = null;
		}
	}

	private IEnumerator LoadBundle(int asset_bundle_id, string bundle_name, bool check_cache = false, OnAssetBundleLoaded onLoadedCB = null)
	{
		string data_path;
		// WebPlayer is obsolete - always use local/StreamingAssets path
		if (false)
		{
			while (CDN_PREFIX == string.Empty)
			{
				yield return false;
			}
			data_path = "unity/AssetBundles/" + bundle_name + ".assetbundle";
		}
		else
		{
			CDN_PREFIX = "file://" + Application.dataPath;
			SERVER_PREFIX = CDN_PREFIX;
			data_path = "/StreamingAssetsLocal/" + bundle_name + ".assetbundle";
		}
		int download_retry = 3;
		WWW download;
		float download_time2;
		do
		{
			string download_url = ((download_retry <= 1 && 0 == 0) ? SERVER_PREFIX : CDN_PREFIX) + data_path;
			download = ((!check_cache) ? new WWW(download_url) : WWW.LoadFromCacheOrDownload(download_url, ASSET_BUNDLE_VERSION));
			Debug.Log("LoadAssetBundle: downloading = " + download.url);
			download_time2 = Time.realtimeSinceStartup;
			while (!download.isDone && download.progress != 1f)
			{
				yield return false;
			}
			yield return download;
			download_time2 = Time.realtimeSinceStartup - download_time2;
			if (download.error == null && download.assetBundle != null)
			{
				m_asset_bundles[asset_bundle_id].bundle = download.assetBundle;
				m_asset_bundles[asset_bundle_id].status = ASSET_BUNDLE_STATUS.LOADED;
				continue;
			}
			Debug.LogError("LoadAssetBundle (" + download_time2 + ") = " + bundle_name + " ----- retry = " + download_retry + " ----- download error = " + download.error);
			Dictionary<string, object> json_dictionary = new Dictionary<string, object>
			{
				{
					"error_code",
					ERROR_CODE.ASSET_BUNDLE_DOWNLOAD
				},
				{ "error_msg", download.error },
				{ "error_url", download_url }
			};
			Application.ExternalCall("OnUnityError", JsonWriter.Serialize(json_dictionary));
			download_retry--;
			yield return false;
		}
		while (download.error != null && download_retry > 0);
		if (download_retry == 0)
		{
			Debug.LogError("LoadAssetBundle (" + download_time2 + ") = " + bundle_name + " --- invalid file");
			m_asset_bundles[asset_bundle_id].status = ASSET_BUNDLE_STATUS.INVALID;
		}
		onLoadedCB?.Invoke(asset_bundle_id, download_retry == 0);
		yield return true;
	}

	public static void Load(int asset_bundle_id, string bundle_name, bool check_cache = false, OnAssetBundleLoaded onLoadedCB = null)
	{
		if (!Instance.IsValidBundle(asset_bundle_id))
		{
			Debug.LogError("AssetBundleManager.Load asset_bundle_id is too big (" + asset_bundle_id + "); check your AssetbundleManager.Initialize function call");
			onLoadedCB?.Invoke(asset_bundle_id, bLoaded: false);
		}
		else if (Instance.m_asset_bundles[asset_bundle_id].status == ASSET_BUNDLE_STATUS.LOADED)
		{
			Debug.Log("AssetBundleManager.Load " + bundle_name + " (" + asset_bundle_id + ") has already been loaded");
			onLoadedCB?.Invoke(asset_bundle_id, bLoaded: true);
		}
		else
		{
			Instance.StartCoroutine(Instance.LoadBundle(asset_bundle_id, bundle_name, check_cache, onLoadedCB));
		}
	}

	public static void Unload(int bundle_id, bool unloadAllLoadedObjects)
	{
		Debug.Log("AssetBundleManager.Unload " + bundle_id);
		if (bundle_id < 0)
		{
			for (int num = Instance.m_asset_bundles.Length - 1; num >= 0; num--)
			{
				AssetBundleData assetBundleData = Instance.m_asset_bundles[num];
				assetBundleData.status = ASSET_BUNDLE_STATUS.LOADING;
				if (assetBundleData.bundle != null)
				{
					assetBundleData.bundle.Unload(unloadAllLoadedObjects);
					assetBundleData.bundle = null;
				}
			}
		}
		else if (Instance.IsValidBundle(bundle_id))
		{
			AssetBundleData assetBundleData2 = Instance.m_asset_bundles[bundle_id];
			assetBundleData2.status = ASSET_BUNDLE_STATUS.LOADING;
			if (assetBundleData2.bundle != null)
			{
				assetBundleData2.bundle.Unload(unloadAllLoadedObjects);
				assetBundleData2.bundle = null;
			}
		}
		else
		{
			Debug.LogError("AssetBundleManager.Unload invalid bundle_id");
		}
		GC.Collect();
	}

	public static void SetData(string json_string)
	{
		JsonReaderSettings jsonReaderSettings = new JsonReaderSettings();
		jsonReaderSettings.TypeHintName = "__type";
		jsonReaderSettings.AllowNullValueTypes = true;
		jsonReaderSettings.AllowUnquotedObjectKeys = true;
		JsonReader jsonReader = new JsonReader(json_string, jsonReaderSettings);
		Dictionary<string, object> dictionary = (Dictionary<string, object>)jsonReader.Deserialize();
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			if (item.Value != null && !((string)item.Value == string.Empty))
			{
				switch (item.Key)
				{
				case "CDN":
					CDN_PREFIX = (string)item.Value;
					break;
				case "Server":
					SERVER_PREFIX = (string)item.Value;
					break;
				case "AssetBundleVersion":
					ASSET_BUNDLE_VERSION = int.Parse((string)item.Value);
					break;
				}
			}
		}
	}
}
