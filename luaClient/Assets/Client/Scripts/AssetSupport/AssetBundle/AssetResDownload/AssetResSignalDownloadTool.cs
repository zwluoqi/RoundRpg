//using System;
//using System.Threading;
//using UnityEngine;
//using System.IO;
//using System.Net;
//using System.Collections;
//
//public class AssetResSignalDownloadTool
//{
//
//	public AssetResDownloadTool parentAssetResDownLoadTool;
//
//	public AssetResSignalDownloadTool (AssetResDownloadTool _parentAssetResDownLoadTool,AssetCell _ac)
//	{
//
//		this.parentAssetResDownLoadTool = _parentAssetResDownLoadTool;
//		this.ac = _ac;
//	}
//
//
//
//	public string serverPath;
//	public string localPath;
//	public AssetCell ac;
//
//
//	public long loadedSize = 0;
//	private long webLoadSize = 0;
//	public Thread downloadThread;
//	private float unscaledTime = 0;
//
//	public void Tick(){
//		unscaledTime = Time.unscaledTime;
//	}
//
//	public DownLoadState downloadState = DownLoadState.NONE;
//	public enum DownLoadState{
//		NONE,
//		DOWNLOADING,
//		SUCESS,
//		FAILED,
//		TIMEOUT,
//	}
//
//	long GetLength(string _fileUrl)
//	{
//		//Debug.LogError("GetLength = " + _fileUrl);
//		HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(_fileUrl);
//		if (request != null)
//		{
//			request.Method = "HEAD";
//			HttpWebResponse res = (HttpWebResponse)request.GetResponse();
//			if (res != null)
//			{
//				//Debug.LogError("res.ContentLength=" + res.ContentLength);
//				return res.ContentLength;
//			}
//			Debug.LogError("res is null! _fileUrl=" + _fileUrl);
//		}
//		Debug.LogError("request连接错误！ _fileUrl=" + _fileUrl);
//		return 0;
//	}
//
//	public void StartHttpDownLoad(){
//		downloadThread = new Thread(() =>
//			{
//				downloadState = DownLoadState.DOWNLOADING;
//				string toPath = parentAssetResDownLoadTool.localTmpSavePath + "/" + ac.path;
//				string webUrl = parentAssetResDownLoadTool.remoteServerPath + "/" + ac.path;
//				long loadCounter = 0;
//				Debug.Log("开始下载资源,ac.path =" + ac.path + " toPath=" + toPath + "    webUrl=" + webUrl);
//				FileUtils.CreateDir(Path.GetDirectoryName (toPath));
//				FileStream fileStream = new FileStream(toPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
//				long fileLength = fileStream.Length;//当前已下载的文件长度
//				long totalLength = GetLength(webUrl);//从服务器获取的文件长度
//				//Debug.LogError("totalLength=" + totalLength + "    fileLength=" + fileStream.Length);
//				if (fileLength < totalLength)
//				{
//					Stream httpStream = null;
//					try
//					{
//						HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(webUrl);
//						request.AddRange((int)fileLength);
//						HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//						fileStream.Seek(fileLength, SeekOrigin.Begin);
//						httpStream = response.GetResponseStream();
//						byte[] buffer = new byte[1024 * 10];
//						int length = httpStream.Read(buffer, 0, buffer.Length);
//						Debug.Log("read length=" + length);
//						float pretime = unscaledTime;
//						float prePercent = 0;
//						while (length > 0)
//						{
//
//							fileStream.Write(buffer, 0, length);
//							fileLength += length;
//							loadedSize += length;
//							webLoadSize += length;
//
//							if (fileLength != prePercent)
//							{
//								pretime = unscaledTime;
//								prePercent = fileLength;
//							}
//							else
//							{
//								if (unscaledTime - pretime > 60)
//								{
//									Debug.LogError("下载超时：" + ac.path );
//									downloadState = DownLoadState.TIMEOUT;
//									break;
//								}
//							}
//							loadCounter++;
//							Thread.Sleep(20);
//
//							length = httpStream.Read(buffer, 0, buffer.Length);
//						}
//					}
//					catch (Exception ex)
//					{
//						Debug.LogError(ac.path + "下载中断，收到错误信息：" + ex.Message);
//						downloadState = DownLoadState.FAILED;
//					}
//					finally
//					{
//						if (httpStream != null)
//						{
//							fileStream.Flush();
//							fileStream.Close();
//							fileStream.Dispose();
//
//							httpStream.Close();
//							httpStream.Dispose();
//						}
//					}
//				}
//				else
//				{
//					//progress = fileLength / totalLength * 100;//进度
//				}
//				Debug.Log("fileLength=" + fileLength.ToString() + " totalLength=" + totalLength.ToString());
//				if (fileLength >= totalLength)
//				{
//					Debug.Log("加入移动列表：" + ac.path);
//					downloadState = DownLoadState.SUCESS;
//				}
//			});
//		downloadThread.IsBackground = true;
//		downloadThread.Start();
//	}
//
//	public IEnumerator StartWWWDownLoad(){
//		downloadState = DownLoadState.DOWNLOADING;
//
//		WWW www = new WWW(parentAssetResDownLoadTool.remoteServerPath + "/" + ac.path);
//		float prePercent = 0;
//		float pretime = Time.unscaledTime;
//
//		yield return www;
//
//		if (www.error != null)
//		{
//			//download failure
//			downloadState = DownLoadState.FAILED;
//			Debug.LogError("下载超时：" + parentAssetResDownLoadTool.remoteServerPath + "/" + ac.path + "   www.error=" + www.error);
//			www.Dispose();
//		}
//		else if (www.isDone)
//		{
//			FileUtils.WriteAllBytes(parentAssetResDownLoadTool.remoteServerPath + "/" + ac.path, www.bytes);
//			www.Dispose();
//			loadedSize += ac.size;
//			webLoadSize += ac.size;
//			Debug.Log(ac.path + "下载完成！");
//			downloadState = DownLoadState.SUCESS;
//		}
//		else
//		{
//			Debug.LogError("下载超时：" + parentAssetResDownLoadTool.remoteServerPath + "/" + ac.path );
//			downloadState = DownLoadState.TIMEOUT;
//		}
//	}
//
//
//
//	public void Destroy(){
//		if (downloadState == DownLoadState.DOWNLOADING) {
//			downloadThread.Abort ();
//		}
//		downloadState = DownLoadState.NONE;
//	}
//
//
//	public void MoveCoverage()
//	{
//		string source = parentAssetResDownLoadTool.localTmpSavePath + "/" + ac.path; 
//		string dest = parentAssetResDownLoadTool.localSavePath + "/" + ac.path;
//		if (File.Exists(dest))
//		{
//			File.Delete(dest);
//		}
//
//		FileUtils.CreateDir(Path.GetDirectoryName (dest));
//
//		if (File.Exists(source))
//		{
//			File.Move(source, dest);
//		}
//	}
//}
