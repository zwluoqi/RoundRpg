using System;
using System.Collections.Generic;
using UnityEngine;

public class TexturePackerManager
{
	private static TexturePackerManager _instance;

	public static TexturePackerManager Instance{
		get{
			if (_instance == null) {
				_instance = new TexturePackerManager ();
			}
			return _instance;
		}
	}


	public Dictionary<string,TexturePacker> spritePackers = new Dictionary<string, TexturePacker>();


	public Sprite GetSprite(string atlasName,string spriteName){
		if (!spritePackers.ContainsKey (atlasName)) {
			spritePackers [atlasName] = new TexturePacker (atlasName);
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
		spritePackers [atlasName].Destory ();
		spritePackers.Remove (atlasName);	
	}
}
