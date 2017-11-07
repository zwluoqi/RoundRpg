using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;
using System.Text;
using UnityEngine;
using System.Diagnostics;

public class Totem_H1 : EditorWindow {




	[MenuItem("Totem_H1/提取所有角色动画为AnimationClip", false, 0)]
	public static void Fbx2AnimtionClip()
	{
		PickAnim.PickAllAnimations ("Client/Res/Models");
	}

	[MenuItem("Totem_H1/生成所有动画时长为Excle", false, 0)]
	public static void Anim2Excle()
	{
		ModelAnim2ExcleTool model = new ModelAnim2ExcleTool ();
		model.Build ("Client/Resources/Models");
	}

	[MenuItem("Totem_H1/excle 转化为 lua", false, 0)]
	public static void Excle2Lua()
	{

		#if UNITY_EDITOR_OSX
		string command = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal"; 
		string cd_shell = "cd "+Application.dataPath + "/../../tools/xls2lua";


		string shell = "./run.sh";
		string arg1 = "";
		string arg2 =  "";
		string argss =  shell +" "+ arg1 +" " + arg2;
		//有人说把空格  和 全部 用冒号 括起来， 但是还是没能成功。
		//string argss =  "\"" + shell +" "+ arg1 +" " + arg2 +"\"";
		//string argss =  shell +"\" \""+ arg1 +"\" \""+ arg2;
		//string argss =  "\"" + shell +"\" \""+ arg1 +"\" \""+ arg2+"\"";
		System.Diagnostics.Process.Start(command,cd_shell+" \t\r\n "+argss);
		UnityEngine.Debug.Log(argss);
		#else
		string dirName = Application.dataPath + "/../../tools/xls2lua/";
		ProcessStartInfo info = new ProcessStartInfo();
		info.FileName = dirName + "/run.bat";
		//info.Arguments = "";
		info.WindowStyle = ProcessWindowStyle.Hidden;
		info.UseShellExecute = true;
		info.WorkingDirectory = dirName;
		info.ErrorDialog = true;
		UnityEngine.Debug.Log(info.WorkingDirectory);
		Process pro = Process.Start(info);
		pro.WaitForExit();
		#endif
	}

	[MenuItem("Totem_H1/一键提取、生成excle、lua动画文件", false, 0)]
	public static void Anim2ExcleOneKey()
	{
		Fbx2AnimtionClip ();

		Anim2Excle ();

		Excle2Lua ();
	}


	[MenuItem("Totem_H1/协议生成目录", false, 0)]
	public static void PB2Lua()
	{

		#if UNITY_EDITOR_OSX
		string command = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal"; 
		string cd_shell = "cd "+Application.dataPath + "/../../tools/protoc-gen-lua/plugin_mac/";


		string shell = "./run.sh";
		string arg1 = "";
		string arg2 =  "";
		string argss =  shell +" "+ arg1 +" " + arg2;
		//有人说把空格  和 全部 用冒号 括起来， 但是还是没能成功。
		//string argss =  "\"" + shell +" "+ arg1 +" " + arg2 +"\"";
		//string argss =  shell +"\" \""+ arg1 +"\" \""+ arg2;
		//string argss =  "\"" + shell +"\" \""+ arg1 +"\" \""+ arg2+"\"";
		System.Diagnostics.Process.Start(command,cd_shell+" \t\r\n "+argss);
		UnityEngine.Debug.Log(argss);
		#else
		string dirName = Application.dataPath + "/../../tools/xls2lua/";
		ProcessStartInfo info = new ProcessStartInfo();
		info.FileName = dirName + "/run.bat";
		//info.Arguments = "";
		info.WindowStyle = ProcessWindowStyle.Hidden;
		info.UseShellExecute = true;
		info.WorkingDirectory = dirName;
		info.ErrorDialog = true;
		UnityEngine.Debug.Log(info.WorkingDirectory);
		Process pro = Process.Start(info);
		pro.WaitForExit();
		#endif
	}


	[MenuItem ("Totem_H1/AtlasMaker")]
	static private void MakeAtlas()
	{


		DirectoryInfo rootDirInfo = new DirectoryInfo (Application.dataPath +"/Client/Res/Atlas");
		foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories()) {
			BuildOnceUGUIAtlas (dirInfo);
		}	
	}


	private static void BuildOnceUGUIAtlas(DirectoryInfo dirInfo)
	{
		string spriteDir = Application.dataPath +"/Client/Resources/Atlas";

		if(!Directory.Exists(spriteDir)){
			Directory.CreateDirectory(spriteDir);
		}

		string atlasName = dirInfo.Name;
		string prefabFullPath = spriteDir+"/"+atlasName+".prefab";
		string prefabPath = prefabFullPath.Substring(prefabFullPath.IndexOf("Assets"));


		GameObject go = new GameObject(atlasName);
		UGUIAtlas atlas = go.AddComponent<UGUIAtlas> ();


		foreach (FileInfo pngFile in dirInfo.GetFiles("*.png",SearchOption.AllDirectories)) {
			string allPath = pngFile.FullName;
			string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
			Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
			atlas.AddSprite (sprite);
		}
		PrefabUtility.CreatePrefab(prefabPath,go);
		GameObject.DestroyImmediate(go);
	}


	[MenuItem ("Assets/AtlasMaker",true)]
	static private bool AssetMenuMakeAtlas()
	{
		if (Selection.activeObject == null) {
			return false;
		}
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
//		string dir = Path.GetFullPath (path);
//		string dirRelativePath = Path.GetDirectoryName (path);

		return path.Contains("Res/Atlas");
	}

	[MenuItem ("Assets/AtlasMaker",false)]
	static private void AssetMenuMakeAtlas0()
	{
//		UnityEngine.Debug.LogWarning (Selection.activeObject.name);

		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		string dir = Path.GetFullPath (path);
		string dirRelativePath = Path.GetDirectoryName (path);

		int slash = Mathf.Max(dirRelativePath.LastIndexOf('/'), dirRelativePath.LastIndexOf('\\'));

		string dirName = dirRelativePath.Substring (slash+1);

		UnityEngine.Debug.LogWarning (dir);
		UnityEngine.Debug.LogWarning (dirRelativePath);
		UnityEngine.Debug.LogWarning (dirName);

		DirectoryInfo dirInfo = new DirectoryInfo (dirRelativePath);
		BuildOnceUGUIAtlas (dirInfo);

	}}
