//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Inspector class used to edit the UGUIAtlas.
/// </summary>

[CustomEditor(typeof(UGUIAtlas))]
public class UGUIAtlasInspector : Editor
{
	static public UGUIAtlasInspector instance;

	enum AtlasType
	{
		Normal,
		Reference,
	}

	UGUIAtlas mAtlas;
	AtlasType mType = AtlasType.Normal;
	UGUIAtlas mReplacement = null;

	void OnEnable () { instance = this; }
	void OnDisable () { instance = null; }

	/// <summary>
	/// Convenience function -- mark all widgets using the sprite as changed.
	/// </summary>

	void MarkSpriteAsDirty ()
	{
//		Sprite sprite = (mAtlas != null) ? mAtlas.GetSprite(UGUISettings.selectedSprite) : null;
//		if (sprite == null) return;
//
//		Image[] sprites = UGUITools.FindActive<Image>();
//
//		foreach (Image sp in sprites)
//		{
//			if (UGUIAtlas.CheckIfRelated(sp.atlas, mAtlas) && sp.spriteName == sprite.name)
//			{
//				UGUIAtlas atl = sp.atlas;
//				sp.atlas = null;
//				sp.atlas = atl;
//				EditorUtility.SetDirty(sp);
//			}
//		}
//
//		UILabel[] labels = UGUITools.FindActive<UILabel>();
//
//		foreach (UILabel lbl in labels)
//		{
//			if (lbl.bitmapFont != null && UGUIAtlas.CheckIfRelated(lbl.bitmapFont.atlas, mAtlas) && lbl.bitmapFont.UsesSprite(sprite.name))
//			{
//				UIFont font = lbl.bitmapFont;
//				lbl.bitmapFont = null;
//				lbl.bitmapFont = font;
//				EditorUtility.SetDirty(lbl);
//			}
//		}
	}

	/// <summary>
	/// Replacement atlas selection callback.
	/// </summary>

	void OnSelectAtlas (Object obj)
	{
		if (mReplacement != obj)
		{
			// Undo doesn't work correctly in this case... so I won't bother.
			//UGUIEditorTools.RegisterUndo("Atlas Change");
			//UGUIEditorTools.RegisterUndo("Atlas Change", mAtlas);

			mAtlas.replacement = obj as UGUIAtlas;
			mReplacement = mAtlas.replacement;
			UGUITools.SetDirty(mAtlas);
			if (mReplacement == null) mType = AtlasType.Normal;
		}
	}

	/// <summary>
	/// Draw the inspector widget.
	/// </summary>

