using UnityEngine;
using System.Collections;

public enum LoginStateEnum{
	CheckVersion,
	DownLoad,
	Main,
}

public class LoginState : FsmStateBase<LoginStateEnum> {

	public LoginManager loginManager;

	public LoginState(LoginStateEnum stateType,LoginManager controller):base(stateType,controller){
		loginManager = controller;

	}
}
