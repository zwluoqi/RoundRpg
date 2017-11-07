using System.Collections.Generic;
using UnityEngine;
using System.Collections;



public class InnerResourceManager
{
	/// <summary>
	/// 资源计数器
	/// </summary>
	public class ObjectCounter
	{
		public object resource;
		public int count = 1;
		public ObjectCounter(object resource, int counter = 1)
		{
			this.resource = resource;
			this.count = counter;
		}
	}

	/// <summary>
	/// 已经加载的资源列表
	/// </summary>
	public Dictionary<string, ObjectCounter> assetContainer = new Dictionary<string, ObjectCounter>();


	public Object GetResource(string resourcePath){
		resourcePath = resourcePath.Replace (".ab","");

		if (assetContainer.ContainsKey(resourcePath))
		{
			return assetContainer[resourcePath].resource as Object;
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// 阻塞加载资源,会增加引用计数器，注意：会阻塞主线程，谨慎使用！！！
	/// </summary>
	/// <param name="resourcePath">资源路径</param>
	/// <returns>加载得到的资源</returns>
	public Object Load(string resourcePath)
	{
		resourcePath = resourcePath.Replace (".ab","");
		Object asset = null;
		if (assetContainer.ContainsKey(resourcePath))
		{
			assetContainer[resourcePath].count++;
			asset = assetContainer[resourcePath].resource as Object;
		}
		else
		{
			asset = Resources.Load (resourcePath);

			if (asset != null)
			{
				assetContainer.Add(resourcePath, new ObjectCounter(asset));
			}
			else
			{
				Debug.LogError(resourcePath + "\t file not exist! LoadResourceBlock()");
			}
		}

		return asset;
	}

	public Object[] LoadAll(string resourcePath){
		resourcePath = resourcePath.Replace (".ab","");
		Object[] asset = null;
		if (assetContainer.ContainsKey(resourcePath))
		{
			assetContainer[resourcePath].count++;
			asset = assetContainer[resourcePath].resource as Object[];
		}
		else
		{
			asset = Resources.LoadAll (resourcePath);

			if (asset != null)
			{
				assetContainer.Add(resourcePath, new ObjectCounter(asset as object));
			}
			else
			{
				Debug.LogError(resourcePath + "\t file not exist! LoadResourceBlock()");
			}
		}

		return asset;
	}

	/// <summary>
	/// 异步加载资源,addRefCount： 是否缓存并增加引用计数器
	/// </summary>
	/// <param name="resourcePath"></param>
	public void LoadAsync(string resourcePath, System.Action<string,Object> act)
	{
		resourcePath = resourcePath.Replace (".ab","");
		if (assetContainer.ContainsKey(resourcePath))
		{
			//如果已缓存，直接回调返回
			assetContainer[resourcePath].count++;
			act(resourcePath, assetContainer[resourcePath].resource as Object);
		}
		else
		{
			ResourceManager.Instance.StartCoroutine(LoadResourceAsync0 (resourcePath,act));
		}
	}

	private IEnumerator LoadResourceAsync0(string resourcePath, System.Action<string,Object> act)
	{
		ResourceRequest request = Resources.LoadAsync(resourcePath);
		yield return request;
		assetContainer[resourcePath] = new ObjectCounter(request.asset, 1);
		act (resourcePath, request.asset);
	}


	/// <summary>
	/// 卸载资源, 引用计数为零将其从内存中清除
	/// </summary>
	/// <param name="resourcePath">资源路径</param>
	public void UnloadResource(string resourcePath)
	{
		if (assetContainer.ContainsKey(resourcePath))
		{
			ObjectCounter oc = assetContainer[resourcePath];
			int counter = --oc.count;
			if (counter == 0)
			{
				assetContainer.Remove(resourcePath);
				if (oc.resource != null && (oc.resource is GameObject) == false)
				{
					Resources.UnloadAsset(oc.resource as Object);
				}
			}
		}
	}


	//手到调用UnloadUnusedResources和GC
	public void UnloadUnusedAndGC()
	{
		GC();
		ResourceManager.Instance.StartCoroutine(UnloadUnusedResources0());
	}

	//手到调用GC
	public void GC()
	{
		float t = Time.realtimeSinceStartup;
		System.GC.Collect();
		Debug.Log("ResourceManager.GC(), cost time: " + (Time.realtimeSinceStartup - t));
	}

	/// <summary>
	/// 清除无用资源，卸载内存
	/// </summary>
	public void UnloadUnusedResources()
	{
		ResourceManager.Instance.StartCoroutine(UnloadUnusedResources0());
	}
	private IEnumerator UnloadUnusedResources0()
	{
		float t = Time.realtimeSinceStartup;
		yield return Resources.UnloadUnusedAssets();
		Debug.Log("ResourceManager.UnloadUnusedResources(), cost time: " + (Time.realtimeSinceStartup - t));
	}


}

