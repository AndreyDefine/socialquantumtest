using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Кеширующая загрузка Asset Bundle
/// </summary>
public class CachingLoad : MonoBehaviour {
	public string BundleURL;
	public string AssetName;
	public int version;
	public	ObjectLoadedCallbackDelegateWithCustomCallBack ObjectLoadedCallback;
	public ObjectLoadedCallbackDelegate customcallback;
	
	private AssetBundle bundle=null;

	public void StartLoading() {
		StartCoroutine (DownloadAndCache());
	}

	IEnumerator DownloadAndCache (){
		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;

		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
		// using "using" because of throwing
		using(WWW www = WWW.LoadFromCacheOrDownload (BundleURL, version)){
			yield return www;
			if (www.error != null)
			{
				DownloadFinished();
				throw new Exception("WWW download had an error:" + www.error);
			}
			bundle = www.assetBundle;
            // Unload the AssetBundles compressed contents to conserve memory
			DownloadFinished();
		} // memory is freed from the web stream (www.Dispose() gets called implicitly)
	}
	
	private void DownloadFinished()
	{
		ObjectLoadedCallback(bundle,AssetName,customcallback);
		DestroyGameObject();
	}
	
	public void DestroyGameObject()
	{
		Destroy(gameObject);
	}
}
