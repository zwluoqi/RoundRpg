using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

public class UILuaBehaviour : MonoBehaviour {
	//避免在lua里面频繁调用find的手段，空间换时间的手段
//	public List<RectTransform> trans = new List<RectTransform> ();
//	public List<UnityEngine.UI.Slider> sliders = new List<UnityEngine.UI.Slider> ();
//	public List<UnityEngine.UI.Text> texts = new List<UnityEngine.UI.Text> ();
//	public List<UnityEngine.UI.Image> images = new List<UnityEngine.UI.Image> ();

	public void Init(LuaTable tb)
	{
		LuaState mLuaState = SimpleLuaClient.GetMainState();
		if (mLuaState == null) return;

		LuaTable mLuaTable = null;
		if (tb == null)
		{
			Debug.LogError ("UILuaBehaviour:nil");
		}
		else
		{
			mLuaTable = tb;
		}
		if (mLuaTable == null)
		{
			Debug.LogWarning("mLuaTable is null:" + name);
			return;
		}
		mLuaTable["gameObject"] = gameObject;
		mLuaTable["transform"] = transform;
		mLuaTable["ui_lua_behaviour"] = this;
//		foreach (var tran in trans) {
//			mLuaTable ["cross_rect_transform_" + tran.name] = tran;
//		}
//		foreach (var slider in sliders) {
//			mLuaTable ["cross_slider_" + slider.name] = slider;
//		}
//		foreach (var text in texts) {
//			mLuaTable ["cross_text_" + text.name] = text;
//		}
//		foreach (var image in images) {
//			mLuaTable ["cross_image_" + image.name] = image;
//		}
	}
}