	public override void OnInspectorGUI ()
	{
		UGUIEditorTools.SetLabelWidth(80f);
		mAtlas = target as UGUIAtlas;

		Sprite sprite = (mAtlas != null) ? mAtlas.GetSprite(UGUISettings.selectedSprite) : null;

		GUILayout.Space(6f);

		if (mAtlas.replacement != null)
		{
			mType = AtlasType.Reference;
			mReplacement = mAtlas.replacement;
		}

		GUILayout.BeginHorizontal();
		AtlasType after = (AtlasType)EditorGUILayout.EnumPopup("Atlas Type", mType);
		UGUIEditorTools.DrawPadding();
		GUILayout.EndHorizontal();

		if (mType != after)
		{
			if (after == AtlasType.Normal)
			{
				mType = AtlasType.Normal;
				OnSelectAtlas(null);
			}
			else
			{
				mType = AtlasType.Reference;
			}
		}

		if (mType == AtlasType.Reference)
		{
			ComponentSelector.Draw<UGUIAtlas>(mAtlas.replacement, OnSelectAtlas, true);

			GUILayout.Space(6f);
			EditorGUILayout.HelpBox("You can have one atlas simply point to " +
				"another one. This is useful if you want to be " +
				"able to quickly replace the contents of one " +
				"atlas with another one, for example for " +
				"swapping an SD atlas with an HD one, or " +
				"replacing an English atlas with a Chinese " +
				"one. All the sprites referencing this atlas " +
				"will update their references to the new one.", MessageType.Info);

			if (mReplacement != mAtlas && mAtlas.replacement != mReplacement)
			{
				UGUIEditorTools.RegisterUndo("Atlas Change", mAtlas);
				mAtlas.replacement = mReplacement;
				UGUITools.SetDirty(mAtlas);
			}
			return;
		}

		GUILayout.Space(6f);
//		Material mat = EditorGUILayout.ObjectField("Material", mAtlas.spriteMaterial, typeof(Material), false) as Material;
//
//		if (mAtlas.spriteMaterial != mat)
//		{
//			UGUIEditorTools.RegisterUndo("Atlas Change", mAtlas);
//			mAtlas.spriteMaterial = mat;
//
//			// Ensure that this atlas has valid import settings
//			if (mAtlas.texture != null) UGUIEditorTools.ImportTexture(mAtlas.texture, false, false, !mAtlas.premultipliedAlpha);
//
//			mAtlas.MarkAsChanged();
//		}
//
//		if (mat != null)
//		{
//			TextAsset ta = EditorGUILayout.ObjectField("TP Import", null, typeof(TextAsset), false) as TextAsset;
//
//			if (ta != null)
//			{
//				// Ensure that this atlas has valid import settings
//				if (mAtlas.texture != null) UGUIEditorTools.ImportTexture(mAtlas.texture, false, false, !mAtlas.premultipliedAlpha);
//
//				UGUIEditorTools.RegisterUndo("Import Sprites", mAtlas);
//				NGUIJson.LoadSpriteData(mAtlas, ta);
//				if (sprite != null) sprite = mAtlas.GetSprite(sprite.name);
//				mAtlas.MarkAsChanged();
//			}
//
//			float pixelSize = EditorGUILayout.FloatField("Pixel Size", mAtlas.pixelSize, GUILayout.Width(120f));
//
//			if (pixelSize != mAtlas.pixelSize)
//			{
//				UGUIEditorTools.RegisterUndo("Atlas Change", mAtlas);
//				mAtlas.pixelSize = pixelSize;
//			}
//		}

//		if (mAtlas.mainTexture != null)
		{
			Color blueColor = new Color(0f, 0.7f, 1f, 1f);
			Color greenColor = new Color(0.4f, 1f, 0f, 1f);

			if (sprite == null && mAtlas.spriteList.Count > 0)
			{
				string spriteName = UGUISettings.selectedSprite;
				if (!string.IsNullOrEmpty(spriteName)) sprite = mAtlas.GetSprite(spriteName);
				if (sprite == null) sprite = mAtlas.spriteList[0];
			}

			if (sprite != null)
			{
				if (sprite == null) return;

//				Texture2D tex = mAtlas.mainTexture as Texture2D;

//				if (tex != null)
				{
					if (!UGUIEditorTools.DrawHeader("Sprite Details")) return;

					UGUIEditorTools.BeginContents();

					GUILayout.Space(3f);
					UGUIEditorTools.DrawAdvancedSpriteField(mAtlas, sprite.name, SelectSprite, true);
					GUILayout.Space(6f);

					GUI.changed = false;

					GUI.backgroundColor = greenColor;
					UGUIEditorTools.IntVector sizeA = UGUIEditorTools.IntPair("Dimensions", "X", "Y", (int)sprite.textureRect.x, (int)sprite.textureRect.y);
					UGUIEditorTools.IntVector sizeB = UGUIEditorTools.IntPair(null, "Width", "Height", (int)sprite.textureRect.width, (int)sprite.textureRect.height);

					EditorGUILayout.Separator();
					GUI.backgroundColor = blueColor;
					UGUIEditorTools.IntVector borderA = UGUIEditorTools.IntPair("Border", "Left", "Right", (int)sprite.border.x, (int)sprite.border.y);
					UGUIEditorTools.IntVector borderB = UGUIEditorTools.IntPair(null, "Bottom", "Top", (int)sprite.border.y, (int)sprite.border.z);

					EditorGUILayout.Separator();
					GUI.backgroundColor = Color.white;
					UGUIEditorTools.IntVector padA = UGUIEditorTools.IntPair("Padding", "Left", "Right", (int)sprite.rect.xMin, (int)sprite.rect.xMax);
					UGUIEditorTools.IntVector padB = UGUIEditorTools.IntPair(null, "Bottom", "Top", (int)sprite.rect.yMax, (int)sprite.rect.yMin);

					if (GUI.changed)
					{
//						UGUIEditorTools.RegisterUndo("Atlas Change", mAtlas);
//
//						sprite.rect.x = sizeA.x;
//						sprite.rect.y = sizeA.y;
//						sprite.rect.width = sizeB.x;
//						sprite.rect.height = sizeB.y;
//
//						sprite.paddingLeft = padA.x;
//						sprite.paddingRight = padA.y;
//						sprite.paddingBottom = padB.x;
//						sprite.paddingTop = padB.y;
//
//						sprite.borderLeft = borderA.x;
//						sprite.borderRight = borderA.y;
//						sprite.borderBottom = borderB.x;
//						sprite.borderTop = borderB.y;
//
//						MarkSpriteAsDirty();
					}

					GUILayout.Space(3f);

					GUILayout.BeginHorizontal();

//					if (GUILayout.Button("Duplicate"))
//					{
//						UGUIAtlasMaker.SpriteEntry se = UGUIAtlasMaker.DuplicateSprite(mAtlas, sprite.name);
//						if (se != null) UGUISettings.selectedSprite = se.name;
//					}

//					if (GUILayout.Button("Save As..."))
//					{
//						#if UNITY_3_5
//						string path = EditorUtility.SaveFilePanel("Save As",
//						UGUISettings.currentPath, sprite.name + ".png", "png");
//						#else
//						string path = EditorUtility.SaveFilePanelInProject("Save As",
//							sprite.name + ".png", "png",
//							"Extract sprite into which file?", UGUISettings.currentPath);
//						#endif
//
//						if (!string.IsNullOrEmpty(path))
//						{
//							UGUISettings.currentPath = System.IO.Path.GetDirectoryName(path);
//							UGUIAtlasMaker.SpriteEntry se = UGUIAtlasMaker.ExtractSprite(mAtlas, sprite.name);
//
//							if (se != null)
//							{
//								byte[] bytes = se.tex.EncodeToPNG();
//								File.WriteAllBytes(path, bytes);
//								AssetDatabase.ImportAsset(path);
//								if (se.temporaryTexture) DestroyImmediate(se.tex);
//							}
//						}
//					}
					GUILayout.EndHorizontal();
					UGUIEditorTools.EndContents();
				}

				if (UGUIEditorTools.DrawHeader("Modify"))
				{
					UGUIEditorTools.BeginContents();

					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(20f);
					EditorGUILayout.BeginVertical();

					UGUISettings.backgroundColor = EditorGUILayout.ColorField("Background", UGUISettings.backgroundColor);

//					if (GUILayout.Button("Add a Shadow")) AddShadow(sprite);
//					if (GUILayout.Button("Add a Soft Outline")) AddOutline(sprite);

//					if (GUILayout.Button("Add a Transparent Border")) AddTransparentBorder(sprite);
//					if (GUILayout.Button("Add a Clamped Border")) AddClampedBorder(sprite);
//					if (GUILayout.Button("Add a Tiled Border")) AddTiledBorder(sprite);
//					EditorGUI.BeginDisabledGroup(!sprite.hasBorder);
//					if (GUILayout.Button("Crop Border")) CropBorder(sprite);
//					EditorGUI.EndDisabledGroup();

					EditorGUILayout.EndVertical();
					GUILayout.Space(20f);
					EditorGUILayout.EndHorizontal();

					UGUIEditorTools.EndContents();
				}

				if (UGUIEditorTools.previousSelection != null)
				{
					GUILayout.Space(3f);
					GUI.backgroundColor = Color.green;

					if (GUILayout.Button("<< Return to " + UGUIEditorTools.previousSelection.name))
					{
						UGUIEditorTools.SelectPrevious();
					}
					GUI.backgroundColor = Color.white;
				}
			}
		}
	}

