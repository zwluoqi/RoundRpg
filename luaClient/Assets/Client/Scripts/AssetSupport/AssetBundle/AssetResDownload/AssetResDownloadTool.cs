//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;
//
//public class AssetResDownloadTool
//{
//	public string localSavePath;
//	public string localTmpSavePath;
//	public string remoteServerPath;
//	public string assetFileToolName;
//
//	public AssetFileTool remoteAssetFileTool;
//
//	public long totalSize;
//	public int totalDownloadCount;
//	protected List<AssetCell> preDownLoadList = new List<AssetCell> ();
//	protected List<AssetCell> errorList = new List<AssetCell>();
//	protected List<AssetCell> downLoadList = new List<AssetCell>();
//	protected List<AssetResSignalDownloadTool> sigDownLoadList = new List<AssetResSignalDownloadTool> ();
//
//
//
//	public static List<AssetResDownloadTool> resDownLoadTools = new List<AssetResDownloadTool>();
//
//	public static void AddPreLoadAssetDownLoadTool(AssetResDownloadTool ardt){
//		resDownLoadTools.Add (ardt);
//	}
//	public static bool NeedDownLoadAsset(){
//		return resDownLoadTools.Count > 0;
//	}
//
//
//
//
//
//	public AssetResDownloadTool Init(string _localSavePath,
//		string _localTmpSavePath,
//		string _remoteServerPath,
//		string _assetFileToolName ){
//		localSavePath = _localSavePath;
//		localTmpSavePath = _localTmpSavePath;
//		remoteServerPath = _remoteServerPath;
//		assetFileToolName = _assetFileToolName;
//
//		return this;
//	}
//
//	public void StartPreDownLoad(){
//		downLoadState = DownLoadState.PREDOWNLOADING;
//		ResourceManager.Instance.StartCoroutine (DownloadRemoteAssetFile ());
//	}
//
//	private const int MAX_DOWNLOADING = 4;
//
//
//	public void Tick(){
////		Debug.Log ("downLoadState:" + downLoadState);
//		switch (downLoadState) {
//		case DownLoadState.PREERROR:
//			
//			break;
//		case DownLoadState.PREDOWNLOADING:
//			
//			break;
//
//		case DownLoadState.DOWNLOADING:
//			if (downLoadList.Count < MAX_DOWNLOADING) {
//				DownloadPackage ();
//			}
//			List<AssetResSignalDownloadTool> tmps = new List<AssetResSignalDownloadTool> ();
//			tmps.AddRange (sigDownLoadList);
//			foreach (var sig in tmps) {
//				sig.Tick ();
//				CheckDownloadPackageOver (sig);
//			}
//			if (CheckAllAssetPackageDownLoadOver ()) {
//				downLoadState = DownLoadState.SUCCESS_DOWNLOAD;
//			}
//			break;
//		case DownLoadState.SUCCESS_DOWNLOAD:
//			SuccessHandler ();
//			break;
//		case DownLoadState.COMPLETE:
//
//			break;
//		}
//	}
//
//	protected virtual void SuccessHandler(){
//		remoteAssetFileTool.SaveToFile (localSavePath + "/" + assetFileToolName);
//
//		downLoadState = DownLoadState.COMPLETE;
//
//	}
//
//	private bool CheckAllAssetPackageDownLoadOver(){
//		if (preDownLoadList.Count > 0) {
//			return false;
//		}
//		if (downLoadList.Count > 0) {
//			return false;
//		}
//		if (errorList.Count > 0) {
//			return false;
//		}
//
//		return true;
//	}
//
//	private void DownloadPackage(){
//		if (preDownLoadList.Count > 0)
//		{
//			AssetCell ac = preDownLoadList[0];
//			preDownLoadList.RemoveAt(0);
//			downLoadList.Add(ac);
//			AssetResSignalDownloadTool asdt = new AssetResSignalDownloadTool (this, ac);
//			sigDownLoadList.Add(asdt);
//			asdt.StartHttpDownLoad ();
//		}
//		else if (errorList.Count > 0)
//		{
//			AssetCell ac = errorList[0];
//			errorList.RemoveAt(0);
//			downLoadList.Add(ac);
//			AssetResSignalDownloadTool asdt = new AssetResSignalDownloadTool (this, ac);
//			sigDownLoadList.Add(asdt);
//			asdt.StartHttpDownLoad ();
//
//		}
//	}
//
//	private void CheckDownloadPackageOver(AssetResSignalDownloadTool sig){
//		switch (sig.downloadState) {
//		case AssetResSignalDownloadTool.DownLoadState.DOWNLOADING:
//
//			break;
//		case AssetResSignalDownloadTool.DownLoadState.FAILED:
//		case AssetResSignalDownloadTool.DownLoadState.TIMEOUT:
//			downLoadList.Remove (sig.ac);
//			errorList.Add (sig.ac);
//			sigDownLoadList.Remove (sig);
//			break;
//		case AssetResSignalDownloadTool.DownLoadState.SUCESS:
//			sig.MoveCoverage ();
//			downLoadList.Remove (sig.ac);
//			sigDownLoadList.Remove (sig);
//
//			break;
//		}
//	}
//
//
//
//
//	public DownLoadState downLoadState;
//	public enum DownLoadState
//	{
//		PREERROR = 1,
//		PREDOWNLOADING = 2,
//		DOWNLOADING = 3,
//		SUCCESS_DOWNLOAD = 4,
//		COMPLETE = 5,
//	}
//
//
//
//
//	private IEnumerator DownloadRemoteAssetFile(){
//
//		WWW www = new WWW (remoteServerPath+"/"+assetFileToolName);
//		yield return www;
//
//		Debug.LogWarning ("over");
//		if (!string.IsNullOrEmpty (www.error)) {
//			downLoadState = DownLoadState.PREERROR;
//			Debug.LogError ("download file error:" + remoteServerPath+"/"+assetFileToolName
//				+"   "+www.error);
//			yield return null;
//		} else {
//			remoteAssetFileTool = AssetFileTool.LoadFromString (www.text);
//
//			if (CheckNeedUpdateAsset ()) {
//				downLoadState = DownLoadState.DOWNLOADING;
//			} else {
//				downLoadState = DownLoadState.SUCCESS_DOWNLOAD;
//			}
//		}
//
//	}
//
//
//	protected virtual bool CheckNeedUpdateAsset(){
//		return false;
//	}
//
//	/// <summary>
//	/// 检查更新包是否需要下载,true=下载，false=不下载
//	/// </summary>
//	/// <param name="ac"></param>
//	/// <returns></returns>
//	protected bool CheckIfDownload(AssetCell ac,ref bool needMove)
//	{
//		needMove = false;
//		//判断doc是否存在
//		if (File.Exists(localSavePath + "/" + ac.path))
//		{
//			FileInfo fi = new FileInfo(localSavePath + "/" + ac.path);
//			if (fi.Length == ac.size)
//			{
//				return false;
//			}
//			else
//			{
//				File.Delete(localSavePath + "/" + ac.path);
//			}
//		}
//
//		if (!File.Exists(localTmpSavePath + "/" + ac.path))
//		{
//			return true;
//		}
//		else
//		{
//			FileInfo fi = new FileInfo(localTmpSavePath + "/" + ac.path);
//			if (fi.Length != ac.size)
//			{
//				File.Delete(localTmpSavePath + "/" + ac.path);
//				return true;
//			}
//			else
//			{
//				needMove = true;
//				return false;
//			}
//		}
//	}
//
//}
//
