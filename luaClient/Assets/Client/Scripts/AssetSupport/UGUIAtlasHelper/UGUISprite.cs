using System;
using UnityEngine.UI;
using UnityEngine;

public class UGUISprite:Image
{

	// Cached and saved values
	[HideInInspector][SerializeField] UGUIAtlas mAtlas;

	public string spriteName
	{
		get{
			if (sprite == null)
				return "";
			return sprite.name;
		}set{
			sprite = mAtlas.GetSprite (value);
		}
	}
}