	/// <summary>
	/// Sprite selection callback.
	/// </summary>

	void SelectSprite (string spriteName)
	{
		if (UGUISettings.selectedSprite != spriteName)
		{
			UGUISettings.selectedSprite = spriteName;
			Repaint();
		}
	}

	/// <summary>
	/// All widgets have a preview.
	/// </summary>

	public override bool HasPreviewGUI () { return true; }

	/// <summary>
	/// Draw the sprite preview.
	/// </summary>

//	public override void OnPreviewGUI (Rect rect, GUIStyle background)
//	{
//		Sprite sprite = (mAtlas != null) ? mAtlas.GetSprite(UGUISettings.selectedSprite) : null;
//		if (sprite == null) return;
//
//		Texture2D tex = mAtlas.mainTexture as Texture2D;
//		if (tex != null) UGUIEditorTools.DrawSprite(tex, rect, sprite, Color.white);
//	}

	/// <summary>
	/// Add a transparent border around the sprite.
	/// </summary>

//	void AddTransparentBorder (Sprite sprite)
//	{
//		List<SpriteEntry> sprites = new List<SpriteEntry>();
//		UGUIAtlasMaker.ExtractSprites(mAtlas, sprites);
//		UGUIAtlasMaker.SpriteEntry se = null;
//
//		for (int i = 0; i < sprites.Count; ++i)
//		{
//			if (sprites[i].name == sprite.name)
//			{
//				se = sprites[i];
//				break;
//			}
//		}
//
//		if (se != null)
//		{
//			int w1 = se.tex.width;
//			int h1 = se.tex.height;
//
//			int w2 = w1 + 2;
//			int h2 = h1 + 2;
//
//			Color32[] c2 = UGUIEditorTools.AddBorder(se.tex.GetPixels32(), w1, h1);
//
//			if (se.temporaryTexture) DestroyImmediate(se.tex);
//
//			++se.borderLeft;
//			++se.borderRight;
//			++se.borderTop;
//			++se.borderBottom;
//
//			se.tex = new Texture2D(w2, h2);
//			se.tex.name = sprite.name;
//			se.tex.SetPixels32(c2);
//			se.tex.Apply();
//			se.temporaryTexture = true;
//
//			UGUIAtlasMaker.UpdateAtlas(mAtlas, sprites);
//
//			DestroyImmediate(se.tex);
//			se.tex = null;
//		}
//	}

