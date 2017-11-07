using System;
/*-----------------------------------------------
// File: AssetBundleBuilder.cs
// Description: 
// Author: Shaobing	492057342@qq.com
-----------------------------------------------*/
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class HelpMenu
{
	[MenuItem("Totem_H1/清空沙盒文件", false, 52)]
	public static void CopyLuaFilesToRes()
	{
		DeleteSubDir (new DirectoryInfo (Application.persistentDataPath));
		Debug.LogWarning ("Clear");
	}



	public static void DeleteSubDir(DirectoryInfo parentDir)
	{
		if (parentDir == null)
		{
			throw new NullReferenceException("目录不存在");
		}
		DirectoryInfo[] directories = parentDir.GetDirectories();
		for (int i = 0; i < directories.Length; i++)
		{
			DirectoryInfo dir2 = directories[i];
			FileUtils.DeleteDir(dir2);
		}
		FileInfo[] files = parentDir.GetFiles();
		for (int j = 0; j < files.Length; j++)
		{
			FileInfo fileInfo = files[j];
			FileUtils.DeleteFile(fileInfo.FullName);
		}
	}
}