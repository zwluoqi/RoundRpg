using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;
using System.Text;
using UnityEngine;

public class BundleManagerWindow : EditorWindow
{

	public int dynamicResVersion;
	public int DynamicResVersion
	{
		get
		{
			return EditorPrefs.GetInt("DynamicResVersion");
		}
		set
		{
			EditorPrefs.SetInt("DynamicResVersion", value);
		}
	}

	private int staticResVersion;
	private int StaticResVersion
	{
		get
		{
			return EditorPrefs.GetInt("StaticResVersion");
		}
		set
		{
			EditorPrefs.SetInt("StaticResVersion", value);
		}
	}

	private string appVersion;



	[MenuItem("Totem_H1/BundleManager Windows", false, 50)]
	public static void Init()
	{
		BundleManagerWindow window = (BundleManagerWindow)EditorWindow.GetWindow(typeof(BundleManagerWindow));
		window.Show();
		window.dynamicResVersion = window.DynamicResVersion;

	}

	public AssetBundleNameSet assetBundleNameSet;
	public string assetPath = "";
	public bool isStaticAsset = true;
	public bool buildEmptyDynamicAssets = false;
	public bool buildEmptyStaticAssets = false;

	void OnGUI()
	{


		GUILayout.Label ("目标平台: " + BundleTools.GetTargetPlatform (), EditorStyles.boldLabel, GUILayout.Height (20));
		GUILayout.Label ("AppVersion: " + VersionTool.packageVersion, EditorStyles.boldLabel, GUILayout.Height (20));
		GUILayout.Label ("StaticResVersion: " + StaticResVersion, EditorStyles.boldLabel, GUILayout.Height (20));
		string rv = EditorGUILayout.TextField(StaticResVersion.ToString(), GUILayout.Width(30));
		int irv = -1;
		if (int.TryParse(rv, out irv))
		{
			StaticResVersion = irv;
		}

		GUILayout.Label ("DynamicResVersion: " + DynamicResVersion, EditorStyles.boldLabel, GUILayout.Height (20));
		rv = EditorGUILayout.TextField(DynamicResVersion.ToString(), GUILayout.Width(30));
		irv = -1;
		if (int.TryParse(rv, out irv))
		{
			DynamicResVersion = irv;
		}

		GUILayout.Label ("当前Bundle数量: " + AssetDatabase.GetAllAssetBundleNames ().Length, EditorStyles.boldLabel, GUILayout.Height (20));
		assetPath = GUILayout.TextArea (assetPath);
		isStaticAsset = GUILayout.Toggle (isStaticAsset, "静态资源(对应生成静态或动态资源的mainfest文件)");
		buildEmptyDynamicAssets = DynamicResVersion == 0;
		buildEmptyStaticAssets = StaticResVersion == 0;
		#region 打包流程


		GUILayout.Space(10);
		if (GUILayout.Button("工具——清除Bundle命名"))
		{
			AssetBundleNameSet.ClearAllBundleName();//清除
		}

		GUILayout.Space(10);
		if (GUILayout.Button("打包流程0=>copy lua文件"))
		{
			CopyLuaFiile();
		}

		GUILayout.Space(10);
		if (GUILayout.Button("打包流程①=>Bundle命名"))
		{
			AssetBundleName();
		}

		GUILayout.Space(10);
		if (GUILayout.Button("打包流程②=>生成assetbundle"))
		{
			BuildAssetBundle(true);
		}

		GUILayout.Space(10);
		if (GUILayout.Button("打包流程③=>生成资源列表文件"))
		{
			CreateManifest();
		}

		GUILayout.Space(10);

		if (GUILayout.Button("打包流程④=>拷贝资源列表到工程"))
		{
			CopyAssetmainifestListToProject();
		}

		GUILayout.Space(10);
		GUILayout.Space(10);
		GUILayout.Space(10);

		#endregion


		#region 一键生成资源包不COPY资源列表


		if (GUILayout.Button("一键生成资源包不COPY资源列表(主要用于在本地测试Static资源是否能正常下载使用)"))
		{
			if(isStaticAsset){
				CopyLuaFiile();
			}
			AssetBundleName();
			BuildAssetBundle(true);
			CreateManifest();
			//CopyAssetmainifestListToProject();
		}

		#endregion

		#region 一键生成资源包


		if (GUILayout.Button("一键生成Bundle资源"))
		{
			if(isStaticAsset){
				CopyLuaFiile();
			}
			AssetBundleName();
			BuildAssetBundle(true);
			CreateManifest();
			CopyAssetmainifestListToProject();
		}

		#endregion
	}