	/// <summary>
	/// Add a border around the sprite that extends the existing edge pixels.
	/// </summary>

//	void AddClampedBorder (Sprite sprite)
//	{
//		List<UGUIAtlasMaker.SpriteEntry> sprites = new List<UGUIAtlasMaker.SpriteEntry>();
//		UGUIAtlasMaker.ExtractSprites(mAtlas, sprites);
//		UGUIAtlasMaker.SpriteEntry se = null;
//
//		for (int i = 0; i < sprites.Count; ++i)
//		{
//			if (sprites[i].name == sprite.name)
//			{
//				se = sprites[i];
//				break;
//			}
//		}
//
//		if (se != null)
//		{
//			int w1 = se.tex.width - se.borderLeft - se.borderRight;
//			int h1 = se.tex.height - se.borderBottom - se.borderTop;
//
//			int w2 = se.tex.width + 2;
//			int h2 = se.tex.height + 2;
//
//			Color32[] c1 = se.tex.GetPixels32();
//			Color32[] c2 = new Color32[w2 * h2];
//
//			for (int y2 = 0; y2 < h2; ++y2)
//			{
//				int y1 = se.borderBottom + NGUIMath.ClampIndex(y2 - se.borderBottom - 1, h1);
//
//				for (int x2 = 0; x2 < w2; ++x2)
//				{
//					int x1 = se.borderLeft + NGUIMath.ClampIndex(x2 - se.borderLeft - 1, w1);
//					c2[x2 + y2 * w2] = c1[x1 + y1 * se.tex.width];
//				}
//			}
//
//			if (se.temporaryTexture) DestroyImmediate(se.tex);
//
//			++se.borderLeft;
//			++se.borderRight;
//			++se.borderTop;
//			++se.borderBottom;
//
//			se.tex = new Texture2D(w2, h2);
//			se.tex.name = sprite.name;
//			se.tex.SetPixels32(c2);
//			se.tex.Apply();
//			se.temporaryTexture = true;
//
//			UGUIAtlasMaker.UpdateAtlas(mAtlas, sprites);
//
//			DestroyImmediate(se.tex);
//			se.tex = null;
//		}
//	}

