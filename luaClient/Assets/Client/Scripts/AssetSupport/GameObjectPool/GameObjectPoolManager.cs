using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 对象池管理器
/// </summary>
public class GameObjectPoolManager : MonoBehaviour
{
    public delegate void OnGetGameObjectAsyncDone(string path, GameObject obj);

    /// <summary>
    /// 从原始资源预先实例化出的对象，放在tranCache下面，使用的时候直接从tranCache下面移走
    /// </summary>
    private Transform tranCache;

    /// <summary>
    /// 对象池列表，记录对象ID、对应的原始资源、被复刻的次数、以及复刻出的所有GameObject列表
    /// </summary>
	public Dictionary<int, GameObjectPool> gameObjectPools = new Dictionary<int, GameObjectPool>();

    /// <summary>
    /// resource资源引用列表，记录资源名、对应的原始资源、以及被引用的次数
    /// </summary>
	public Dictionary<string, AssetCounter> resourcePools = new Dictionary<string, AssetCounter>();


        /// <summary>
    /// 阻塞方式获取GameObject对象。如果对象的资源已加载，直接返回；没有提前预加载，使用阻塞加载，并返回。
    /// 包含加载、注册、复制整个过程。
    /// </summary>
    public GameObject GetGameObjectDirect(string path)
    {
        GameObject obj = null;

		Object sourceObj = ResourceManager.Instance.GetResource(path);
        if (sourceObj != null)
        {
            GameObjectPoolManager.Instance.Regist(path,1);
            obj = GameObjectPoolManager.Instance.Spawn(path);
        }
        else
        {
			Object asset = ResourceManager.Instance.LoadResourceBlock(path);
            if (asset != null)
            {
                GameObjectPoolManager.Instance.Regist(path,1);
                obj = GameObjectPoolManager.Instance.Spawn(path);
            }
        }

        return obj;
    }

    /// <summary>
    /// 异步获取资源，在回调函数里面返回资源对象
    /// </summary>
	public void GetGameObjectAsync(string path, Void_UnityEngineObject finishCallback)
    {
		Object sourceObj =  ResourceManager.Instance.GetResource(path);
        if (sourceObj != null)
        {
            GameObjectPoolManager.Instance.Regist(path,1);
            GameObject obj = GameObjectPoolManager.Instance.Spawn(path);
			finishCallback ( obj);
        }
        else
        {
			ResourceManager.Instance.LoadResourceAsync(path, (string resPath,UnityEngine.Object asset) => 
            {
                GameObject obj = null;
                if (asset != null)
                {
                    GameObjectPoolManager.Instance.Regist(path,1);
                    obj = GameObjectPoolManager.Instance.Spawn(path);
                }
                if (finishCallback != null)
                {
                    finishCallback(obj);
                }
            });
        }
    }

    /// <summary>
    /// 查询对象是否已经缓存过
    /// </summary>
    public bool IsGameObjectCached(string path)
    {
        if (resourcePools.ContainsKey(path))
        {
            return true;
        }
			
        return false;
    }
		

    


    #region 基于resourceManager的方法
    /// <summary>
    ///  基于resourceManager,注册一个对象池
    /// </summary>
    /// <param name="resourcePath">资源路径</param>
    public void Regist(string resourcePath, int preNum)
    {
        if (resourcePools.ContainsKey(resourcePath))
        {
            resourcePools[resourcePath].count++;
			CreateGameObjectPool( resourcePools[resourcePath].prefab,preNum);
        }
        else
        {
			Object obj = ResourceManager.Instance.GetResource(resourcePath);
            if (obj == null)
            {
                Debug.LogError(resourcePath + " hasn't loaded before!!!");
                return;
            }
			CreateGameObjectPool(obj as GameObject, preNum);
            resourcePools.Add(resourcePath, new AssetCounter(obj as GameObject, preNum));
        }
    }
    /// <summary>
    /// 使用资源路径生产一个物体
    /// </summary>
    /// <param name="resourcePath">资源路径</param>
    /// <returns>生产的物体</returns>
    public GameObject Spawn(string resourcePath)
    {
        if (resourcePools.ContainsKey(resourcePath))
        {
            return Spawn(resourcePools[resourcePath].prefab);
        }
        else
        {
            Debug.LogError("Pool error! It has not exist the resource pool " + resourcePath);
            return null;
        }
    }
    /// <summary>
    /// 注销resource方式的缓冲池
    /// </summary>
    /// <param name="resourcePath">资源路径</param>
    public void UnRegist(string resourcePath)
    {
        if (resourcePools.ContainsKey(resourcePath))
        {
            if (--resourcePools[resourcePath].count == 0)
            {
                DestoryGameObjectPool(resourcePools[resourcePath].prefab);
                resourcePools.Remove(resourcePath);
            }
        }
    }


    #endregion 基于resourceManager的方法



    #region 基于gameObject的原始方法

