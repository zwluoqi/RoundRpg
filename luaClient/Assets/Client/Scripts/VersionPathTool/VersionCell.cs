using System;


public class VersionCell{
	private int _bundleVersion;
	private int _codeVersion;
	private int _staticResVersion;
	private int _dynamicResVersion;
	private int _litttleVersion;

	public string AppCode{
		get{
			return _bundleVersion + "." + _codeVersion;
		}
	}

	public int BundleVersion{
		get{
			return _bundleVersion;
		}
	}

	public int CodeVersion{
		get{
			return _codeVersion;
		}
	}


	public int StaticResVersion{
		get{
			return _staticResVersion;
		}
	}
	public int DynamicResVersion{
		get{
			return _dynamicResVersion;
		}
	}
	public int LitttleVersion{
		get{
			return _litttleVersion;
		}
	}
	public void CalculateVersion(string currentVersion){
		char[] splis = { '.' };
		string[] versions = currentVersion.Split (splis, StringSplitOptions.RemoveEmptyEntries);
		int i = 0;
		this._bundleVersion = int.Parse (versions [i++]);
		this._codeVersion = int.Parse (versions [i++]);
		this._staticResVersion = int.Parse (versions [i++]);
		this._dynamicResVersion = int.Parse (versions [i++]);
		this._litttleVersion = int.Parse (versions [i++]);

	}
}
