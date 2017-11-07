
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;





public enum ResUseMode{
	RESOURCE,
	PERSIS_STATIC_RESOURCE,
	PERSIS_DYNAMIC_RESOURCE,
}

public class ResourceManager:MonoBehaviour
{


	public AssetBundleManager staticAssetBundleManager = new AssetBundleManager();
	public AssetBundleManager dynamicAssetBundleManager = new AssetBundleManager ();
	public InnerResourceManager innerResoruceManager = new InnerResourceManager();



	/// <summary>
	/// 正在加载的资源列表
	/// </summary>
	Dictionary<string, Void_STR_UnityEngineObject> loadingCallbackList = new Dictionary<string, Void_STR_UnityEngineObject>();


	public static ResUseMode GetResourceUseMode(string resPath){
		//UnityEngine.UI.Image g = new UnityEngine.UI.Image;

		resPath = resPath.ToLower ();

		if (!resPath.EndsWith (".ab")) {
			resPath+=".ab";
		}

		if (AssetFileToolManager.updatedStaticAssetCells.ContainsKey (resPath)) {
			return ResUseMode.PERSIS_STATIC_RESOURCE;
		}

		if (AssetFileToolManager.currentDynamicAssetFileTool.assetCells.ContainsKey (resPath)) {
			return ResUseMode.PERSIS_DYNAMIC_RESOURCE;
		}

		return ResUseMode.RESOURCE;
	}

	/// <summary>
	/// 直接获取资源，不会改动引用计数器
	/// </summary>
	/// <param name="resourcePath"></param>
	/// <returns>资源</returns>
	public Object GetResource(string resourcePath)
	{

		ResUseMode mode = GetResourceUseMode (resourcePath);
		switch (mode) {
		case ResUseMode.RESOURCE:
			return innerResoruceManager.GetResource (resourcePath);
		case ResUseMode.PERSIS_STATIC_RESOURCE:
			{
				int lastSplitIndex = resourcePath.LastIndexOf ('/');
				string assetName = resourcePath.Substring (lastSplitIndex + 1);
				return staticAssetBundleManager.GetAssetBundleAsset (resourcePath+".ab",assetName);
			}
		case ResUseMode.PERSIS_DYNAMIC_RESOURCE:
			{
				int lastSplitIndex = resourcePath.LastIndexOf ('/');
				string assetName = resourcePath.Substring (lastSplitIndex + 1);
				return dynamicAssetBundleManager.GetAssetBundleAsset (resourcePath+".ab",assetName);
			}
		}
		return null;
	}

	/// <summary>
	/// 阻塞加载资源,会增加引用计数器，注意：会阻塞主线程，谨慎使用！！！
	/// </summary>
	/// <param name="resourcePath">资源路径</param>
	/// <returns>加载得到的资源</returns>
	public Object LoadResourceBlock(string resourcePath)
	{
		Object asset = null;
		ResUseMode useMode = GetResourceUseMode (resourcePath);
		if (useMode == ResUseMode.RESOURCE) {
			asset = this.innerResoruceManager.Load (resourcePath);
		} else if (useMode == ResUseMode.PERSIS_DYNAMIC_RESOURCE) {
			int lastSplitIndex = resourcePath.LastIndexOf ('/');
			string assetName = resourcePath.Substring (lastSplitIndex + 1);
			asset = this.dynamicAssetBundleManager.LoadObjectFormAssetBundle ( resourcePath + ".ab", assetName);
		} else {
			int lastSplitIndex = resourcePath.LastIndexOf ('/');
			string assetName = resourcePath.Substring (lastSplitIndex + 1);
			asset = this.staticAssetBundleManager.LoadObjectFormAssetBundle ( resourcePath + ".ab", assetName);
		}

		return asset;
	}

