//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Editor component used to display a list of sprites.
/// </summary>

public class SpriteSelector : ScriptableWizard
{
	static public SpriteSelector instance;

	void OnEnable () { instance = this; }
	void OnDisable () { instance = null; }

	public delegate void Callback (string sprite);

	SerializedObject mObject;
	SerializedProperty mProperty;

	UGUISprite mSprite;
	Vector2 mPos = Vector2.zero;
	Callback mCallback;
	float mClickTime = 0f;

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		EditorGUIUtility.labelWidth = 80;

		if (UGUISettings.atlas == null)
		{
			GUILayout.Label("No Atlas selected.", "LODLevelNotifyText");
		}
		else
		{
			UGUIAtlas atlas = UGUISettings.atlas;
			bool close = false;
			GUILayout.Label(atlas.name + " Sprites", "LODLevelNotifyText");
			UGUIEditorTools.DrawSeparator();

			GUILayout.BeginHorizontal();
			GUILayout.Space(84f);

			string before = UGUISettings.partialSprite;
			string after = EditorGUILayout.TextField("", before, "SearchTextField");
			if (before != after) UGUISettings.partialSprite = after;

			if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
			{
				UGUISettings.partialSprite = "";
				GUIUtility.keyboardControl = 0;
			}
			GUILayout.Space(84f);
			GUILayout.EndHorizontal();

//			Texture2D tex = atlas.mainTexture as Texture2D;
//
//			if (tex == null)
//			{
//				GUILayout.Label("The atlas doesn't have a texture to work with");
//				return;
//			}

			BetterList<string> sprites = atlas.GetListOfSprites(UGUISettings.partialSprite);

			float size = 80f;
			float padded = size + 10f;

			int columns = Mathf.FloorToInt(Screen.width / padded)/2;
			if (columns < 1) columns = 1;
//			Debug.LogWarning (columns);

			int offset = 0;
			Rect rect = new Rect(10f, 0, size, size);

			GUILayout.Space(10f);
			mPos = GUILayout.BeginScrollView(mPos);
			int rows = 1;

			while (offset < sprites.size)
			{
				GUILayout.BeginHorizontal();
				{
					int col = 0;
					rect.x = 10f;

					for (; offset < sprites.size; ++offset)
					{
						Sprite sprite = atlas.GetSprite(sprites[offset]);
						if (sprite == null) continue;

						// Button comes first
						if (GUI.Button(rect, ""))
						{
							if (Event.current.button == 0)
							{
								float delta = Time.realtimeSinceStartup - mClickTime;
								mClickTime = Time.realtimeSinceStartup;

								if (UGUISettings.selectedSprite != sprite.name)
								{
									if (mSprite != null)
									{
										UGUIEditorTools.RegisterUndo("Atlas Selection", mSprite);
										mSprite.SetNativeSize();
										EditorUtility.SetDirty(mSprite.gameObject);
									}

									UGUISettings.selectedSprite = sprite.name;
									UGUIEditorTools.RepaintSprites();
									if (mCallback != null) mCallback(sprite.name);
								}
								else if (delta < 0.5f) close = true;
							}
							else
							{
//								NGUIContextMenu.AddItem("Edit", false, EditSprite, sprite);
//								NGUIContextMenu.AddItem("Delete", false, DeleteSprite, sprite);
//								NGUIContextMenu.Show();
							}
						}

						if (Event.current.type == EventType.Repaint)
						{
							// On top of the button we have a checkboard grid
							UGUIEditorTools.DrawTiledTexture(rect, UGUIEditorTools.backdropTexture);
//							Rect uv = new Rect(sprite.rect.x, sprite.rect.y, sprite.rect.width, sprite.rect.height);
//							uv = UGUIMath.ConvertToTexCoords(uv, tex.width, tex.height);

							// Calculate the texture's scale that's needed to display the sprite in the clipped area
//							float scaleX = rect.width / uv.width;
//							float scaleY = rect.height / uv.height;

							// Stretch the sprite so that it will appear proper
//							float aspect = (scaleY / scaleX) / ((float)tex.height / tex.width);
//							Rect clipRect = rect;
//
//							if (aspect != 1f)
//							{
//								if (aspect < 1f)
//								{
//									// The sprite is taller than it is wider
//									float padding = size * (1f - aspect) * 0.5f;
//									clipRect.xMin += padding;
//									clipRect.xMax -= padding;
//								}
//								else
//								{
//									// The sprite is wider than it is taller
//									float padding = size * (1f - 1f / aspect) * 0.5f;
//									clipRect.yMin += padding;
//									clipRect.yMax -= padding;
//								}
//							}

							GUI.DrawTexture (rect, sprite.texture);

							// Draw the selection
							if (UGUISettings.selectedSprite == sprite.name)
							{
								UGUIEditorTools.DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
							}
						}

						GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
						GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
						GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), sprite.name, "ProgressBarBack");
						GUI.contentColor = Color.white;
						GUI.backgroundColor = Color.white;

						if (++col >= columns)
						{
							++offset;
							break;
						}
						rect.x += padded;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(padded);
				rect.y += padded + 26;
				++rows;
			}
			GUILayout.Space(rows * 26);
			GUILayout.EndScrollView();

			if (close) Close();
		}
	}

	/// <summary>
	/// Edit the sprite (context menu selection)
	/// </summary>

