using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using LuaInterface;
using System.IO;
using System.Text;

public class SimpleLuaResLoader : LuaFileUtils
{

	public Dictionary<string,TextAsset> tas;
	public SimpleLuaResLoader()
	{
		instance = this;
	}

	public override byte[] ReadFile(string fileName)
	{
		string bundleName = fileName.Replace ('/','.');
		if (bundleName.StartsWith (".")) {
			bundleName = bundleName.Substring (1);
		}

		byte[] buffer = null;
		if (ResourceManager.GetResourceUseMode ("lua.ab") == ResUseMode.PERSIS_STATIC_RESOURCE) {
			buffer = ReadDownLoadFile(bundleName);
		}
		if (buffer == null) {
			buffer = base.ReadFile (fileName);
		}
		if (buffer == null)
		{
			buffer = ReadResourceFile(bundleName);
		}

		return buffer;
	}

	private byte[] ReadResourceFile(string fileName)
	{
		byte[] buffer = null;
		if (!fileName.EndsWith(".lua"))
		{
			fileName += ".lua";
		}
		string path = "lua/" + fileName;
		TextAsset text = Resources.Load(path, typeof(TextAsset)) as TextAsset;

		if (text != null)
		{
			Debug.LogWarning ("从Resource读取lua文件:"+fileName);
			buffer = text.bytes;
			Resources.UnloadAsset(text);
		}

		return buffer;
	}

	private byte[] ReadDownLoadFile(string fileName)
	{
		if (!fileName.EndsWith(".lua"))
		{
			fileName += ".lua";
		}
		if (tas == null) {
			tas = new Dictionary<string, TextAsset> ();
			AssetBundle assetBundle = ResourceManager.Instance.staticAssetBundleManager.LoadAssetBundle ("lua.ab");
			Object[] objects = assetBundle.LoadAllAssets ();
			ResourceManager.Instance.staticAssetBundleManager.UnLoadAssetBundle ("lua.ab");
			foreach (var obj in objects) {
				tas [obj.name] = obj as TextAsset;
			}
		}
		TextAsset ta = null;
		tas.TryGetValue (fileName+".bytes",out ta);


		if (ta == null) {
			return null;
		} else {
			Debug.LogWarning ("从沙盒读取lua文件:"+fileName);
			return ta.bytes;
		}
	}
}

public class SimpleLuaClient : LuaClient
{
	protected override LuaFileUtils InitLoader()
	{
		return new SimpleLuaResLoader();
	}

	protected override void LoadLuaFiles()
	{
	}

	protected override void OpenLibs()
	{
		base.OpenLibs();
	}

	protected override void CallMain()
	{
		base.CallMain();
	}

	protected override void StartMain()
	{
		base.StartMain();
	}

	protected override void Bind()
	{
		base.Bind();
	}

	protected override void OnLoadFinished()
	{
	}

	public void OnLuaFilesLoaded()
	{
		if (GameConfig.EnableLuaDebug)
		{
			OpenZbsDebugger();
		}

		luaState.Start();
		StartLooper();
		StartMain();
	}
}