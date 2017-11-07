using System;
using UnityEngine;
using System.Collections.Generic;

public class StaticResDownload:AssetResDownloadTool
{

	protected override bool CheckNeedUpdateAsset(){
		List<AssetCell> unneceList;
		List<AssetCell> lackList;
		List<AssetCell> obbList;
		AssetFileTool.Compare(AssetFileToolManager.innerStaticAssetFileTool, remoteAssetFileTool, out unneceList, out lackList, out obbList);


		List<AssetCell> checkAssetcells = new List<AssetCell> ();
		checkAssetcells.AddRange (lackList);
		checkAssetcells.AddRange (obbList);

		//统计需要下载的资源包
		foreach (var item in checkAssetcells)
		{
			AssetCell ac = item;
			bool needMove = false;
			if (CheckIfDownload (ac, ref needMove)) {
				totalSize += ac.size;
				totalDownloadCount++;
				if (!preDownLoadList.Contains (ac)) {
					preDownLoadList.Add (ac);
				}
				Debug.LogWarning("数据包需要更新的的bundle数据为:" + ac.path);

			} else if (needMove) {
				FileUtils.MoveCoverage (localTmpSavePath+"/"+ac.path,localSavePath+"/"+ac.path );
			}
		}

		return preDownLoadList.Count > 0;
	}

}