    /// <summary>
    /// 建立一个对象池
    /// </summary>
    /// <param name="obj">对象模板</param>
    /// <param name="num">预建立的对象数</param>
	public void CreateGameObjectPool(GameObject obj, int num)
    {
        if (null == obj)
        {
            Debug.LogWarning("error! fail to run GameObjectPoolManager.New( null )");
            return;
        }
        int uid = obj.GetInstanceID();
        bool objExist = gameObjectPools.ContainsKey(uid);

        if (!objExist)
        {
			GameObjectPool newPool = new GameObjectPool( obj, num, uid);
			gameObjectPools[newPool.id] = newPool;
			gameObjectPools[newPool.id].HideInHierarchy(tranCache);
        }
    }

    /// <summary>
    /// 生产一个物体
    /// </summary>
    /// <param name="obj">模板</param>
    /// <returns>生产的物体</returns>
    private GameObject Spawn(GameObject obj)
    {
        int id;
        if (TryGetPoolId(obj, out id))
        {
            return gameObjectPools[id].Spawn();
        }
        else
        {
            Debug.LogError("Object " + obj + " doesnt exsit in ObjectPoolManager List.");
            return null;
        }

    }

    /// <summary>
    /// 回收一个物体
    /// </summary>
    /// <param name="obj">要回收的物体</param>
    public void Unspawn(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        int ID;
        if (TryGetPoolId(obj, out ID))
        {
			obj.transform.SetParent (tranCache);
			gameObjectPools[ID].Unspawn(obj);
        }
        else
        {
			Debug.LogWarning(string.Format("ObjectPoolManager.Unspawn: {0}; pool not Registed!", obj.name));
            Destroy(obj);
        }

    }

    /// <summary>
    /// 延迟回收物体
    /// </summary>
    /// <param name="obj">要回收的物体</param>
    /// <param name="duration">延迟时间</param>
    public void Unspawn(GameObject obj, float duration)
    {
        if (duration > 0)
        {
            StartCoroutine(UnspawnTimed(obj, duration));
        }
        else
        {
            if (obj != null)
            {
                Unspawn(obj);
            }
        }
    }

    private IEnumerator UnspawnTimed(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (obj != null)
        {
            Unspawn(obj);
        }
    }

    //回收并立刻销毁
    public void UnspawnAndDestory(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        int ID;
        if (TryGetPoolId(obj, out ID))
        {
			obj.transform.SetParent (tranCache);
            gameObjectPools[ID].UnspawnAndDestory(obj);
        }
        else
        {
            Debug.LogWarning(string.Format("ObjectPoolManager.UnspawnAndDestory: {0}; pool not Registed!", obj.name));
            Destroy(obj);
        }
    }

    //延时回收并销毁
    public void UnspawnAndDestory(GameObject obj, float duration)
    {
        if (duration > 0)
        {
            StartCoroutine(UnspawnAndDestoryTimed(obj, duration));
        }
        else
        {
            if (obj != null)
            {
                UnspawnAndDestory(obj);
            }
        }
    }

    private IEnumerator UnspawnAndDestoryTimed(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (obj != null)
        {
            UnspawnAndDestory(obj);
        }
    }
  
    /// <summary>
    /// 清空一个对象池
    /// </summary>
    /// <param name="gObj"></param>
    public void DestoryGameObjectPool(GameObject gObj)
    {
        int id;
        if (TryGetPoolId(gObj, out id))
        {
            gameObjectPools[id].UnspawnAll();
            gameObjectPools.Remove(id);
        }
    }

    /// <summary>
    /// 获取一个对象的对象池id
    /// </summary>
    /// <param name="obj">物体</param>
    /// <param name="zhenYanId">返回对象池id</param>
    /// <returns>物体池是否存在</returns>
    public bool TryGetPoolId(GameObject obj, out int id)
    {
        if (obj == null)
        {
            id = 0;
            return false;
        }
        GameObjectPoolId oid = obj.GetComponent<GameObjectPoolId>();
        if (oid != null)
        {
            id = oid.id;
        }
        else
        {
            id = obj.GetInstanceID();
        }
        return gameObjectPools.ContainsKey(id);
    }


    #endregion 基于gameObject的原始方法


    #region Tools


    public void ReducePoolSize()
    {
        Debug.LogFormat("Total gameObjectPools count = {0}, ReducePoolSize!", gameObjectPools.Count);
        foreach(KeyValuePair<int, GameObjectPool> entry in gameObjectPools)
        {
            entry.Value.DestoryAvailable();
        }
    }

    #endregion


    #region 单例
    private void Awake()
    {
        var obj = new GameObject("cache");
        obj.SetActive(false);
        GameObject.DontDestroyOnLoad(obj);
        tranCache = obj.transform;
    }

    private static GameObjectPoolManager instance;
    public static GameObjectPoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameObjectPoolManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameObjectPoolManager");
                    GameObject.DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<GameObjectPoolManager>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// 清空所有物体
    /// </summary>
    public void Release()
    {
        resourcePools.Clear();
        foreach (GameObjectPool pool in gameObjectPools.Values)
        {
            pool.UnspawnAll();
        }
        gameObjectPools.Clear();
    }
    #endregion 单例
}


/// <summary>
/// 原始资源实例化计数器
/// </summary>
public class AssetCounter
{
    //原始资源未实例化前的Prefab
    public GameObject prefab;

    //原始资源被实例化的次数
    public int count;

    public AssetCounter(GameObject prefab, int count)
    {
        this.prefab = prefab;
        this.count = count;
    }
}

