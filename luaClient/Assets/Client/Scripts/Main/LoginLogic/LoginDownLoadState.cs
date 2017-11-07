using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoginDownLoadState : LoginState{



	public LoginDownLoadState(LoginStateEnum stateType,LoginManager controller):base(stateType,controller){

	}

	public override void Enter ()
	{
		base.Enter ();

		loginManager.checkResUILogic.ShowCheck();

		foreach (var assetDownloadTool in AssetResDownloadTool.resDownLoadTools) {
			assetDownloadTool.StartPreDownLoad ();
		}
	}



	public override void Leave ()
	{
		base.Leave ();
	}

	public List<AssetResDownloadTool> errorLists = new List<AssetResDownloadTool>();

	public override void Update ()
	{
		base.Update ();

		List<AssetResDownloadTool> tmps = new List<AssetResDownloadTool> ();
		tmps.AddRange (AssetResDownloadTool.resDownLoadTools);
		foreach (var tmp in tmps) {
			tmp.Tick ();
			switch (tmp.downLoadState) {
				case AssetResDownloadTool.DownLoadState.DOWNLOADING:
					loginManager.checkResUILogic.ShowDownLoading ();
				break;
				case AssetResDownloadTool.DownLoadState.COMPLETE:
					AssetResDownloadTool.resDownLoadTools.Remove (tmp);
					break;
				case AssetResDownloadTool.DownLoadState.PREERROR:
					errorLists.Add (tmp);
					AssetResDownloadTool.resDownLoadTools.Remove (tmp);
					break;
			}
		}

		if (CheckIsDownLoadOver ()) {
			controller.SwitchState (LoginStateEnum.Main);
		} else {

		}
	}



	public bool CheckIsDownLoadOver(){
		if (AssetResDownloadTool.resDownLoadTools.Count > 0) {
			return false;
		}
//		if (errorLists.Count > 0) {
//			return false;
//		}
		return true;
	}

}