	public void CopyLuaFiile(){
		LuaBundleBuilder.CopyLuaFilesToRes ();
	}

	public void AssetBundleName(){
		Debug.LogWarning ("AssetBundleName------------------------------------");

		string[] buildAssetPath = new string[1];
		if (isStaticAsset) {
			if (buildEmptyStaticAssets) {
				buildAssetPath [0] = "Client/Resources/Empty/";
			} else {
				buildAssetPath [0] = "Client/Resources/";
			}
		} else {
			if (buildEmptyDynamicAssets) {
				buildAssetPath [0] = "Client/DynamicResources/Empty/";
			} else {
				buildAssetPath [0] = "Client/DynamicResources/";
			}
		}
		assetBundleNameSet = new AssetBundleNameSet ();
		assetBundleNameSet.BuildName (buildAssetPath,isStaticAsset);

		AssetDatabase.RemoveUnusedAssetBundleNames();

		AssetDatabase.Refresh();
		Debug.LogWarning("Bundle命名完成！-----------------------------------");
	}

	public AssetFileTool localManifest;
	public void BuildAssetBundle(bool clearOldBundle ){
		Debug.LogWarning ("BuildAssetBundle-----------------------------------");

		//*****************读取以前列表文件信息********************
		string manifest = BundleTools.GetBuildBundlePath (isStaticAsset) + "/" + assetFileTool;
		localManifest = AssetFileTool.LoadFromFile(manifest);


		BundleTools.CreateBuildBundleDir(isStaticAsset);
		if (clearOldBundle)
		{
			BundleTools.ClearBuildBundleDir(isStaticAsset);
		}

		BuildTarget platform = EditorUserBuildSettings.activeBuildTarget;
		BuildAssetBundleOptions babo = BuildAssetBundleOptions.ChunkBasedCompression;

		System.DateTime dt = System.DateTime.Now;
		string targetPath = BundleTools.GetBuildBundlePath(isStaticAsset);

		BuildPipeline.BuildAssetBundles(targetPath, babo, platform);
		Debug.Log("Build bundle cost time: " + (DateTime.Now - dt).TotalSeconds + "s");

		AssetBundleNameSet.ClearAllBundleName();//清除

		AssetDatabase.Refresh();

	}

	public void CreateManifest(){
		Debug.LogWarning ("CreateManifest-----------------------------------");
		EditorApplication.delayCall += BuildManifest;//生成ALL资源列表文件
//		EditorApplication.delayCall += CreateBasicPackage;//生成基础包和扩展包资源列表文件
	}

	public void CopyAssetmainifestListToProject(){
		Debug.LogWarning ("CopyAssetmainifestListToProject-----------------------------------");

		EditorApplication.delayCall += CopyAssetmainifestListToProjectDelay;

	}


	public void CalculateBundleInfo(){

	}



	private void CopyAssetmainifestListToProjectDelay(){
		if (buildEmptyDynamicAssets || isStaticAsset) {
			CopyFileToProject (assetFileTool, assetFileTool + ".txt",true,false);
			CopyFileToProject (mainfestName, mainfestName + ".txt",true,false);
		}
	}

