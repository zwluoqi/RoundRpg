using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

public class FileUtils
{
    private static StringBuilder mstrbuilder = new StringBuilder();

    public static string StringBuilder(params object[] args)
    {
        FileUtils.mstrbuilder.Remove(0, FileUtils.mstrbuilder.Length);
        if (args != null)
        {
            int num = args.Length;
            for (int i = 0; i < num; i++)
            {
                FileUtils.mstrbuilder.Append(args[i]);
            }
        }
        return FileUtils.mstrbuilder.ToString();
    }

    public static string GetHttpReqPlus()
    {
        StringBuilder stringBuilder = new StringBuilder();
        long s = 1;
        unchecked
        {
            s = (long)((ulong)(-1294967296L));
        }
        stringBuilder.Append(DateTime.Now.Ticks / s);
        return stringBuilder.ToString();
    }

    public static bool Exists(string fileName)
    {
        return fileName != null && !(fileName.Trim() == string.Empty) && File.Exists(fileName);
    }

    public static string GetStringMd5(string strValue)
    {
        byte[] bytes = Encoding.Default.GetBytes(strValue);
        MD5 mD = MD5.Create();
        byte[] array = mD.ComputeHash(bytes);
        string text = string.Empty;
        for (int i = 0; i < array.Length; i++)
        {
            text += array[i].ToString("X");
        }
        return text;
    }

    public static string GetByteMd5(byte[] bytes)
    {
        MD5 mD = MD5.Create();
        byte[] array = mD.ComputeHash(bytes);
        string text = string.Empty;
        for (int i = 0; i < array.Length; i++)
        {
            text += array[i].ToString("X");
        }
        return text;
    }

    public static bool CreateDir(string dirName)
    {
        if (!Directory.Exists(dirName))
        {
            Directory.CreateDirectory(dirName);
        }
        return true;
    }

