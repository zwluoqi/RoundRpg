using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 对单一原始资源（Prefab）进化多次实例化之后生成的众多复刻对象，进行记录
/// </summary>
public class GameObjectPool
{
    //原始资源Prefab的InstanceID，正整数
    public int id = -1;

    //原始资源路径名
//    private string prefabName;

    //原始资源未实例化前的Prefab
    private GameObject prefabAsset;

    //所有被实例化出的对象的记录
	private List<GameObject> allObjects = new List<GameObject>();

    //已经被实例化，但未被获取走的对象列表
	private List<GameObject> unActiveObjects = new List<GameObject>();

    //最大存储数量
    private int maxAvailableCount = 10;

    public GameObjectPool(GameObject obj, int num, int id)
    {
//        prefabName = name;
        prefabAsset = obj;
        this.id = id;
        PrePopulate(num);
    }

	private void PrePopulate(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(prefabAsset);
            obj.AddComponent<GameObjectPoolId>().id = id;
            unActiveObjects.Add(obj);
            allObjects.Add(obj);
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// 获取已实例化的对象，如果没有，实例化一份
    /// </summary>
    public GameObject Spawn()
    {
        GameObject spawnObj = null;

        while (unActiveObjects.Count > 0)
        {
            spawnObj = unActiveObjects[0];
            unActiveObjects.RemoveAt(0);
            if (spawnObj != null)
            {
                spawnObj.SetActive(true);
                break;
            }
        }

        if (spawnObj == null)
        {
            spawnObj = (GameObject)GameObject.Instantiate(prefabAsset);
            spawnObj.AddComponent<GameObjectPoolId>().id = id;
            allObjects.Add(spawnObj);
            spawnObj.SetActive(true);
        }

        return spawnObj;
    }

    /// <summary>
    /// 归还对象，放回available列表
    /// </summary>
    public void Unspawn(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        obj.SetActive(false);

        if (unActiveObjects.Count <= maxAvailableCount && !unActiveObjects.Contains(obj))
        {
            unActiveObjects.Add(obj);
        }
        else
        {
            UnspawnAndDestory(obj);
        }
    }

    //归还对象，不放回available列表，立刻删除
    public void UnspawnAndDestory(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (allObjects.Contains(obj))
        {
            allObjects.Remove(obj);
        }

        if (unActiveObjects.Contains(obj))
        {
            unActiveObjects.Remove(obj);
        }

        obj.SetActive(false);
        GameObject.Destroy(obj);
    }

    public void UnspawnAll()
    {
        foreach (GameObject obj in allObjects)
        {
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }
        allObjects.Clear();
    }

    public void HideInHierarchy(Transform t)
    {
        foreach (GameObject obj in unActiveObjects)
        {
            if (obj != null)
            {
				obj.transform.SetParent (t);
			}
        }
    }

    public GameObject GetPrefab()
    {
        return prefabAsset;
    }

    //把归还的对象，删除
    public void DestoryAvailable()
    {
        while (unActiveObjects.Count > 0)
        {
            GameObject obj = unActiveObjects[0];
            unActiveObjects.RemoveAt(0);
            if (obj != null)
            {
                if (allObjects.Contains(obj))
                {
                    allObjects.Remove(obj);
                }
				Debug.Log("Destory obj from pool: " + prefabAsset.name);
                GameObject.Destroy(obj);
            }
        }
    }

    public int TotalCount
    {
        get
        {
			return allObjects.Count;
        }
    }

    public int AvailableCount
    {
        get
        {
            return unActiveObjects.Count;
        }
    }

    public override string ToString()
    {
		return string.Format("Name:{0} Total:{1} unActiveObjects:{2}", prefabAsset.name, allObjects.Count, unActiveObjects.Count);
    }
}

