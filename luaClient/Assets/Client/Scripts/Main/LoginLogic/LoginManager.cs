using UnityEngine;
using System.Collections;

public class LoginManager : FsmControllerBase<LoginStateEnum> {

	public CheckResUI checkResUILogic;

	public void CreateCheckUI(){
		Object obj  = Resources.Load ("ui/page/check_res_ui");
		GameObject checkResUI = GameObject.Instantiate (obj) as GameObject;
		checkResUI.transform.SetParent (Main.Instance.uiRootTrans);
		UIUtil.ResetRectPos (checkResUI);
		checkResUILogic = checkResUI.GetComponent<CheckResUI> ();
	}

	public void DestroyCheckUI(){
		GameObject.Destroy (checkResUILogic.gameObject);
		checkResUILogic = null;
	}


	public override void InitState ()
	{
		base.InitState ();
//		allStates [LoginStateEnum.CheckVersion] = new LoginCheckVersionState (LoginStateEnum.CheckVersion, this);
//		allStates [LoginStateEnum.DownLoad] = new LoginDownLoadState (LoginStateEnum.DownLoad, this);
//		
		allStates [LoginStateEnum.Main] = new LoginMainState (LoginStateEnum.Main, this);

		SwitchState (LoginStateEnum.Main);

		Main.Instance.updateEvent += UpdateState;
	}



	public void Clear(){
		Main.Instance.updateEvent -= UpdateState;
	}

}