    public static bool CreateFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            FileStream fileStream = File.Create(fileName);
            fileStream.Close();
            fileStream.Dispose();
        }
        return true;
    }

    public static string Read(string fileName)
    {
        if (!FileUtils.Exists(fileName))
        {
            return null;
        }
        string result;
        using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
        {
            result = new StreamReader(fileStream).ReadToEnd();
        }
        return result;
    }

    public static byte[] ReadBinary(string fileName)
    {
        if (!FileUtils.Exists(fileName))
        {
            return null;
        }
        byte[] result;
        using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
        {
            byte[] array = new byte[fileStream.Length];
            fileStream.Read(array, 0, array.Length);
            fileStream.Seek(0L, SeekOrigin.Begin);
            result = array;
        }
        return result;
    }

    public static string ReadLine(string fileName)
    {
        if (!FileUtils.Exists(fileName))
        {
            return null;
        }
        string result;
        using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
        {
            result = new StreamReader(fileStream).ReadLine();
        }
        return result;
    }

    public static bool Write(string fileName, string content)
    {
        if (!FileUtils.Exists(fileName) || content == null)
        {
            return false;
        }
        bool result;
        using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
        {
            FileStream obj = fileStream;
            lock (obj)
            {
                if (!fileStream.CanWrite)
                {
                    throw new SecurityException(FileUtils.StringBuilder(new object[]
					{
						"文件fileName=",
						fileName,
						"是只读文件不能写入!"
					}));
                }
                byte[] bytes = Encoding.Default.GetBytes(content);
                fileStream.Write(bytes, 0, bytes.Length);
                result = true;
            }
        }
        return result;
    }

    public static bool WriteBytes(string fileName, byte[] bytes)
    {
        if (!FileUtils.Exists(fileName) || bytes == null)
        {
            return false;
        }
        bool result;
        using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
        {
            FileStream obj = fileStream;
            lock (obj)
            {
                if (!fileStream.CanWrite)
                {
                    throw new SecurityException(FileUtils.StringBuilder(new object[]
					{
						"文件fileName=",
						fileName,
						"是只读文件不能写入!"
					}));
                }
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
                result = true;
            }
        }
        return result;
    }

    public static bool WriteLine(string fileName, string content)
    {
        bool result;
        using (FileStream fileStream = new FileStream(fileName, FileMode.Append))
        {
            FileStream obj = fileStream;
            lock (obj)
            {
                if (!fileStream.CanWrite)
                {
                    throw new SecurityException(FileUtils.StringBuilder(new object[]
					{
						"文件fileName=",
						fileName,
						"是只读文件不能写入!"
					}));
                }
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(content);
                streamWriter.Dispose();
                streamWriter.Close();
                result = true;
            }
        }
        return result;
    }

    public static bool CopyDir(DirectoryInfo fromDir, string toDir)
    {
        return FileUtils.CopyDir(fromDir, toDir, fromDir.FullName);
    }

    public static bool CopyDir(string fromDir, string toDir)
    {
        if (fromDir == null || toDir == null)
        {
            throw new NullReferenceException("参数为空");
        }
        if (fromDir == toDir)
        {
            throw new Exception(FileUtils.StringBuilder(new object[]
			{
				"两个目录都是",
				fromDir
			}));
        }
        if (!Directory.Exists(fromDir))
        {
            throw new IOException(FileUtils.StringBuilder(new object[]
			{
				"目录fromDir=",
				fromDir,
				"不存在"
			}));
        }
        DirectoryInfo directoryInfo = new DirectoryInfo(fromDir);
        return FileUtils.CopyDir(directoryInfo, toDir, directoryInfo.FullName);
    }

    private static bool CopyDir(DirectoryInfo fromDir, string toDir, string rootDir)
    {
        string text = string.Empty;
        FileInfo[] files = fromDir.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo fileInfo = files[i];
            text = toDir + fileInfo.FullName.Substring(rootDir.Length);
            string dirName = text.Substring(0, text.LastIndexOf("/"));
            FileUtils.CreateDir(dirName);
            File.Copy(fileInfo.FullName, text, true);
        }
        DirectoryInfo[] directories = fromDir.GetDirectories();
        for (int j = 0; j < directories.Length; j++)
        {
            DirectoryInfo fromDir2 = directories[j];
            FileUtils.CopyDir(fromDir2, toDir, rootDir);
        }
        return true;
    }

    public static bool DeleteFile(string fileName)
    {
        if (FileUtils.Exists(fileName))
        {
            File.Delete(fileName);
            return true;
        }
        return false;
    }

    public static void DeleteDir(DirectoryInfo dir)
    {
        if (dir == null)
        {
            throw new NullReferenceException("目录不存在");
        }
        DirectoryInfo[] directories = dir.GetDirectories();
        for (int i = 0; i < directories.Length; i++)
        {
            DirectoryInfo dir2 = directories[i];
            FileUtils.DeleteDir(dir2);
        }
        FileInfo[] files = dir.GetFiles();
        for (int j = 0; j < files.Length; j++)
        {
            FileInfo fileInfo = files[j];
            FileUtils.DeleteFile(fileInfo.FullName);
        }
        dir.Delete();
    }

    public static bool DeleteDir(string dir, bool onlyDir)
    {
        if (dir == null || dir.Trim() == string.Empty)
        {
            throw new NullReferenceException("目录dir=" + dir + "不存在");
        }
        if (!Directory.Exists(dir))
        {
            return false;
        }
        DirectoryInfo directoryInfo = new DirectoryInfo(dir);
        if (directoryInfo.GetFiles().Length == 0 && directoryInfo.GetDirectories().Length == 0)
        {
            Directory.Delete(dir);
            return true;
        }
        if (!onlyDir)
        {
            return false;
        }
        FileUtils.DeleteDir(directoryInfo);
        return true;
    }

    public static bool FindFile(string dir, string fileName)
    {
        if (dir == null || dir.Trim() == string.Empty || fileName == null || fileName.Trim() == string.Empty || !Directory.Exists(dir))
        {
            return false;
        }
        DirectoryInfo dir2 = new DirectoryInfo(dir);
        return FileUtils.FindFile(dir2, fileName);
    }

    public static bool FindFile(DirectoryInfo dir, string fileName)
    {
        DirectoryInfo[] directories = dir.GetDirectories();
        for (int i = 0; i < directories.Length; i++)
        {
            DirectoryInfo directoryInfo = directories[i];
            if (File.Exists(FileUtils.StringBuilder(new object[]
			{
				directoryInfo.FullName,
				"/",
				fileName
			})))
            {
                return true;
            }
            FileUtils.FindFile(directoryInfo, fileName);
        }
        return false;
    }

	public static void MoveCoverage(string source,string dest)
	{
		if (!File.Exists (source)) {
			UnityEngine.Debug.LogError ("source not exist:" + source);
		}
		if (File.Exists(dest))
		{
			File.Delete(dest);
		}

		FileUtils.CreateDir(Path.GetDirectoryName (dest));

		if (File.Exists(source))
		{
			File.Move(source, dest);
		}
	}


	public static void WriteAllText(string filePath,string currentVersion){
		string dirPath = Path.GetDirectoryName (filePath);
		if (!Directory.Exists (dirPath)) {
			Directory.CreateDirectory (dirPath);
		}
		File.WriteAllText (filePath, currentVersion);
	}

	public static void WriteAllBytes(string filePath,byte[] data){
		string dirPath = Path.GetDirectoryName (filePath);
		if (!Directory.Exists (dirPath)) {
			Directory.CreateDirectory (dirPath);
		}
		File.WriteAllBytes (filePath, data);
	}
}
