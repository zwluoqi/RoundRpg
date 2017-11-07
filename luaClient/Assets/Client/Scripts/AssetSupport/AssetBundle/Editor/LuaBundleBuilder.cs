
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class LuaBundleBuilder
{

	[MenuItem("Totem_H1/Copy Lua  files to Resources", false, 51)]
	public static void CopyLuaFilesToRes()
	{
		ClearAllLuaFiles();
		string destDir = Application.dataPath + "/Resources" + "/Lua";
		CopyLuaBytesFiles(LuaConst.luaDir, destDir);
		CopyLuaBytesFiles(LuaConst.toluaDir, destDir);
		AssetDatabase.Refresh();
		Debug.Log("Copy lua files over");
	}

	static void CopyLuaBytesFiles(string sourceDir, string destDir, bool appendext = true, string searchPattern = "*.lua", SearchOption option = SearchOption.AllDirectories)
	{
		if (!Directory.Exists(sourceDir))
		{
			return;
		}

		string[] files = Directory.GetFiles(sourceDir, searchPattern, option);
		int len = sourceDir.Length;

		if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
		{
			--len;
		}         

		for (int i = 0; i < files.Length; i++)
		{
			string str = files[i].Remove(0, len);
			str = str.Replace ('/', '.');
			str = str.Replace ('\\', '.');
			str = str.Substring (1);

			string dest = destDir + "/" + str;
			if (appendext) dest += ".txt";
			string dir = Path.GetDirectoryName(dest);
			Directory.CreateDirectory(dir);
			File.Copy(files[i], dest.ToLower(), true);
		}
	}


	static void ClearAllLuaFiles()
	{
//		string osPath = Application.streamingAssetsPath + "/" + GetOS();
//
//		if (Directory.Exists(osPath))
//		{
//			string[] files = Directory.GetFiles(osPath, "Lua*.unity3d");
//
//			for (int i = 0; i < files.Length; i++)
//			{
//				File.Delete(files[i]);
//			}
//		}
//
//		string path = osPath + "/Lua";
//
//		if (Directory.Exists(path))
//		{
//			Directory.Delete(path, true);
//		}
//
//		path = Application.streamingAssetsPath + "/Lua";
//
//		if (Directory.Exists(path))
//		{
//			Directory.Delete(path, true);
//		}
//
//		path = Application.dataPath + "/temp";
//
//		if (Directory.Exists(path))
//		{
//			Directory.Delete(path, true);
//		}
//
//		path = Application.dataPath + "/Resources/Lua";
//
//		if (Directory.Exists(path))
//		{
//			Directory.Delete(path, true);
//		}
//
//		path = Application.persistentDataPath + "/" + GetOS() + "/Lua";
//
//		if (Directory.Exists(path))
//		{
//			Directory.Delete(path, true);
//		}
	}

	static string GetOS()
	{
		return LuaConst.osDir;
	}
}
