using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;
using System.Text;
using UnityEngine;

public class BundleTools
{

	#region Tools
	public static string GetTargetPlatform()
	{
		return EditorUserBuildSettings.activeBuildTarget.ToString();
	}

	public static string GetABManefistName()
	{
		switch (EditorUserBuildSettings.activeBuildTarget)
		{
		case BuildTarget.Android:
			return "android";
		case BuildTarget.iOS:
			return "ios";
		default:
			return "pc";
		}
	}


	public static string GetMD5HashFromFile(string fileName)
	{
		try
		{
			FileStream file = new FileStream(fileName, FileMode.Open);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}
		catch (Exception ex)
		{
			throw new Exception("GetMD5HashFromFile() fail, error:" + ex.Message);
		}
	}

	/// 删除指定目录下的所有文件及其子目录
	public static void DeleteAllFilesInFolder(string strPath)
	{
		//删除这个目录下的所有子目录
		if (Directory.GetDirectories(strPath).Length > 0)
		{
			foreach (string var in Directory.GetDirectories(strPath))
			{
				//DeleteDirectory(var);
				Directory.Delete(var, true);
				//DeleteDirectory(var);
			}
		}
		//删除这个目录下的所有文件
		if (Directory.GetFiles(strPath).Length > 0)
		{
			foreach (string var in Directory.GetFiles(strPath))
			{
				File.Delete(var);
			}
		}

	}
		

	public static string GetBuildBundlePath(bool isStaticRes)
	{
		string platformName = GetABManefistName();
		string appVersion = GetVersionCode ().AppCode;
		string mainFest = 	(isStaticRes ? AssetFileTool.staticMainFest : AssetFileTool.dynamicMainFest);
		string bundlePath = Application.dataPath + "/../AssetBundles/"+appVersion+"/"+ platformName+"/"+ mainFest +"/";
		return bundlePath.Replace(@"\", @"/");
	}

	public static VersionCell GetVersionCode(){
		VersionCell vc = new VersionCell ();
		vc.CalculateVersion (VersionTool.packageVersion);
		return vc;
	}

	public static void CreateBuildBundleDir(bool isStaticRes)
	{
		string bundlePath = GetBuildBundlePath(isStaticRes);

		if (!Directory.Exists(bundlePath))
		{
			Directory.CreateDirectory(bundlePath);
		}
	}

	public static void ClearBuildBundleDir(bool isStaticRes)
	{
		string bundlePath = GetBuildBundlePath(isStaticRes);

		if (Directory.Exists(bundlePath))
		{
			DeleteAllFilesInFolder(bundlePath);
		}
		Directory.CreateDirectory(bundlePath);
	}



	#endregion
}
