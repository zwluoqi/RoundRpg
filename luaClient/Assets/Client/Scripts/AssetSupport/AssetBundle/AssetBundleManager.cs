using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AssetBundleManager {




	public void UpdateAssetBundleMainfestInfo(string mainfestParentPath, string abMainfestPath){
		//TODO
		this.mainfestParentPath = PathTool.FixedPath(mainfestParentPath+"/");
		AssetBundle ab = AssetBundle.LoadFromFile(abMainfestPath);
		abMainfest = ab.LoadAsset ("assetbundlemanifest") as AssetBundleManifest;
		ab.Unload (false);
	}

	public AssetBundleManifest abMainfest;
	public string mainfestParentPath;



	public Object GetAssetBundleAsset(string bundlePath,string assetName){
		FixedBundleFilePath (ref bundlePath);
		AssetCounter asset = null;
		if (assetDicts.TryGetValue (bundlePath + "/" + assetName, out asset)) {
			return asset.obj;
		}
		return null;
	}

	public static void FixedBundleFilePath(ref string bundlePath){
		if (!bundlePath.EndsWith (".ab")) {
			bundlePath += bundlePath+".ab";
		}
		bundlePath = PathTool.FixedPath (bundlePath);
		bundlePath = bundlePath.ToLower ();
	}
	#region LoadAndUnload
	public Dictionary<string,BundleCounter> bundleDicts = new Dictionary<string, BundleCounter> ();
	public Dictionary<string,AssetCounter> assetDicts = new Dictionary<string, AssetCounter> ();

	/// <summary>
	/// Loads the object form asset bundle.
	/// 外部唯一同步加载家口加载OBJECT
	/// -------------------记载完对象后会立即删除AssetBundle
	/// </summary>
	/// <returns>The object form asset bundle.</returns>
	/// <param name="bundlePath">Bundle path.</param>
	/// <param name="assetName">Asset name.</param>
	public Object LoadObjectFormAssetBundle(string bundlePath,string assetName){
		FixedBundleFilePath (ref bundlePath);

		AssetCounter asset = null;
		assetDicts.TryGetValue (bundlePath + "/" + assetName, out asset);
		if (asset != null) {
			asset.count++;
			return asset.obj;
		}
		if (!bundleDicts.ContainsKey (bundlePath)) {
			return HandlerLoadAssetBundleWithDepedence (bundlePath, assetName);
		} else {
			return HandlerLoadAssetBundle (bundlePath, assetName);
		}
	}

	public AssetBundle LoadAssetBundle(string bundlePath){
		FixedBundleFilePath (ref bundlePath);

		BundleCounter bc = null;
		if (bundleDicts.TryGetValue (bundlePath, out bc)) {
			bc = bundleDicts [bundlePath];
			bc.count++;
		} else {
			AssetBundle ab = AssetBundle.LoadFromFile (mainfestParentPath + "/" + bundlePath);
			bc = new BundleCounter (ab, 1, bundlePath);
			bundleDicts [bundlePath] = bc; 
		}
		return bc.asset;
	}


	private Object HandlerLoadAssetBundleWithDepedence(string bundlePath,string assetName){
		string[] dependencies = abMainfest.GetAllDependencies (bundlePath);
		if (dependencies.Length > 0) {
			foreach (var de in dependencies) {
				Debug.LogWarning (bundlePath + " HandlerLoadAssetBundleWithDepedence:" + de );
				LoadAssetBundle (de);
				int lastSplitIndex = de.LastIndexOf ('/');
				string de_assetName = de.Substring (lastSplitIndex + 1);
				HandlerLoadAssetBundle (de, de_assetName);
			}
			LoadAssetBundle (bundlePath);
			return HandlerLoadAssetBundle (bundlePath, assetName);
		} else {
			LoadAssetBundle (bundlePath);
			return HandlerLoadAssetBundle (bundlePath, assetName);
		}
	}

	private Object HandlerLoadAssetBundle(string bundlePath,string assetName){
		AssetBundle ab = bundleDicts [bundlePath].asset;

		if (ab != null) {
			Object obj =  ab.LoadAsset (assetName);
			UnLoadAssetBundle (bundlePath);
			AssetCounter ac = new AssetCounter (obj, 1, bundlePath + "/" + assetName);
			assetDicts [bundlePath + "/" + assetName] = ac;
			return obj;
		} else {
			return null;
		}
	}


	/// <summary>
	/// Loads the object form asset bundle async.
	/// 外部唯一异步加载接口加载对象
	/// -------------------记载完对象后会立即删除AssetBundle
	/// </summary>
	/// <param name="bundlePath">Bundle path.</param>
	/// <param name="assetName">Asset name.</param>
	/// <param name="act">Act.</param>
	public void LoadObjectFormAssetBundleAsync(string bundlePath,string assetName,System.Action<string, string,Object> act){
		FixedBundleFilePath (ref bundlePath);

		AssetCounter asset = null;
		assetDicts.TryGetValue (bundlePath + "/" + assetName, out asset);
		if (asset != null) {
			asset.count++;
			act (bundlePath, assetName, asset.obj);
			return;
		} else {
			if (!bundleDicts.ContainsKey (bundlePath)) {
				HandlerLoadAssetBundleAsyncWithDepedence (bundlePath, assetName, act);
			} else {
				HandlerLoadAssetBundleAsync (bundlePath, assetName, act);
			}
		}
	}



	private IEnumerator LoadAssetBundleAsync(string bundlePath,System.Action<string,AssetBundle> act){
		BundleCounter bc = null;
		if (bundleDicts.TryGetValue (bundlePath, out bc)) {
			bc = bundleDicts [bundlePath];
			bc.count++;
		} else {
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync (mainfestParentPath + "/" + bundlePath);
			yield return request;
			bc = new BundleCounter (request.assetBundle, 1, bundlePath);
			bundleDicts [bundlePath] = bc;
		}
		act (bundlePath,bc.asset);
	}



	private void HandlerLoadAssetBundleAsyncWithDepedence(string bundlePath,string assetName,System.Action<string, string,Object> act){
		string[] dependencies = abMainfest.GetAllDependencies (bundlePath);



		if (dependencies.Length > 0) {
			Dictionary<string,bool> dependenciesLoadAsyncState = new Dictionary<string, bool> ();

			System.Action<string> loadDepedenceCallback = delegate(string _de) {
				dependenciesLoadAsyncState [_de] = true;
				foreach (var state in dependenciesLoadAsyncState) {
					if (!state.Value) {
						return;
					}
				}
				ResourceManager.Instance.StartCoroutine (LoadAssetBundleAsync (bundlePath,
					(_bundlePath, _ab) => {
						HandlerLoadAssetBundleAsync (bundlePath, assetName, act);
					}
				));
			};
				
			foreach (var de in dependencies) {
				dependenciesLoadAsyncState [de] = false;
			}

			foreach (var de in dependencies) {
				Debug.LogWarning (bundlePath + " HandlerLoadAssetBundleAsyncWithDepedence:" + de );

				ResourceManager.Instance.StartCoroutine (LoadAssetBundleAsync (de,
					(_de, _de_ab) => {
						int lastSplitIndex = _de.LastIndexOf ('/');
						string de_assetName = _de.Substring (lastSplitIndex + 1);
						HandlerLoadAssetBundleAsync (_de, de_assetName,null);

						loadDepedenceCallback(_de);
					}
				));
			}
		} else {
			ResourceManager.Instance.StartCoroutine( LoadAssetBundleAsync (bundlePath,
				(_bundlePath,_ab)=>
				{
					HandlerLoadAssetBundleAsync (bundlePath, assetName, act);
				}
			));
		}

	}


	private void HandlerLoadAssetBundleAsync(string bundlePath,string assetName,System.Action<string, string,Object> act){
		AssetBundle ab = bundleDicts [bundlePath].asset;
		if (ab == null) {
			if (act != null) {
				act (bundlePath, assetName, null);
			}
			return;
		}
			
		Object obj =  ab.LoadAsset (assetName);
		UnLoadAssetBundle (bundlePath);

		AssetCounter ac = new AssetCounter (obj, 1, bundlePath + "/" + assetName);
		assetDicts [bundlePath + "/" + assetName] = ac;
		if (act != null) {
			act (bundlePath, assetName, obj);
		}
	}

	public void UnLoadAssetBundle(string bundlePath){
		FixedBundleFilePath (ref bundlePath);


		BundleCounter bc = null;
		if (bundleDicts.TryGetValue (bundlePath, out bc)) {
			bc.count--;
			if (bc.count <= 0) {
				bc.asset.Unload (false);
				bundleDicts.Remove (bundlePath);
			}
		}
	}


	public void UnLoadObject(string bundlePath,string assetName){
		FixedBundleFilePath (ref bundlePath);

		AssetCounter ac = null;
		if (assetDicts.TryGetValue (bundlePath + "/" + assetName,out ac)) {
			ac.count--;
			if (ac.count <= 0) {
				//TODO
				assetDicts.Remove(bundlePath + "/" + assetName);
			}
		}
	}
	#endregion

	#region Tool

	public void PrintLoadDataInfo(){
		foreach (var bc in bundleDicts) {
			Debug.LogWarning ("bundle path:" + bc.Key + " count:" + bc.Value.count);
		}
		foreach (var ac in assetDicts) {
			Debug.LogWarning ("bundle assetObject path:" + ac.Key + " count:" + ac.Value.count);
		}
	}
	#endregion

	public class BundleCounter
	{
		public int count;
		public AssetBundle asset;
		public string bundleName;
		public BundleCounter(AssetBundle asset, int count, string bundleName)
		{
			this.count = count;
			this.asset = asset;
			this.bundleName = bundleName;
		}
	}

	public class AssetCounter{
		public int count;
		public Object obj;
		public string assetName;
		public AssetCounter(Object obj, int count, string assetName)
		{
			this.count = count;
			this.obj = obj;
			this.assetName = assetName;
		}
	}
}
