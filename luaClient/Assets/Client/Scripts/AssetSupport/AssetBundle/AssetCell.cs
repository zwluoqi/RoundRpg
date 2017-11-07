using UnityEngine;
using System.Collections;

public enum AssetLoadLevel
{
	BasicPack = 0,
	Pack1,
	Pack2,
	Pack3,
	Pack4,
	Pack5,
	Pack6,
	Pack7,
	Pack8,
}
/// <summary>
/// Asset信息
/// </summary>
public class AssetCell
{
	//路径
	public string path;

	//对文件的hash值
	public string hashCode;

	//文件大小
	public long size;

	public int loadLevel;

	public AssetCell()
	{

	}

	public AssetCell(string path, string hashCode, long size, int level = 0)
	{
		this.path = path;
		this.hashCode = hashCode;
		this.size = size;
		this.loadLevel = level;
	}

	public bool ReadData(string data)
	{
		string[] ds = data.Split(new char[] { '\t' });
		if (ds.Length != 4)
		{
			return false;
		}
		path = ds[0];
		hashCode = ds[1];
		size = long.Parse(ds[2]);
		loadLevel = int.Parse(ds[3]);
		return true;
	}

	public static AssetCell GetAssetCellByStr(string data)
	{
		string[] ds = data.Split(new char[] { '\t' });
		if (ds.Length != 4)
		{
			return null;
		}
		return new AssetCell(ds[0], ds[1], long.Parse(ds[2]), int.Parse(ds[3]));
	}

	public override string ToString()
	{
		return path + "\t" + hashCode + "\t" + size + "\t" + loadLevel;
	}
}
