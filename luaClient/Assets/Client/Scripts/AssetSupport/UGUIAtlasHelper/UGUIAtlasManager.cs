using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UGUIAtlasManager  {

	private static UGUIAtlasManager _instance;

	public static UGUIAtlasManager Instance{
		get{
			if (_instance == null) {
				_instance = new UGUIAtlasManager ();
			}
			return _instance;
		}
	}


	public Dictionary<string,UGUIAtlas> spritePackers = new Dictionary<string, UGUIAtlas>();


	public Sprite GetSprite(string atlasName,string spriteName){
		if (!spritePackers.ContainsKey (atlasName)) {
			spritePackers [atlasName] = LoadResources (atlasName);
		}
		return spritePackers [atlasName].GetSprite (spriteName);
	}


	public void UnLoadAllAtlas(){
		foreach (var spritePacker in spritePackers) {
			spritePacker.Value.Destory ();
		}
		spritePackers.Clear ();
	}

	public void UnLoadAtlas(string atlasName){
		UGUIAtlas sp = null;
		if (spritePackers.TryGetValue (atlasName, out sp)) {
			sp.Destory ();
		}


		spritePackers.Remove (atlasName);	
	}

	public static UGUIAtlas LoadResources(string atlasName){
		string atlasPath = "Atlas/" + atlasName;

		UGUIAtlas atlas = null;
		ResUseMode useMode = ResourceManager.GetResourceUseMode (atlasPath);
		switch (useMode) {
		case ResUseMode.RESOURCE:
			atlas = Resources.Load<UGUIAtlas> (atlasPath);
			break;
		case ResUseMode.PERSIS_STATIC_RESOURCE:
			{
				AssetBundle ab = ResourceManager.Instance.staticAssetBundleManager.LoadAssetBundle (atlasPath+".ab");
				atlas = ab.LoadAsset<UGUIAtlas> (atlasName.ToLower());
				ResourceManager.Instance.staticAssetBundleManager.UnLoadAssetBundle (atlasPath+".ab");
			}
			break;
		case ResUseMode.PERSIS_DYNAMIC_RESOURCE:
			{
				AssetBundle ab = ResourceManager.Instance.dynamicAssetBundleManager.LoadAssetBundle (atlasPath+".ab");
				atlas = ab.LoadAsset<UGUIAtlas> (atlasName.ToLower());
				ResourceManager.Instance.staticAssetBundleManager.UnLoadAssetBundle (atlasPath+".ab");

			}
			break;
		}

		return atlas;

	}
}