//	void EditSprite (object obj)
//	{
//		if (this == null) return;
//		Sprite sd = obj as Sprite;
//		UGUIEditorTools.SelectSprite(sd.name);
//		Close();
//	}

	/// <summary>
	/// Delete the sprite (context menu selection)
	/// </summary>

//	void DeleteSprite (object obj)
//	{
//		if (this == null) return;
//		Sprite sd = obj as Sprite;
//
//		List<UGUIAtlasMaker.SpriteEntry> sprites = new List<UGUIAtlasMaker.SpriteEntry>();
//		UGUIAtlasMaker.ExtractSprites(UGUISettings.atlas, sprites);
//
//		for (int i = sprites.Count; i > 0; )
//		{
//			UGUIAtlasMaker.SpriteEntry ent = sprites[--i];
//			if (ent.name == sd.name)
//				sprites.RemoveAt(i);
//		}
//		UGUIAtlasMaker.UpdateAtlas(UGUISettings.atlas, sprites);
//		UGUIEditorTools.RepaintSprites();
//	}

	/// <summary>
	/// Property-based selection result.
	/// </summary>

	void OnSpriteSelection (string sp)
	{
		if (mObject != null && mProperty != null)
		{
			mObject.Update();
			mProperty.stringValue = sp;
			mObject.ApplyModifiedProperties();
		}
	}

	/// <summary>
	/// Show the sprite selection wizard.
	/// </summary>

	static public void ShowSelected ()
	{
		if (UGUISettings.atlas != null)
		{
			Show(delegate(string sel) { UGUIEditorTools.SelectSprite(sel); });
		}
	}

	/// <summary>
	/// Show the sprite selection wizard.
	/// </summary>

	static public void Show (SerializedObject ob, SerializedProperty pro, UGUIAtlas atlas)
	{
		if (instance != null)
		{
			instance.Close();
			instance = null;
		}

		if (ob != null && pro != null && atlas != null)
		{
			SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("Select a Sprite");
			UGUISettings.atlas = atlas;
			UGUISettings.selectedSprite = pro.hasMultipleDifferentValues ? null : pro.stringValue;
			comp.mSprite = null;
			comp.mObject = ob;
			comp.mProperty = pro;
			comp.mCallback = comp.OnSpriteSelection;
		}
	}

	/// <summary>
	/// Show the selection wizard.
	/// </summary>

	static public void Show (Callback callback)
	{
		if (instance != null)
		{
			instance.Close();
			instance = null;
		}

		SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("Select a Sprite");
		comp.mSprite = null;
		comp.mCallback = callback;
	}
}
