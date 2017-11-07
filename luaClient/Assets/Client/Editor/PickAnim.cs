using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;

public class PickAnim  {

	/// <summary>
	/// 提取目录下所有fbx的动画资源,
	/// </summary>
	/// <param name="dir">源目录</param>
	/// <param name="dirName">目标目录</param>
	public static void PickAllAnimations(string srcDir)
	{
		//判断有无源目录
		if (!IsFolderExists(srcDir))
			return;
//		//如果目标路径不为空则创建
//		if(dstDir != null)
//			CreateFolder(dstDir);
		sb = new StringBuilder();
		srcDir = string.Concat("Assets/", srcDir);
		DirectoryInfo dirInfo = new DirectoryInfo(srcDir);
		PickAinmByDirInfo (srcDir, dirInfo);

		Debug.LogWarning (sb.ToString ());
		AssetDatabase.Refresh();

	}

	private static void PickAinmByDirInfo(string srcDir, DirectoryInfo dirInfo){
		FileInfo[] files = dirInfo.GetFiles();
		foreach (FileInfo file in files) {
			if (!file.Name.EndsWith (".meta") && !file.Name.EndsWith (".DS_Store") && !file.Name.EndsWith (".tpsheet")) {
				PickAnimByFileInfo (srcDir, file);
			}
		}
		DirectoryInfo[] dirs = dirInfo.GetDirectories ();
		foreach (DirectoryInfo dir in dirs) {
			PickAinmByDirInfo (srcDir+"/"+dir.Name,dir);
		}
	}
	private static StringBuilder sb;

	private static void PickAnimByFileInfo(string srcDir,FileInfo file){
			
			string dstDir = srcDir.Replace ("Res", "Resources");

			string resPath = string.Concat(srcDir, "/", file.Name);
			UnityEngine.Object[] clips = AssetDatabase.LoadAllAssetsAtPath(resPath);
			for (int i = 0; i < clips.Length; i++)
			{
				if (clips[i] is AnimationClip)
				{
					sb.AppendLine ("pick anim:"+resPath);
//					Debug.LogWarning ("pick anim:"+resPath);
					AnimationClip clip = (AnimationClip)clips[i];
					if (clip.name.StartsWith("__preview__"))
					{   //最后一个是原始动画，不需要
						continue;
					}

					string oldPath = string.Concat(srcDir, "/", clip.name, ".anim");
					//设置被提取的动画文件的存放路径
					string newPath = "";
					if (dstDir != null)
						newPath = string.Concat(dstDir, "/", clip.name, ".anim");
					else
						newPath = oldPath;

					AnimationClip oldClip = AssetDatabase.LoadAssetAtPath(newPath, typeof(AnimationClip)) as AnimationClip;
					string[] arrAnim = { "idle", "run_2", "ride_idle", "combat" };
					if (oldClip == null)
					{
						AnimationClip newClip = new AnimationClip();
						EditorUtility.CopySerialized(clip, newClip);
						CreateFolder(dstDir.Replace("Assets/",""));
						AssetDatabase.CreateAsset(newClip, newPath);

						for (int j = 0; j != arrAnim.Length; ++j)
						{
							if (clip.name.Equals(arrAnim[j]))
							{
								AssetDatabase.SetLabels(oldClip, new string[] { "totme_base_anim" });
								break;
							}
						}
					}
					else
					{
						EditorUtility.CopySerialized(clip, oldClip);

						for(int j=0; j != arrAnim.Length; ++j)
						{
							if(clip.name.Equals(arrAnim[j]))
							{
							AssetDatabase.SetLabels(oldClip, new string[] { "totme_base_anim" });
								break;
							}
						}
					}


				}
			}


	}

	/// 删除文件
	public static void DeleteFile(string fileName)
	{
		if (IsFileExists(fileName))
		{
			File.Delete(GetFullPath(fileName));

			AssetDatabase.Refresh();
		}
	}

	/// 检测是否存在文件夹
	public static bool IsFolderExists(string folderPath)
	{
		if (folderPath.Equals(string.Empty))
		{
			return false;
		}

		return Directory.Exists(GetFullPath(folderPath));
	}

	/// 创建文件夹
	public static void CreateFolder(string folderPath)
	{
		if (!IsFolderExists(folderPath))
		{
			Directory.CreateDirectory(GetFullPath(folderPath));

			AssetDatabase.Refresh();
		}
	}

	/// 复制文件夹
	public static void CopyFolder(string srcFolderPath, string destFolderPath)
	{

		#if !UNITY_WEBPLAYER
		if (!IsFolderExists(srcFolderPath))
		{
			return;
		}

		CreateFolder(destFolderPath);


		srcFolderPath = GetFullPath(srcFolderPath);
		destFolderPath = GetFullPath(destFolderPath);

		// 创建所有的对应目录
		foreach (string dirPath in Directory.GetDirectories(srcFolderPath, "*", SearchOption.AllDirectories))
		{
			Directory.CreateDirectory(dirPath.Replace(srcFolderPath, destFolderPath));
		}

		// 复制原文件夹下所有内容到目标文件夹，直接覆盖
		foreach (string newPath in Directory.GetFiles(srcFolderPath, "*.*", SearchOption.AllDirectories))
		{

			File.Copy(newPath, newPath.Replace(srcFolderPath, destFolderPath), true);
		}

		AssetDatabase.Refresh();
		#endif

		#if UNITY_WEBPLAYER
		Debug.LogWarning("FileStaticAPI::CopyFolder is innored under wep player platfrom");
		#endif
	}

	/// 删除文件夹
	public static void DeleteFolder(string folderPath)
	{
		#if !UNITY_WEBPLAYER
		if (IsFolderExists(folderPath))
		{

			Directory.Delete(GetFullPath(folderPath), true);

			AssetDatabase.Refresh();
		}
		#endif

		#if UNITY_WEBPLAYER
		Debug.LogWarning("FileStaticAPI::DeleteFolder is innored under wep player platfrom");
		#endif
	}

	/// 返回Application.dataPath下完整目录
	private static string GetFullPath(string srcName)
	{
		if (srcName.Equals(string.Empty))
		{
			return Application.dataPath;
		}

		if (srcName[0].Equals('/'))
		{
			srcName.Remove(0, 1);
		}

		return Application.dataPath + "/" + srcName;
	}

	/// 检测文件是否存在Application.dataPath目录
	public static bool IsFileExists(string fileName)
	{
		if (fileName.Equals(string.Empty))
		{
			return false;
		}

		return File.Exists(GetFullPath(fileName));
	}

}