	/// <summary>
	/// 异步加载资源,addRefCount： 是否缓存并增加引用计数器
	/// </summary>
	/// <param name="resourcePath"></param>
	public void LoadResourceAsync(string resourcePath, Void_STR_UnityEngineObject asyncCallBack = null)
	{

		if (loadingCallbackList.ContainsKey(resourcePath))
		{
			loadingCallbackList[resourcePath] += asyncCallBack;
		}
		else
		{
			loadingCallbackList.Add(resourcePath, null);
			loadingCallbackList[resourcePath] += asyncCallBack;
			LoadResourceAsync0 (resourcePath);
		}
	}

	private void LoadResourceAsync0(string resourcePath)
	{
		ResUseMode useMode = GetResourceUseMode (resourcePath);
		switch (useMode) {
		case ResUseMode.RESOURCE:
			this.innerResoruceManager.LoadAsync (resourcePath, (_resourcePath, _asset) => {
				HandlerAsyncCallBack(resourcePath,_asset);
			});
			break;
		case ResUseMode.PERSIS_STATIC_RESOURCE:
			{

				int lastSplitIndex = resourcePath.LastIndexOf ('/');
				string assetName = resourcePath.Substring (lastSplitIndex + 1);
				this.staticAssetBundleManager.LoadObjectFormAssetBundleAsync (resourcePath + ".ab", assetName
					,(_filePath, _assetName,_asset)=>{
						HandlerAsyncCallBack(resourcePath,_asset);
					});
			}
			break;
		case ResUseMode.PERSIS_DYNAMIC_RESOURCE:
			{
				int lastSplitIndex = resourcePath.LastIndexOf ('/');
				string assetName = resourcePath.Substring (lastSplitIndex + 1);
				this.dynamicAssetBundleManager.LoadObjectFormAssetBundleAsync (resourcePath + ".ab", assetName
					,(_filePath, _assetName,_asset)=>{
						HandlerAsyncCallBack(resourcePath,_asset);
					});

			}
			break;
		}


	}

	private void HandlerAsyncCallBack(string resourcePath, Object asset){

		if (!loadingCallbackList.ContainsKey(resourcePath))
		{
			Debug.LogError ("loadingCallbackList:"+resourcePath+" had be removed");
		}

		if (asset == null) {
			Debug.LogError ("equest.asset null:" + resourcePath);
		}


		Void_STR_UnityEngineObject asyncCallBack = loadingCallbackList[resourcePath];

		loadingCallbackList.Remove(resourcePath);

		if (asyncCallBack != null)
		{
			asyncCallBack(resourcePath, asset);
		}
	}

	/// <summary>
	/// 卸载资源, 引用计数为零将其从内存中清除
	/// </summary>
	/// <param name="resourcePath">资源路径</param>
	public void UnloadResource(string resourcePath)
	{
		ResUseMode useMode = GetResourceUseMode (resourcePath);
		switch (useMode) {
		case ResUseMode.RESOURCE:
			innerResoruceManager.UnloadResource (resourcePath);
			break;
		case ResUseMode.PERSIS_STATIC_RESOURCE:
			{
				int lastSplitIndex = resourcePath.LastIndexOf ('/');
				string assetName = resourcePath.Substring (lastSplitIndex + 1);
				staticAssetBundleManager.UnLoadObject (resourcePath+ ".ab", assetName);
			}
			break;
		case ResUseMode.PERSIS_DYNAMIC_RESOURCE:
			{
				int lastSplitIndex = resourcePath.LastIndexOf ('/');
				string assetName = resourcePath.Substring (lastSplitIndex + 1);
				dynamicAssetBundleManager.UnLoadObject (resourcePath+ ".ab", assetName);
			}
			break;
		}
	}


	private static ResourceManager instance;
	public static ResourceManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<ResourceManager>();
				if (instance == null)
				{
					GameObject obj = new GameObject("ResourceManager");
					GameObject.DontDestroyOnLoad(obj);
					instance = obj.AddComponent<ResourceManager>();
				}
			}
			return instance;
		}
	}


}

