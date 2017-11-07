using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class AssetBundleNameSet
{

	//资源列表
	private HashSet<string> originRes = new HashSet<string>();

	//每个被引用资源的引用数量
	private Dictionary<string, int> dependence = new Dictionary<string, int>();

	//Bundle名称列表
	private HashSet<string> bundleName = new HashSet<string>();

	//
	private int resNumber = 0;

	private System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();



	public static void ClearAllBundleName(){
		string[] allAss = AssetDatabase.GetAllAssetBundleNames();
		foreach (var ass in allAss)
		{
			AssetDatabase.RemoveAssetBundleName(ass, true);
		}
		AssetDatabase.RemoveUnusedAssetBundleNames();
		Debug.Log("清除完成！");

	}

	public void BuildName(string[] buildAssetPath,bool isStaticAsset){
		AssetBundleNameSet.ClearAllBundleName();//清除


		System.DateTime dt = System.DateTime.Now;
		Debug.Log("clear time: " + (System.DateTime.Now - dt).TotalSeconds);

		bundleName.Clear();
		resNumber = 0;
		dependence.Clear();
		originRes.Clear();


		this.SetAssetBundleName(buildAssetPath);//Bundle命名
		this.SetSceneAssetBundleName();//场景Bundle命名
		if (isStaticAsset) {
			this.SetDirAssetBundleName (Application.dataPath + "/Resources/Lua/", "lua");//Lua的Bundle命名
		} else {
//			this.SetDirAssetBundleName (Application.dataPath + "/Client/DynamicResources/Empty/", "empty");
		}

		Debug.Log("<color=yellow>all asset number " + resNumber + "</color>");


		bundleName.Clear();
		resNumber = 0;
		dependence.Clear();
		originRes.Clear();

		AssetDatabase.RemoveUnusedAssetBundleNames();

		Debug.Log("set name done! time: " + (System.DateTime.Now - dt).TotalSeconds);

	}


	private void SetAssetBundleName(string[] path = null)
	{
		if (path != null && path.Length > 0)
		{
			SetAssetName(path);
		}
		else
		{
			Debug.LogError ("无资源打包");
		}	
	}

	private void SetSceneAssetBundleName()
	{

	}


	private void SetDirAssetBundleName(string dir,string assetBundleName){
		CycleFile(dir, (fileInfo, pathName) =>
			{
				AssetImporter ai = AssetImporter.GetAtPath(pathName);
				if (ai != null)
				{
					SetBundleNameSingle(ai, assetBundleName);
				}
				else
				{
					Debug.LogError("Asset import error :" + pathName);
				}
			});
	}




	/// <summary>
	/// 设置Path路径下的bundle名称（检测空格，重新命名，依赖命名检测）
	/// </summary>
	/// <param name="path"></param>
	private void SetAssetName(string[] paths)
	{
		foreach(var path in paths){
			string repath = Application.dataPath + "/" + path;

			CheckAssetNameValid(repath);
			SetAssetBundleNameAtDir(repath);
		}


		RenameAllDependence();
	}

	/// <summary>
	/// 重命名资源AssetBundle
	/// </summary>
	/// <param name="dir"></param>
	private  void SetAssetBundleNameAtDir(string dir)
	{
		CycleFile(dir, (fileInfo, pathName) =>
			{
				AssetImporter ai = AssetImporter.GetAtPath(pathName);
				if (ai != null)
				{
					string bundle_name = pathName.Replace("Assets/Client/Resources/", "");// Effect/New_Prefab.Prefab
					bundle_name = bundle_name.Replace("Assets/Client/DynamicResources/", "");// Effect/New_Prefab.Prefab
					bundle_name = bundle_name.Substring(0, bundle_name.LastIndexOf('.')); //    Effect/New_Prefab
					SetBundleNameSingle(ai, bundle_name);
				}
				else
				{
					Debug.LogError("Asset import error :" + pathName);
				}
			});
	}

	/// <summary>
	/// 设置Bundle名称
	/// </summary>
	/// <param name="ai"></param>
	/// <param name="name"></param>
	private  void SetBundleNameSingle(AssetImporter ai, string name)
	{
		if (name.IndexOf(' ') != -1)
		{
			Debug.LogError("发现空格文件：" + name);
		}
		else
		{
			name = name.ToLower().Replace(@"&&", "");
			if (!name.Contains(".ab"))
			{
				name += ".ab";
			}
			ai.assetBundleName = name;
			bundleName.Add(name);
		}

	}

	/// <summary>
	/// 给依赖资源设置Bundle名
	/// </summary>
	private  void RenameAllDependence()
	{
		Debug.Log("<color=yellow>依赖资源命名 all dependence number " + dependence.Count + "</color>");
		foreach (var kvp in dependence)
		{
			if (kvp.Value > 1 && !kvp.Key.EndsWith(".cs") && !kvp.Key.EndsWith(".js") && !kvp.Key.ToLower().EndsWith(".shader"))
			{
				string fin = kvp.Key;
				if (fin.IndexOf(' ') != -1)
				{
					Debug.LogError("文件名称有空格:" + kvp.Key);
				}
				else
				{
					AssetImporter ai = AssetImporter.GetAtPath(fin);
					if (originRes.Contains(kvp.Key))
					{
						Debug.LogWarning("Base resource be dependence " + kvp.Key);
					}
					else
					{
						string p = fin.Substring(7);
						p = p.Substring(0, p.LastIndexOf('.'));
						if (p.IndexOf(' ') != -1)
						{
							stringBuilder.AppendLine(p);
							Debug.LogError(p + "文件名称有空格");
						}
						SetBundleNameSingle(ai, p);
					}
				}
			}
		}
	}

	#region 检查Path下面的资源名字,检测空格
	/// <summary>
	/// 检查Path下面的资源名字,检测空格
	/// </summary>
	/// <param name="path"></param>
	private void CheckAssetNameValid(string path)
	{
		stringBuilder.AppendLine("<color=red>带有空格的资源文件:</color>");
		//检查资源
		CycleFile(path, (fileInfo, pathName) =>
			{
				if (fileInfo.Name.IndexOf(' ') != -1 && !fileInfo.Name.ToLower().EndsWith(".shader") && !fileInfo.Name.ToLower().EndsWith(".tpsheet"))
				{
					stringBuilder.AppendLine(pathName);
				}
				GetDependence(pathName);
			});

		//检查资源依赖项的空格
		//stringBuilder.AppendLine("<color=yellow>all dependence number: " + dependence.Count + "</color>");
		foreach (var kvp in dependence)
		{
			if (kvp.Value > 1 && !kvp.Key.EndsWith(".cs") && !kvp.Key.EndsWith(".js") && !kvp.Key.ToLower().EndsWith(".shader"))
			{
				string fin = kvp.Key;
				if (fin.IndexOf(' ') != -1)
				{
					stringBuilder.AppendLine(fin);
				}
			}
		}

		Debug.Log("资源依赖项的空格:" + stringBuilder.ToString());

		stringBuilder = new System.Text.StringBuilder();
		AssetDatabase.Refresh();
	}
	#endregion

	#region 循环遍历dir目录下的所有文件
	/// <summary>
	/// 循环遍历dir目录下的所有文件
	/// </summary>
	/// <param name="dir">目录</param>
	/// <param name="act">对于每一个文件的回调操作</param>
	 void CycleFile(string dir, System.Action<FileInfo, string> act)
	{
		DirectoryInfo di = new DirectoryInfo(dir);
//		string dd = di.FullName.Replace("\\", "/");

		//给当前目录下的Asset设Bundle名
		FileInfo[] fis = di.GetFiles();
		foreach (var fi in fis)
		{
			if (!fi.Name.EndsWith(".meta") && !fi.Name.EndsWith(".DS_Store") && !fi.Name.EndsWith(".tpsheet"))
			{
				//fi.Name = New_Prefab.Prefab
				//fi.FulName = G:\badperson\trunk\client\police\Assets\Resources\Effect\New_Prefab.Prefab
				string path_name = fi.FullName.Remove(0, Application.dataPath.Length);// \Resources\Effect\New_Prefab.Prefab
				path_name = path_name.Replace("\\", "/");// /Resources/Effect/New_Prefab.Prefab
				path_name = "Assets" + path_name;// Assets/Resources/Effect/New_Prefab.Prefab
				act(fi, path_name);
			}
		}

		//给当前目录下的子目录设Bundle名
		DirectoryInfo[] dirs = di.GetDirectories();
		foreach (DirectoryInfo dr in dirs)
		{
			if (dr.Name.StartsWith(".svn"))
			{
				continue;
			}
			CycleFile(dr.FullName, act);
		}
	}
	#endregion


	/// <summary>
	/// 获取依赖项
	/// </summary>
	/// <param name="pathName"></param>
	private  void GetDependence(string pathName)
	{
		originRes.Add(pathName);
		string[] ds = AssetDatabase.GetDependencies(pathName, true);
		foreach (var depend in ds)
		{
			if (dependence.ContainsKey(depend))
			{
				dependence[depend]++;
			}
			else
			{
				dependence[depend] = 1;
			}
		}
		++resNumber;
	}
}