	/// <summary>
	/// Add a border around the sprite that copies the pixels from the opposite side, making it possible for the sprite to tile without seams.
	/// </summary>

//	void AddTiledBorder (Sprite sprite)
//	{
//		List<UGUIAtlasMaker.SpriteEntry> sprites = new List<UGUIAtlasMaker.SpriteEntry>();
//		UGUIAtlasMaker.ExtractSprites(mAtlas, sprites);
//		UGUIAtlasMaker.SpriteEntry se = null;
//
//		for (int i = 0; i < sprites.Count; ++i)
//		{
//			if (sprites[i].name == sprite.name)
//			{
//				se = sprites[i];
//				break;
//			}
//		}
//
//		if (se != null)
//		{
//			int w1 = se.tex.width - se.borderLeft - se.borderRight;
//			int h1 = se.tex.height - se.borderBottom - se.borderTop;
//
//			int w2 = se.tex.width + 2;
//			int h2 = se.tex.height + 2;
//
//			Color32[] c1 = se.tex.GetPixels32();
//			Color32[] c2 = new Color32[w2 * h2];
//
//			for (int y2 = 0; y2 < h2; ++y2)
//			{
//				int y1 = se.borderBottom + NGUIMath.RepeatIndex(y2 - se.borderBottom - 1, h1);
//
//				for (int x2 = 0; x2 < w2; ++x2)
//				{
//					int x1 = se.borderLeft + NGUIMath.RepeatIndex(x2 - se.borderLeft - 1, w1);
//					c2[x2 + y2 * w2] = c1[x1 + y1 * se.tex.width];
//				}
//			}
//
//			if (se.temporaryTexture) DestroyImmediate(se.tex);
//
//			++se.borderLeft;
//			++se.borderRight;
//			++se.borderTop;
//			++se.borderBottom;
//
//			se.tex = new Texture2D(w2, h2);
//			se.tex.name = sprite.name;
//			se.tex.SetPixels32(c2);
//			se.tex.Apply();
//			se.temporaryTexture = true;
//
//			UGUIAtlasMaker.UpdateAtlas(mAtlas, sprites);
//
//			DestroyImmediate(se.tex);
//			se.tex = null;
//		}
//	}

	/// <summary>
	/// Crop the border pixels around the sprite.
	/// </summary>

//	void CropBorder (Sprite sprite)
//	{
//		List<UGUIAtlasMaker.SpriteEntry> sprites = new List<UGUIAtlasMaker.SpriteEntry>();
//		UGUIAtlasMaker.ExtractSprites(mAtlas, sprites);
//		UGUIAtlasMaker.SpriteEntry se = null;
//
//		for (int i = 0; i < sprites.Count; ++i)
//		{
//			if (sprites[i].name == sprite.name)
//			{
//				se = sprites[i];
//				break;
//			}
//		}
//
//		if (se != null)
//		{
//			int w1 = se.tex.width;
//			int h1 = se.tex.height;
//
//			int w2 = w1 - se.borderLeft - se.borderRight;
//			int h2 = h1 - se.borderTop - se.borderBottom;
//
//			Color32[] c1 = se.tex.GetPixels32();
//			Color32[] c2 = new Color32[w2 * h2];
//
//			for (int y2 = 0; y2 < h2; ++y2)
//			{
//				int y1 = y2 + se.borderBottom;
//
//				for (int x2 = 0; x2 < w2; ++x2)
//				{
//					int x1 = x2 + se.borderLeft;
//					c2[x2 + y2 * w2] = c1[x1 + y1 * w1];
//				}
//			}
//
//			se.borderLeft = 0;
//			se.borderRight = 0;
//			se.borderTop = 0;
//			se.borderBottom = 0;
//
//			if (se.temporaryTexture) DestroyImmediate(se.tex);
//
//			se.tex = new Texture2D(w2, h2);
//			se.tex.name = sprite.name;
//			se.tex.SetPixels32(c2);
//			se.tex.Apply();
//			se.temporaryTexture = true;
//
//			UGUIAtlasMaker.UpdateAtlas(mAtlas, sprites);
//
//			DestroyImmediate(se.tex);
//			se.tex = null;
//		}
//	}

