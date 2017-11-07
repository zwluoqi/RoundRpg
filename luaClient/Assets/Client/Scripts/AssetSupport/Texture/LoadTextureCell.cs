using System;
using UnityEngine.UI;

public class LoadTextureCell
{
	public RawImage iconTexture;
	public string path;
	public bool alpha;

	public LoadTextureCell(RawImage _uiTexture, string _path, bool _alpha)
	{
		iconTexture = _uiTexture;
		path = _path;
		alpha = _alpha;
	}
}

