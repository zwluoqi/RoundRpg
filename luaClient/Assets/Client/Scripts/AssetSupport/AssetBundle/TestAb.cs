using UnityEngine;
using System.Collections;

public class TestAb : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AssetBundle luaBundle = AssetBundle.LoadFromFile (Application.dataPath + "/../AssetBundles/0.0/" + PathTool.GetBundleTargetName ()+"/" + AssetFileTool.staticMainFest+"/"+ "/lua.ab");
//		AssetBundle luaBundle = AssetBundle.LoadFromFile (Application.dataPath + "/../AssetBundles/0.0/" + PathTool.GetBundleTargetName ()+"/" + AssetFileTool.dynamicMainFest+"/"+ "/empty/empty.ab");


		Object[] assets = luaBundle.LoadAllAssets ();
		foreach (var asset in assets) {
			Debug.Log ("name:" + asset.name);
		}
		string aName = "main.lua.txt";
		Object obj = luaBundle.LoadAsset (aName);
		if (obj != null) {
			Debug.Log ("LoadAsset with name:"+aName);
		}
		aName = "main";
		obj = luaBundle.LoadAsset (aName);
		if (obj != null) {
			Debug.Log ("LoadAsset with name:"+aName);
		}
//		Debug.Log ("lua count:" + assets.Length);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
