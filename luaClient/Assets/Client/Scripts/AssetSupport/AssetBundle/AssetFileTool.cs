using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;

/// <summary>
/// 管理AssetCell
/// </summary>
public class AssetFileTool
{
	/// <summary>
	/// AssetFileTool
	/// </summary>
	public const string staticAssetFileTool = "staticassetfiletool";
	public const string dynamicAssetFileTool = "dynamicassetfiletool";

	/// <summary>
	/// mainfest
	/// </summary>
	public const string staticMainFest = "staticmainfest";
	public const string dynamicMainFest = "dynamicmainfest";

    //资源版本号
    public int version;
    public string platform;
    //所有的资源列表
    public Dictionary<string, AssetCell> assetCells = new Dictionary<string, AssetCell>();

    private FileStream alwaysFileStream;
    private StreamWriter streamWriter;

    //文件在apk包内，需要用www加载，用www.text取得字符串
    public static AssetFileTool LoadFromString(string data)
    {
        AssetFileTool aft = new AssetFileTool();
        try
        {
            using (StringReader sr = new StringReader(data))
            {
                aft.version = int.Parse(sr.ReadLine());
                string line = sr.ReadLine();
                aft.platform = line;
                while (line != null)
                {
                    AssetCell ac = new AssetCell();
                    if (ac.ReadData(line))
                    {
                        try
                        {
                            aft.assetCells.Add(ac.path, ac);
                        }
                        catch (Exception e)
                        {
                            Debug.Log(line + ": " + e.Message);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Asset file error " + line);
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            return aft;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Asset file load error " + e);
            return null;
        }

    }
		

    public AssetCell GetAssetByName(string assetName)
    {
        return assetCells[assetName];
    }

    public static int GetResVersionFromString(string content)
    {
        int num = 0;
        try
        {
            using (StringReader sr = new StringReader(content))
            {
                int.TryParse(sr.ReadLine(), out num);
                sr.Close();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Asset file load error " + e);
        }
        return num;
    }
    /// <summary>
    /// 只获取版本号
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static int GetResVersionFromFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Loaded asset is not exist. Path: " + path);
            return 0;
        }

        int version = 0;
        try
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(path)))
            {
                int.TryParse(sr.ReadLine(), out version);
                sr.Close();
                return version;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Asset file load error " + e.Message);
            return version;
        }
    }

    //文件在apk包外
    public static AssetFileTool LoadFromFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Loaded asset is not exist. Path: " + path);
            return null;
        }
        AssetFileTool aft = new AssetFileTool();
        try
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(path)))
            {
                aft.version = int.Parse(sr.ReadLine());
                string line = sr.ReadLine();
                aft.platform = line;
                while (line != null)
                {
                    AssetCell ac = new AssetCell();
                    if (ac.ReadData(line))
                    {
                        if (aft.assetCells.ContainsKey(ac.path))
                        {
                            Debug.LogError("key repeat: " + ac.path);
                        }
                        aft.assetCells.Add(ac.path, ac);
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            return aft;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Asset file load error " + e.Message);
            return null;
        }

    }

    public bool SaveToFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (StreamWriter sw = new StreamWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                sw.WriteLine(version);
                foreach (var ac in assetCells.Values)
                {
                    sw.WriteLine(ac.ToString());
                }
                sw.Flush();
                sw.Close();
                return true;
            }
        }
        catch (System.Exception e)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Debug.LogError("asset save manifest file error" + e);
            return false;
        }
    }

    public void SetAlwaysFile(string path)
    {
        alwaysFileStream = File.Open(path, FileMode.Append);
        streamWriter = new StreamWriter(alwaysFileStream);
    }

    public void AddAssetAndSave(AssetCell ac)
    {
        assetCells.Add(ac.path, ac);
        streamWriter.WriteLine(ac.ToString());
        streamWriter.Flush();
    }

    public void AddAsset(AssetCell ac)
    {
        assetCells.Add(ac.path, ac);
    }

    public static void Compare(AssetFileTool local, AssetFileTool remote, out List<AssetCell> unnecessary,
        out List<AssetCell> lack, out List<AssetCell> unCorrect)
    {
        unnecessary = new List<AssetCell>();
        lack = new List<AssetCell>();
        unCorrect = new List<AssetCell>();
        if (local == null || local.assetCells.Count == 0)
        {
            foreach (var item in remote.assetCells)
            {
                lack.Add(item.Value);
            }
            return;
        }
        foreach (var rkvp in remote.assetCells)
        {
            if (local.assetCells.ContainsKey(rkvp.Key))
            {
                if (local.assetCells[rkvp.Key].hashCode != rkvp.Value.hashCode)
                {
                    unCorrect.Add(rkvp.Value);
                }
            }
            else
            {
                lack.Add(rkvp.Value);
            }
        }
        foreach (var lkvp in local.assetCells)
        {
            if (!remote.assetCells.ContainsKey(lkvp.Key))
            {
                unnecessary.Add(lkvp.Value);
            }
        }
    }

    public static void Compare(AssetFileTool local, AssetFileTool remote, out List<AssetCell> unnecessary,
       out List<AssetCell> lack, out List<AssetCell> unCorrect, Dictionary<int, List<AssetCell>> expandAssetDic)
    {
        unnecessary = new List<AssetCell>();
        lack = new List<AssetCell>();
        unCorrect = new List<AssetCell>();
        if (local == null || local.assetCells.Count == 0)
        {
            foreach (var item in remote.assetCells)
            {
                lack.Add(item.Value);
            }
            return;
        }
        foreach (var rkvp in remote.assetCells)
        {
            if (rkvp.Value.loadLevel > 0)
            {
                if (expandAssetDic.ContainsKey(rkvp.Value.loadLevel))
                {
                    expandAssetDic[rkvp.Value.loadLevel].Add(rkvp.Value);
                }
                else
                {
                    List<AssetCell> list = new List<AssetCell>();
                    list.Add(rkvp.Value);
                    expandAssetDic.Add(rkvp.Value.loadLevel, list);
                }
            }
            if (local.assetCells.ContainsKey(rkvp.Key))
            {
                if (local.assetCells[rkvp.Key].hashCode != rkvp.Value.hashCode)
                {
                    unCorrect.Add(rkvp.Value);
                }
            }
            else
            {
                lack.Add(rkvp.Value);
            }
        }
        foreach (var lkvp in local.assetCells)
        {
            if (!remote.assetCells.ContainsKey(lkvp.Key))
            {
                unnecessary.Add(lkvp.Value);
            }
        }
    }

#if UNITY_EDITOR
    public static AssetFileTool CreateAssetFileByManifest(AssetBundleManifest manifest, int version, string baseAdd, AssetCell mainAc)
    {
        AssetFileTool aft = new AssetFileTool();
        aft.version = version;
        aft.AddAsset(mainAc);
        string[] asets = manifest.GetAllAssetBundles();
        for (int i = 0; i < asets.Length; ++i)
        {
            FileInfo fi = new FileInfo(baseAdd + asets[i]);
            //Hash128 h = manifest.GetAssetBundleHash(asets[i]);
            string hashCode = GetMD5HashFromFile(baseAdd + asets[i]);
            AssetCell ac = new AssetCell(asets[i], hashCode.ToString(), fi.Length);
            aft.AddAsset(ac);
        }
        return aft;
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
        catch (System.Exception ex)
        {
            throw new System.Exception("GetMD5HashFromFile() fail, error:" + ex.Message);
        }
    }
#endif



}
