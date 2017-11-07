//using System;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class DynamicResDownload:AssetResDownloadTool
//{
//	protected override bool CheckNeedUpdateAsset(){
//		//统计需要下载的资源包
//		foreach (var item in remoteAssetFileTool.assetCells)
//		{
//			AssetCell ac = item.Value;
//			bool needMove = false;
//			if (CheckIfDownload (ac, ref needMove)) {
//				totalSize += ac.size;
//				totalDownloadCount++;
//				if (!preDownLoadList.Contains (ac)) {
//					preDownLoadList.Add (ac);
//				}
//				Debug.LogWarning("数据包需要更新的的bundle数据为:" + ac.path);
//			} else if (needMove) {
//				FileUtils.MoveCoverage (localTmpSavePath+"/"+ac.path,localSavePath+"/"+ac.path );
//			}
//		}
//
//		return preDownLoadList.Count > 0;
//	}
//
//}