	/// <summary>
	/// Add a dark shadow below and to the right of the sprite.
	/// </summary>

//	void AddShadow (Sprite sprite)
//	{
//		List<UGUIAtlasMaker.SpriteEntry> sprites = new List<UGUIAtlasMaker.SpriteEntry>();
//		UGUIAtlasMaker.ExtractSprites(mAtlas, sprites);
//		UGUIAtlasMaker.SpriteEntry se = null;
//
//		for (int i = 0; i < sprites.Count; ++i)
//		{
//			if (sprites[i].name == sprite.name)
//			{
//				se = sprites[i];
//				break;
//			}
//		}
//
//		if (se != null)
//		{
//			int w1 = se.tex.width;
//			int h1 = se.tex.height;
//
//			int w2 = w1 + 2;
//			int h2 = h1 + 2;
//
//			Color32[] c2 = UGUIEditorTools.AddBorder(se.tex.GetPixels32(), w1, h1);
//			UGUIEditorTools.AddShadow(c2, w2, h2, UGUISettings.backgroundColor);
//
//			if (se.temporaryTexture) DestroyImmediate(se.tex);
//
//			if ((se.borderLeft | se.borderRight | se.borderBottom | se.borderTop) != 0)
//			{
//				++se.borderLeft;
//				++se.borderRight;
//				++se.borderTop;
//				++se.borderBottom;
//			}
//
//			se.tex = new Texture2D(w2, h2);
//			se.tex.name = sprite.name;
//			se.tex.SetPixels32(c2);
//			se.tex.Apply();
//			se.temporaryTexture = true;
//
//			UGUIAtlasMaker.UpdateAtlas(mAtlas, sprites);
//
//			DestroyImmediate(se.tex);
//			se.tex = null;
//		}
//	}
//
	/// <summary>
	/// Add a dark shadowy outline around the sprite, giving it some visual depth.
	/// </summary>

//	void AddOutline (Sprite sprite)
//	{
//		List<UGUIAtlasMaker.SpriteEntry> sprites = new List<UGUIAtlasMaker.SpriteEntry>();
//		UGUIAtlasMaker.ExtractSprites(mAtlas, sprites);
//		UGUIAtlasMaker.SpriteEntry se = null;
//
//		for (int i = 0; i < sprites.Count; ++i)
//		{
//			if (sprites[i].name == sprite.name)
//			{
//				se = sprites[i];
//				break;
//			}
//		}
//
//		if (se != null)
//		{
//			int w1 = se.tex.width;
//			int h1 = se.tex.height;
//
//			int w2 = w1 + 2;
//			int h2 = h1 + 2;
//
//			Color32[] c2 = UGUIEditorTools.AddBorder(se.tex.GetPixels32(), w1, h1);
//			UGUIEditorTools.AddDepth(c2, w2, h2, UGUISettings.backgroundColor);
//
//			if (se.temporaryTexture) DestroyImmediate(se.tex);
//
//			if ((se.borderLeft | se.borderRight | se.borderBottom | se.borderTop) != 0)
//			{
//				++se.borderLeft;
//				++se.borderRight;
//				++se.borderTop;
//				++se.borderBottom;
//			}
//
//			se.tex = new Texture2D(w2, h2);
//			se.tex.name = sprite.name;
//			se.tex.SetPixels32(c2);
//			se.tex.Apply();
//			se.temporaryTexture = true;
//
//			UGUIAtlasMaker.UpdateAtlas(mAtlas, sprites);
//
//			DestroyImmediate(se.tex);
//			se.tex = null;
//		}
//	}
}
