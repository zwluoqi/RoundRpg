using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class AssetFileToolManager
{
//	public static string staticMainFestPath {get{return PathTool.CurrentVersionStaticResDataPath + "/" + AssetFileTool.staticMainFest;}}
//	public static string dynamicMainFestPath {get{return PathTool.CurrentVersionDynamicResDataPath + "/" + AssetFileTool.dynamicMainFest;}}
//
//	public static string staticAssetFileToolPath { get { return PathTool.CurrentVersionStaticResDataPath + "/" + AssetFileTool.staticAssetFileTool; } }
//	public static string dynamicAssetFileToolPath { get { return PathTool.CurrentVersionDynamicResDataPath + "/" + AssetFileTool.dynamicAssetFileTool; } }
//
//
//
//	public static AssetFileTool innerStaticAssetFileTool;
//	public static AssetFileTool innerDynamicAssetFileTool;
//
//	public static AssetFileTool currentStaticAssetFileTool;
//	public static AssetFileTool currentDynamicAssetFileTool;
//
//	public static Dictionary<string,AssetCell> updatedStaticAssetCells;
//
//
//	//更新最新的需要从外部读取的静态资源列表
//	public static void UpdateStaticAssets(){
//		updatedStaticAssetCells = new Dictionary<string, AssetCell> ();
//
//		if (currentStaticAssetFileTool.version <= innerStaticAssetFileTool.version) {
//			return;
//		}
//		List<AssetCell> unneceList;
//		List<AssetCell> lackList;
//		List<AssetCell> obbList;
//		AssetFileTool.Compare(innerStaticAssetFileTool, currentStaticAssetFileTool, out unneceList, out lackList, out obbList);
//		foreach (var ac in lackList) {
//			updatedStaticAssetCells [ac.path] = ac;
//		}
//		foreach (var ac in obbList) {
//			updatedStaticAssetCells [ac.path] = ac;
//		}
//
//		if (updatedStaticAssetCells.Count > 0) {
//			 StringBuilder mSb = new StringBuilder();
//			mSb.AppendLine ("有一些Resources资源要从外部获取：" + updatedStaticAssetCells.Count);
//			foreach (var data in updatedStaticAssetCells) {
//				mSb.AppendLine ("需要从外部获取的Resource资源："+data.Value.path);
//			}
//			Debug.LogWarning (mSb.ToString());
//			mSb = null;
//		}
//	}
//
//	//更新完成后读取最新的文件列表
//	public static void ReadCurrentAssetFile(){
//
//		currentStaticAssetFileTool = AssetFileTool.LoadFromFile (PathTool.CurrentVersionStaticResDataPath + "/" + AssetFileTool.staticAssetFileTool);
//		currentDynamicAssetFileTool = AssetFileTool.LoadFromFile (PathTool.CurrentVersionDynamicResDataPath + "/" + AssetFileTool.dynamicAssetFileTool);
//	}
//
//	//开始游戏后将文件列表存入Pers中
//	public static void WriteInnerAssetFileToolAndMainfestFlie(){
//
//
//		TextAsset text;
//		if (!File.Exists (staticMainFestPath)) {
//			text = Resources.Load<TextAsset> ("BundleList/"+PathTool. GetBundleTargetName()+"/" + AssetFileTool.staticMainFest);
//			FileUtils.WriteAllBytes (staticMainFestPath, text.bytes);
//			Resources.UnloadAsset (text);
//		}
//
//
//		text = Resources.Load<TextAsset> ("BundleList/"+PathTool. GetBundleTargetName()+"/" + AssetFileTool.staticAssetFileTool);
//		innerStaticAssetFileTool = AssetFileTool.LoadFromString (text.text);
//		if (!File.Exists (staticAssetFileToolPath)) {
//			FileUtils.WriteAllBytes (staticAssetFileToolPath,text.bytes);
//		}
//		Resources.UnloadAsset (text);
//
//
//		if (!File.Exists (dynamicMainFestPath)) {
//			text = Resources.Load<TextAsset> ("BundleList/"+PathTool. GetBundleTargetName()+"/"+ AssetFileTool.dynamicMainFest);
//			FileUtils.WriteAllBytes (dynamicMainFestPath,text.bytes);
//			Resources.UnloadAsset (text);
//		}
//
//		text = Resources.Load<TextAsset> ("BundleList/"+PathTool. GetBundleTargetName()+"/" + AssetFileTool.dynamicAssetFileTool);
//		innerDynamicAssetFileTool= AssetFileTool.LoadFromString (text.text);
//		if (!File.Exists (dynamicAssetFileToolPath)) {
//			FileUtils.WriteAllBytes (dynamicAssetFileToolPath,text.bytes);
//		}
//		Resources.UnloadAsset (text);
//
//	}
}
