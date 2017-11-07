using UnityEngine;
using System.Collections.Generic;

public class TexturePacker {

	public string atlasPath;
	public Texture2D mianTexture;
	public Dictionary<string,Sprite> spriteDict = new Dictionary<string, Sprite>();

	public TexturePacker(string atlasName){
		atlasPath = "Atlas/" + atlasName;
		Sprite[] sprites = null;

		ResUseMode useMode = ResourceManager.GetResourceUseMode (atlasPath);
		switch (useMode) {
		case ResUseMode.RESOURCE:
			sprites = Resources.LoadAll<Sprite> (atlasPath);
			break;
		case ResUseMode.PERSIS_STATIC_RESOURCE:
			{
				AssetBundle ab = ResourceManager.Instance.staticAssetBundleManager.LoadAssetBundle (atlasPath+".ab");
				sprites = ab.LoadAllAssets<Sprite> ();
				ResourceManager.Instance.staticAssetBundleManager.UnLoadAssetBundle (atlasPath+".ab");
			}
			break;
		case ResUseMode.PERSIS_DYNAMIC_RESOURCE:
			{
				AssetBundle ab = ResourceManager.Instance.dynamicAssetBundleManager.LoadAssetBundle (atlasPath+".ab");
				sprites = ab.LoadAllAssets<Sprite> ();
				ResourceManager.Instance.staticAssetBundleManager.UnLoadAssetBundle (atlasPath+".ab");

			}
			break;
		}

		if (sprites == null || sprites.Length ==0) {
			Debug.LogError ("error atlasName:"+atlasPath);
		} else {
			InsertSprites (sprites[0].texture,sprites);
		}
	}

	public void Destory(){

		Resources.UnloadAsset (mianTexture);

		spriteDict.Clear ();
	}

	private void InsertSprites(Texture2D texture, Sprite[] sprites){
		this.mianTexture = texture;
		for(int i=0;i<sprites.Length;i++) {
			spriteDict [sprites[i].name] = sprites[i] as Sprite;
		}
	}


	public Sprite GetSprite(string spriteName){
		if (!spriteDict.ContainsKey (spriteName)) {
			Debug.LogError ("error spriteName:"+spriteName);
			spriteDict [spriteName] = Sprite.Create(Texture2D.whiteTexture,new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f));
		} 
		return spriteDict [spriteName];
	}

}
