using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour {



	public void LoadScene(string sceneName,Void_String callback){
		SceneManager.LoadScene (sceneName);
		if (callback != null) {
			callback (sceneName);
		}
	}

	public void LoadAsync(string sceneName,Void_String callback ){
		StartCoroutine(LoadAsync0(sceneName,callback));
	}

	IEnumerator LoadAsync0(string sceneName,Void_String callback ){
		yield return SceneManager.LoadSceneAsync (sceneName);
		if (callback != null) {
			callback (sceneName);
		}
	}



	private static SceneLoadManager instance;
	public static SceneLoadManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<SceneLoadManager>();
				if (instance == null)
				{
					GameObject obj = new GameObject("SceneLoadManager");
					GameObject.DontDestroyOnLoad(obj);
					instance = obj.AddComponent<SceneLoadManager>();
				}
			}
			return instance;
		}
	}
}
