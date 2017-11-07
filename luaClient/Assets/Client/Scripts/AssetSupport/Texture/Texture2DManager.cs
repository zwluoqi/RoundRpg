using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Texture2DManager
{
    #region 单例模式
    //
    private static Texture2DManager m_instance;

    //
    public static Texture2DManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new Texture2DManager();
                m_instance.Init();
            }

            return m_instance;
        }
    }

    public static void Release()
    {
        if(m_instance != null)
        {
            m_instance.Destory();
        }
        m_instance = null;
    }

    #endregion

    #region TextureCell
    public class TextureCell
    {
        public Texture2D texture;
        
        //
        public float latestUseTime;
        public List<RawImage> usedList;

        //
        public TextureCell(Texture2D texture)
        {
            this.texture = texture;

            usedList = new List<RawImage>();
        }

        // 使用的次数
        public int UsedCount
        {
            get
            {
                return usedList.Count;
            }
        }

        public void AddUsedUITexture(RawImage usedUITexture)
        {
            latestUseTime = Time.realtimeSinceStartup;
            usedList.Add(usedUITexture);
        }

        public bool IsUnUsedTextureCell()
        {
            return UsedCount <= 0;
        }

		public void AutoClearNullObject(){
			for(int i = usedList.Count - 1; i >= 0; i--)
			{
				if(usedList[i] == null)
				{
					usedList.RemoveAt(i);
				}
			}
		}
    }

    #endregion


    // TEXTURE_SIZE
    public const int TEXTURE_SIZE = 128;

    // 默认缓存大小
    public const int CACHE_MAX_SIZE = 64; //
    private int cache_max_size = CACHE_MAX_SIZE;
    
    //
    private Dictionary<string, TextureCell> m_texture_cache_dic;// 纹理缓存
    private List<string> orderKey;

    //
    private void Init()
    {
        //
        m_texture_cache_dic = new Dictionary<string, TextureCell>();
        orderKey = new List<string>();

        //
		cache_max_size = CACHE_MAX_SIZE;
    }

    private void Destory()
    {

    }

    /// <summary>
    ///  创建纹理,纹理格式根据画质高低自动选择
    ///  texture_path格式需为bytes格式
    ///  低画质：  使用压缩格式
    ///  中高画质: 使用真彩色
    /// </summary>
    public Texture2D CreateTextureAuto(RawImage usedUITexture, string texture_path, out bool isNewCreate, bool alpha = true,int textureSize = TEXTURE_SIZE)
    {
        return CreateTextureByImage(usedUITexture, texture_path, out isNewCreate);
    }

    /// <summary>
    /// 由一张图片创建一个纹理
    /// </summary>
    public Texture2D CreateTextureByImage(RawImage usedUITexture, string texture_path, out bool isNewCreate)
    {
        Texture2D texture2D = null;
        isNewCreate = false;

        // 缓存中获取
        texture2D = GetTexture2DFormCache(texture_path);
        if (texture2D != null)
        {
            // 处理缓存
            AddCache(usedUITexture, texture_path, texture2D);
            return texture2D;
        }

        //
		texture2D = ResourceManager.Instance.LoadResourceBlock(texture_path)as Texture2D;
        if (texture2D != null)
        {
            isNewCreate = true;
            //Resources.UnloadAsset(texture2D);

            // 处理缓存
            AddCache(usedUITexture, texture_path, texture2D);
        }
        else
        {
            Debug.LogError("Texture2D not found:" + texture_path);
        }

        return texture2D;
    }

    #region 缓存管理

    /// <summary>
    /// 清理所有缓存
    /// </summary>
    public void CleanAllCache()
    {
//         foreach (TextureCell tex in m_texture_cache_dic.Values)
//         {
//             GameObject.Destroy(tex.texture);
//         }
        m_texture_cache_dic.Clear();
        orderKey.Clear();
    }


    /// <summary>
    /// 清除所有不用的缓存
    /// </summary>
    public void ClearUnusedAllCache()
    {
        for (int i = orderKey.Count - 1; i >= 0; --i)
        {
            string key = orderKey[i];
            if (m_texture_cache_dic.ContainsKey(key) )
            {
				m_texture_cache_dic [key].AutoClearNullObject ();
				if (m_texture_cache_dic [key].IsUnUsedTextureCell ()) {
					//GameObject.Destroy(m_texture_cache_dic[key].texture);
					orderKey.RemoveAt (i);
					m_texture_cache_dic.Remove (key);
				}
            }
        }
    }

    

    /// <summary>
    /// 根据排序，删除末尾的Texture
    /// </summary>
    private void AutoClearCache()
    {
        for (int i = orderKey.Count - 1; i >= 0; --i)
        {
            string key = orderKey[i];
            if (m_texture_cache_dic.ContainsKey(key))
            {
				m_texture_cache_dic [key].AutoClearNullObject ();
				if (m_texture_cache_dic [key].IsUnUsedTextureCell ()) {
					//GameObject.Destroy(m_texture_cache_dic[key].texture);
					orderKey.RemoveAt (i);
					m_texture_cache_dic.Remove (key);
				}
            }

            //
            if (orderKey.Count <= cache_max_size)
            {
                break;
            }
        }
    }

    // FIFO排序
    private void AutoSortKeyFIFO(string texture_key)
    {
        //
        if (orderKey.Contains(texture_key))
        {
            orderKey.Remove(texture_key);
        }

        orderKey.Insert(0, texture_key);
    }

    //
    private void AddCache(RawImage usedUITexture, string texture_path, Texture2D texture)
    {
        //
        string key = GetTexture2DKey(texture_path);

        if (m_texture_cache_dic.ContainsKey(key))
        {
            m_texture_cache_dic[key].AddUsedUITexture(usedUITexture);
        }
        else
        {
            m_texture_cache_dic[key] = new TextureCell(texture);
			m_texture_cache_dic[key].AddUsedUITexture(usedUITexture);
        }

        //
        AutoSortKeyFIFO(key);

        //
        if (m_texture_cache_dic.Count > cache_max_size)
        {
            AutoClearCache();
        }
    }

    //
    private Texture2D GetTexture2DFormCache(string texture_path)
    {
        // key值
        string key = GetTexture2DKey(texture_path);

        //
        if (m_texture_cache_dic.ContainsKey(key))
        {
            return m_texture_cache_dic[key].texture;
        }
        else
        {
            return null;
        }
    }

    private string GetTexture2DKey(string texture_path)
    {
        return texture_path;
    }

    #endregion
}
