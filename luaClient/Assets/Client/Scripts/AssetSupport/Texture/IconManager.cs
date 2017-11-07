using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class IconManager:MonoBehaviour
{
    #region 单例模式
    //
    private static IconManager m_instance;

	public static IconManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = GameObject.FindObjectOfType<IconManager>();
				if (m_instance == null)
				{
					GameObject obj = new GameObject("IconManager");
					GameObject.DontDestroyOnLoad(obj);
					m_instance = obj.AddComponent<IconManager>();
					m_instance.Init();

				}
			}
			return m_instance;
		}
	}

    #endregion

    //


    //
    private const int MAX_LOAD_EVERY_FRAME = 5;   // 默认每帧最大加载数

    //
	private int m_maxLoadEveryFrame = MAX_LOAD_EVERY_FRAME;


    private List<LoadTextureCell> m_cellCache = new List<LoadTextureCell>();
    private int m_loadNumber = 0;

    //
    void IconUpdate()
    {
        m_loadNumber = 0;
        while (m_cellCache.Count > 0)
        {
            LoadTextureCell loadTextureCell = m_cellCache[0];
            m_cellCache.RemoveAt(0);

			if (loadTextureCell.iconTexture == null) {
				continue;
			}
            //
            bool isNewCreated;
            SyncSetUITexture(loadTextureCell, out isNewCreated);
            
            //
            if (isNewCreated)
            {
                m_loadNumber++;
            }
            if (m_loadNumber > m_maxLoadEveryFrame)
            {
                break;
            }
        }
    }

    //
    private void Init()
    {
        // 选择渲染的效率
		m_maxLoadEveryFrame = MAX_LOAD_EVERY_FRAME;


        m_loadNumber = 0;

        //
        
    }

	void Update(){
		IconUpdate ();
	}
		


    /// <summary>
    /// 每帧最多进行十次图片加载;
    /// </summary>
    /// <param name="ui_texture"></param>
    /// <param name="path"></param>
    /// <param name="alpha"></param>
    public void SetUITexture(RawImage uiTexture, string path, bool alpha)
    {
        LoadTextureCell loadTextureCell = new LoadTextureCell(uiTexture, path, alpha);

        if (m_loadNumber > m_maxLoadEveryFrame)
        {
            m_cellCache.Add(loadTextureCell);
        }
        else
        {
            bool isNewCreated;
            SyncSetUITexture(loadTextureCell, out isNewCreated);
            if (isNewCreated)
            {
                m_loadNumber++;
            }
        }
    }

    private void SyncSetUITexture(LoadTextureCell loadTextureCell, out bool isNewCreated)
    {
        isNewCreated = false;
        if (loadTextureCell.iconTexture != null)
        {
            Texture2D texure = Texture2DManager.Instance.CreateTextureAuto(loadTextureCell.iconTexture, loadTextureCell.path, out isNewCreated, loadTextureCell.alpha);
			if (texure != loadTextureCell.iconTexture.texture)
            {
				loadTextureCell.iconTexture.texture = texure;

				loadTextureCell.iconTexture.SetNativeSize ();

            }
        }
    }
		
}
