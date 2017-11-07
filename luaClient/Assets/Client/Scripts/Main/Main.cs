using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Main : MonoBehaviour {
	public Transform uiRootTrans;

	private LuaManager luaManager;
	private LoginManager loginManager;

	public delegate void UpdateEvent();
	public event UpdateEvent updateEvent;



	void Awake(){
		_instance = this;
		GameObject.DontDestroyOnLoad (this.gameObject);


		Application.runInBackground = true;

		#if UNITY_EDITOR
		ReadyConfig lua_config = ReadyConfig.GetReadConfig (Application.dataPath + "/Client/lua_config.txt");

		string open_lua_socket =	lua_config.datas["open_lua_socket"];
		string open_zbs_debugger = lua_config.datas["open_zbs_debugger"];

		LuaConst.openLuaSocket = open_lua_socket == "1";
		LuaConst.openZbsDebugger = open_zbs_debugger == "1";
		#endif
	}

	void Start () {
		luaManager = gameObject.AddComponent<LuaManager> ();

		StartLoginLogic ();
	}

	public void StartLoginLogic(){
		loginManager = new LoginManager ();
		loginManager.InitState ();
	}

	public void StopLoginLogic(){
		loginManager.Clear ();
		loginManager = null;
	}


	public void Update(){
		if (updateEvent != null) {
			updateEvent ();
		}
	}



	public void StartLuaManager(){
		luaManager.StartSimpleLuaClinet();
	}

	public void StopLuaManager(){
		luaManager.StopSimpleLuaClient ();
	}



	private static Main _instance;
	public static Main Instance{
		get{
			return _instance;
		}
	}

}
