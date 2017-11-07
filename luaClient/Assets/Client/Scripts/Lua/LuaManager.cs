using UnityEngine;
using System.Collections;
using LuaInterface;
using System.IO;
using System.Text;


public class LuaManager :MonoBehaviour
{
    private SimpleLuaClient mSimpleLuaClient;

    private void InitLuaPath()
    {
		
	}


	public void StartSimpleLuaClinet(){
		mSimpleLuaClient = gameObject.AddComponent<SimpleLuaClient>();
		InitLuaPath();
		mSimpleLuaClient.OnLuaFilesLoaded();
	
	}

	public void StopSimpleLuaClient(){
		GameObject.Destroy (mSimpleLuaClient);
		mSimpleLuaClient = null;
	}





    public object[] CallFunction(string funcName, params object[] args)
    {
        LuaFunction func = SimpleLuaClient.GetMainState().GetFunction(funcName);
        if (func != null)
        {
            return func.Call(args);
        }
        return null;
    }


}
