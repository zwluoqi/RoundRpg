using UnityEngine;
using UnityEditor;
using System.Collections;
using SimpleSpritePacker;
using NUnit.Framework;
using System.Collections.Generic;

namespace SimpleSpritePackerEditor
{
	public class SPDropWindow : EditorWindow
	{
		private static Color green = new Color(0.345f, 0.625f, 0.370f, 1f);


		private SPInstance m_Instance;
		
		[MenuItem ("Window/Simple Sprite Packer/Drag and Drop Window")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(SPDropWindow));
		}
		
		protected void OnEnable()
		{
			this.titleContent = new GUIContent("SP Drop");
			
			if (EditorPrefs.HasKey(SPTools.Settings_SavedInstanceIDKey))
			{
				string instancePath = AssetDatabase.GetAssetPath(EditorPrefs.GetInt(SPTools.Settings_SavedInstanceIDKey, 0));
				
				if (!string.IsNullOrEmpty(instancePath))
				{
					this.m_Instance = AssetDatabase.LoadAssetAtPath(instancePath, typeof(SPInstance)) as SPInstance;
				}
			}
		}
		
		protected void OnGUI()
		{
			EditorGUIUtility.labelWidth = 100f;
			
			GUILayout.BeginVertical();
			GUILayout.Space(8f);
			
			GUI.changed = false;
			this.m_Instance = EditorGUILayout.ObjectField("Sprite Packer", this.m_Instance, typeof(SPInstance), false) as SPInstance;
			if (GUI.changed)
			{
				// Save the instance id
				EditorPrefs.SetInt(SPTools.Settings_SavedInstanceIDKey, (this.m_Instance == null) ? 0 : this.m_Instance.GetInstanceID());
			}
			
			GUILayout.Space(4f);
			
			if (this.m_Instance == null)
			{
				EditorGUILayout.HelpBox("Please set the sprite packer instance reference in order to use this feature.", MessageType.Info);
			}
			else
			{
				Event evt = Event.current;

				Rect add_area = GUILayoutUtility.GetRect(200, 100);
				GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
				boxStyle.alignment = TextAnchor.UpperCenter;
				GUI.color = SPDropWindow.green;
				GUI.Box(add_area, "Add Sprite (Drop Here)", boxStyle);
				GUI.color = Color.white;


				Rect delete_area = GUILayoutUtility.GetRect(200, 100);
				boxStyle.alignment = TextAnchor.LowerCenter;
				GUI.color = SPDropWindow.green;
				GUI.Box(delete_area, "Delete Sprite (Drop Here)", boxStyle);
				GUI.color = Color.white;


				switch (evt.type)
				{
					case EventType.DragUpdated:
					case EventType.DragPerform:
					{
						if (add_area.Contains (evt.mousePosition)) {
						
							DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						
							if (evt.type == EventType.DragPerform) {
								DragAndDrop.AcceptDrag ();
							
								Object[] filtered = SPTools.FilterResourcesForAtlasImport (DragAndDrop.objectReferences);


								List<Object> updateList = new List<Object> ();
								// Disallow miltiple sprites of the same source, if set so
								if (!SPTools.GetEditorPrefBool (SPTools.Settings_AllowMuliSpritesOneSource)) {

								}


								// Additional filtering specific to the instance
								for (int i = 0; i < filtered.Length; i++) {
									if (this.m_Instance.sprites.Find (s => s.name == filtered [i].name) != null) {
										Debug.LogWarning ("A sprite with source \"" + SimpleSpritePackerEditor.SPTools.GetAssetPath (filtered [i]) + "\" already exists in the atlas,will update.");
										updateList.Add (filtered [i]);
										System.Array.Clear (filtered, i, 1);
									}
								}
							
								// Types are handled internally
								this.m_Instance.QueueAction_AddSprites (filtered);

								this.m_Instance.QueueAction_UpdateSprites (updateList.ToArray ());
						
								Selection.activeObject = this.m_Instance;
								//EditorPrefs.SetBool(SPTools.Settings_ShowSpritesKeys, false);
							}
						} else if (delete_area.Contains (evt.mousePosition)) {
							DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

							if (evt.type == EventType.DragPerform) {
								DragAndDrop.AcceptDrag ();

								Object[] filtered = SPTools.FilterResourcesForAtlasImport (DragAndDrop.objectReferences);


								// Disallow miltiple sprites of the same source, if set so
								if (!SPTools.GetEditorPrefBool (SPTools.Settings_AllowMuliSpritesOneSource)) {

								}


								// Additional filtering specific to the instance
								for (int i = 0; i < filtered.Length; i++) {
									if (this.m_Instance.sprites.Find (s => s.name == filtered [i].name) == null) {
										Debug.LogWarning ("A sprite not with source \"" + SimpleSpritePackerEditor.SPTools.GetAssetPath (filtered [i]) + "\" already exists in the atlas, will delete.");
										System.Array.Clear (filtered, i, 1);
									}
								}

								// Types are handled internally
								this.m_Instance.QueueAction_DeleteSprites (filtered);

//								this.m_Instance.QueueAction_UpdateSprites (updateList.ToArray ());

								Selection.activeObject = this.m_Instance;
								//EditorPrefs.SetBool(SPTools.Settings_ShowSpritesKeys, false);
							}
						}
						break;
					}
				}
			}
			
			GUILayout.EndVertical();
		}
	}
}