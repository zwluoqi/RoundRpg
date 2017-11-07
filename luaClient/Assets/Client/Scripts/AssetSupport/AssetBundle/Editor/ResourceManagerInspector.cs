using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ResourceManager), true)]
public class ResourceManagerInspector:Editor
{
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();


		ResourceManager rm = target as ResourceManager;

		EditorGUILayout.LabelField(@"----------------------Resource内部资源加载情况：");
		foreach (var obj in rm.innerResoruceManager.assetContainer) {


			EditorGUILayout.TextField(obj.Key.ToString(), obj.Value.count.ToString());

		}

		EditorGUILayout.LabelField(@"----------------------Static外部资源加载情况：");
		EditorGUILayout.LabelField(@"----------------------Bundle：");
		foreach (var obj in rm.staticAssetBundleManager.bundleDicts) {
			EditorGUILayout.TextField(obj.Key.ToString(), obj.Value.count.ToString());
		}
		EditorGUILayout.LabelField(@"----------------------AssetObject：");
		foreach (var obj in rm.staticAssetBundleManager.assetDicts) {
			EditorGUILayout.TextField(obj.Key.ToString(), obj.Value.count.ToString());
		}

		EditorGUILayout.LabelField(@"----------------------Dynamic外部资源加载情况：");
		EditorGUILayout.LabelField(@"----------------------Bundle：");
		foreach (var obj in rm.dynamicAssetBundleManager.bundleDicts) {
			EditorGUILayout.TextField(obj.Key.ToString(), obj.Value.count.ToString());
		}
		EditorGUILayout.LabelField(@"----------------------AssetObject：");
		foreach (var obj in rm.dynamicAssetBundleManager.assetDicts) {
			EditorGUILayout.TextField(obj.Key.ToString(), obj.Value.count.ToString());
		}
	}

}
