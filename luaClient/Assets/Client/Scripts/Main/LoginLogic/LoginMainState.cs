using UnityEngine;
using System.Collections;



public class LoginMainState :LoginState{



	public LoginMainState(LoginStateEnum stateType,LoginManager controller):base(stateType,controller){

	}

	public override void Enter ()
	{
		base.Enter ();

		loginManager.DestroyCheckUI ();

		AssetFileToolManager.ReadCurrentAssetFile ();
		AssetFileToolManager.UpdateStaticAssets ();
		ResourceManager.Instance.staticAssetBundleManager.UpdateAssetBundleMainfestInfo (PathTool.CurrentVersionStaticResDataPath, AssetFileToolManager.staticMainFestPath);
		ResourceManager.Instance.dynamicAssetBundleManager.UpdateAssetBundleMainfestInfo (PathTool.CurrentVersionDynamicResDataPath, AssetFileToolManager.dynamicMainFestPath);

		Main.Instance.StopLoginLogic ();
		Main.Instance.StartLuaManager ();
	}


}
