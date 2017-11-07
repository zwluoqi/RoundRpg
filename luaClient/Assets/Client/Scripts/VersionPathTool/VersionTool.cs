using System;
using System.IO;



public class VersionTool
{
	private static VersionTool _instance;
	public static VersionTool Instance{
		get{
			if (_instance == null) {
				_instance = new VersionTool ();
			}
			return _instance;
		}
	}


//	public string serverVersion;
	public const string packageVersion = "0.0.1.0.0";
	public string currentVersion{ get; private set;}

//	public VersionCell serverVersionCell = new VersionCell();

	public VersionCell currentVersionCell = new VersionCell();

	public void InitVersionData(){
		if (File.Exists (PathTool.VersionFilePath)) {
			string tmp = File.ReadAllText (PathTool.VersionFilePath);
			string fileVersion = tmp;
			currentVersion = fileVersion;
		} else {
			currentVersion = packageVersion;
			FileUtils.WriteAllText (PathTool.VersionFilePath, currentVersion);
		}
		currentVersionCell = new VersionCell ();
		currentVersionCell.CalculateVersion (currentVersion);
	}

//	public bool CheckServerVersion(string serverVersion){
//		this.serverVersion = serverVersion;
//		serverVersionCell = new VersionCell ();
//		serverVersionCell.CalculateVersion (serverVersion);
//
//		if (serverVersion == currentVersion) {
//			return true;
//		} else {
//			return false;
//		}
//	}

//	public void WriteNewVersion(string newVersion){
//		currentVersion = newVersion;
//		FileUtils.WriteAllText (PathTool.VersionFilePath, currentVersion);
//		currentVersionCell.CalculateVersion (currentVersion);
//	}

//	public void UseOldVersion(){
//		;
//	}
//
//	public bool IsNeedUpdateStaticRes()
//	{
//		if (currentVersionCell.BundleVersion != serverVersionCell.BundleVersion) {
//			return false;
//		}
//		if (currentVersionCell.CodeVersion != serverVersionCell.CodeVersion) {
//			return false;
//		}
//		if (currentVersionCell.StaticResVersion != serverVersionCell.StaticResVersion)
//			return true;
//		return false;
//	}
//
//	public bool IsNeedUpdateDynamicRes()
//	{
//		if (currentVersionCell.DynamicResVersion != serverVersionCell.DynamicResVersion)
//			return true;
//		return false;
//	}
}

