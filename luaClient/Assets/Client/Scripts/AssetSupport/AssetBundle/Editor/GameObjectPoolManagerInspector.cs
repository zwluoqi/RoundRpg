using UnityEditor;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(GameObjectPoolManager), true)]
public class GameObjectPoolManagerInspector : Editor {


	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();


		GameObjectPoolManager rm = target as GameObjectPoolManager;

		EditorGUILayout.LabelField("GameObjectPoolManager -> resourcePools total count: " + rm.resourcePools.Count + "\n");
		foreach (KeyValuePair<string, AssetCounter> obj in rm.resourcePools)
		{
			EditorGUILayout.TextField(obj.Key.ToString(), obj.Value.count.ToString());
		}



		EditorGUILayout.LabelField("GameObjectPoolManager -> pools total count: " + rm.gameObjectPools.Count + "\n");
		foreach (KeyValuePair<int, GameObjectPool> obj in rm.gameObjectPools)
		{
			EditorGUILayout.TextField(obj.Key.ToString(), obj.Value.ToString());
		}

	}
}