	private void CopyFileToProject(string sourceName,string targetName,bool resourceDir,bool streamAssets){

		//获取资源列表
		string path = BundleTools.GetBuildBundlePath (isStaticAsset) + "/" + sourceName;

		if (streamAssets) {
			Debug.Log ("###############    拷贝资源列表到工程StreamingAssets目录    ###############");

			string streamingAssetManiPath = Application.streamingAssetsPath + "/"+BundleTools.GetABManefistName()+"/";

			//创建StreamingAssets下的文件夹
			if (!Directory.Exists (streamingAssetManiPath)) {
				Directory.CreateDirectory (streamingAssetManiPath);
			}


			//拷贝assetManifest文件到streamingAsset
			if (File.Exists (streamingAssetManiPath + targetName)) {
				File.Delete (streamingAssetManiPath + targetName);
			}
			File.Copy (path, streamingAssetManiPath + targetName, true);
		}

		if (resourceDir) {

			//拷贝assetManifest文件到Resources
			string resourceAssetMainPath = Application.dataPath + "/Resources/BundleList/"+BundleTools.GetABManefistName()+"/";
			//创建StreamingAssets下的文件夹
			if (!Directory.Exists (resourceAssetMainPath)) {
				Directory.CreateDirectory (resourceAssetMainPath);
			}

			if (File.Exists (resourceAssetMainPath + targetName)) {
				File.Delete (resourceAssetMainPath + targetName);
			}
			File.Copy (path, resourceAssetMainPath + targetName, true);
		}
		AssetDatabase.Refresh ();
	}
		
	public string assetFileTool{
		get{
			return (isStaticAsset ? AssetFileTool.staticAssetFileTool : AssetFileTool.dynamicAssetFileTool);
		}
	}

	public string mainfestName{
		get{
			return (isStaticAsset ? AssetFileTool.staticMainFest : AssetFileTool.dynamicMainFest);
		}
	}

	//生成主资源列表assetmanifest
	public void BuildManifest()
	{
		string mainPath = BundleTools.GetBuildBundlePath(isStaticAsset) + "/" +mainfestName;
		AssetBundle ab = AssetBundle.LoadFromFile(mainPath);
		if (ab == null)
		{
			Debug.LogError("ab is null!");
			return;
		}
		AssetCell mac = new AssetCell();
		FileInfo fi = new FileInfo(mainPath);
		mac.path = mainfestName;
		mac.size = fi.Length;
		mac.hashCode = BundleTools.GetMD5HashFromFile(mainPath);
		mac.loadLevel = 0;

		AssetBundleManifest abm = ab.LoadAsset("assetbundlemanifest") as AssetBundleManifest;
		string[] des = abm.GetAllDependencies ("ui/page/main_ui.ab");
		foreach (var de in des) {
			Debug.LogWarning ("de:"+de);
		}
		ab.Unload(false);




		//即将生成的列表
		int versionNum = 0;
		if (isStaticAsset) {
			versionNum = StaticResVersion;
		} else if (buildEmptyDynamicAssets) {
			versionNum = 0;
		} else {
			versionNum = DynamicResVersion;
		}
		AssetFileTool aft = AssetFileTool.CreateAssetFileByManifest(abm, versionNum, BundleTools.GetBuildBundlePath(isStaticAsset) + "/", mac);

		List<AssetCell> unneceList;
		List<AssetCell> lackList;
		List<AssetCell> obbList;
		AssetFileTool.Compare(localManifest, aft, out unneceList, out lackList, out obbList);

		if (unneceList.Count != 0)
		{
			Debug.LogWarning("旧包中无用的bandle数据为:" + unneceList.Count);
			foreach (var cell in unneceList) {
				Debug.LogWarning (cell.path);
			}
		}

		if (lackList.Count != 0)
		{
			Debug.LogWarning("新增的bandle数据为:" + lackList.Count);
			foreach (var cell in lackList) {
				Debug.LogWarning (cell.path);
			}
		}

		if (obbList.Count != 0)
		{
			Debug.LogWarning("数据包需要更新的的bandle数据为:" + obbList.Count);
			foreach (var cell in obbList) {
				Debug.LogWarning (cell.path);
			}
		}

		string manifest = BundleTools.GetBuildBundlePath (isStaticAsset) + "/" + assetFileTool;

		if (File.Exists(manifest))
		{
			File.Delete(manifest);
		}

		aft.SaveToFile(manifest);

		Debug.Log("生成资源列表文件已完成! " + assetFileTool);
	}


}

