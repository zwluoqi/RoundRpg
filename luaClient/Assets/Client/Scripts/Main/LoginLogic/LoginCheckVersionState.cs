using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class LoginCheckVersionState:LoginState  {



	public LoginCheckVersionState(LoginStateEnum stateType,LoginManager controller):base(stateType,controller){

	}





	public override void Enter ()
	{
		base.Enter ();

		loginManager.CreateCheckUI ();


		VersionTool.Instance.InitVersionData ();
		AssetFileToolManager.WriteInnerAssetFileToolAndMainfestFlie ();


		AssetResDownloadTool staticResDownloadTool = new StaticResDownload().Init (
			PathTool.CurrentVersionStaticResDataPath,
			PathTool.NewVersionStaticReDataPath,
			ServerPathTool.NewVersionStaticReDataPath,
			AssetFileTool.staticAssetFileTool);
		AssetResDownloadTool.AddPreLoadAssetDownLoadTool (staticResDownloadTool);

		AssetResDownloadTool dynamicResDownloadTool = new DynamicResDownload().Init (
			PathTool.CurrentVersionDynamicResDataPath, 
			PathTool.NewVersionDynamicResDataPath,
			ServerPathTool.NewVersionDynamicResDataPath,
			AssetFileTool.dynamicAssetFileTool);
		AssetResDownloadTool.AddPreLoadAssetDownLoadTool (dynamicResDownloadTool);
			

		if (AssetResDownloadTool.NeedDownLoadAsset ()) {
			controller.SwitchState (LoginStateEnum.DownLoad);
		}

	}



}
