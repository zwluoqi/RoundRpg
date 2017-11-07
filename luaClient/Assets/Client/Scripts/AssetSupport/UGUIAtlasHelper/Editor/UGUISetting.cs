//----------------------------------------------
//            UGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Unity doesn't keep the values of static variables after scripts change get recompiled. One way around this
/// is to store the references in EditorPrefs -- retrieve them at start, and save them whenever something changes.
/// </summary>

public class UGUISettings
{
	public enum ColorMode
	{
		Orange,
		Green,
		Blue,
	}

	#region Generic Get and Set methods
	/// <summary>
	/// Save the specified boolean value in settings.
	/// </summary>

	static public void SetBool (string name, bool val) { EditorPrefs.SetBool(name, val); }

	/// <summary>
	/// Save the specified integer value in settings.
	/// </summary>

	static public void SetInt (string name, int val) { EditorPrefs.SetInt(name, val); }

	/// <summary>
	/// Save the specified float value in settings.
	/// </summary>

	static public void SetFloat (string name, float val) { EditorPrefs.SetFloat(name, val); }

	/// <summary>
	/// Save the specified string value in settings.
	/// </summary>

	static public void SetString (string name, string val) { EditorPrefs.SetString(name, val); }

	/// <summary>
	/// Save the specified color value in settings.
	/// </summary>

	static public void SetColor (string name, Color c) { SetString(name, c.r + " " + c.g + " " + c.b + " " + c.a); }

	/// <summary>
	/// Save the specified enum value to settings.
	/// </summary>

	static public void SetEnum (string name, System.Enum val) { SetString(name, val.ToString()); }

	/// <summary>
	/// Save the specified object in settings.
	/// </summary>

	static public void Set (string name, Object obj)
	{
		if (obj == null)
		{
			EditorPrefs.DeleteKey(name);
		}
		else
		{
			if (obj != null)
			{
				string path = AssetDatabase.GetAssetPath(obj);

				if (!string.IsNullOrEmpty(path))
				{
					EditorPrefs.SetString(name, path);
				}
				else
				{
					EditorPrefs.SetString(name, obj.GetInstanceID().ToString());
				}
			}
			else EditorPrefs.DeleteKey(name);
		}
	}

	/// <summary>
	/// Get the previously saved boolean value.
	/// </summary>

	static public bool GetBool (string name, bool defaultValue) { return EditorPrefs.GetBool(name, defaultValue); }

	/// <summary>
	/// Get the previously saved integer value.
	/// </summary>

	static public int GetInt (string name, int defaultValue) { return EditorPrefs.GetInt(name, defaultValue); }

	/// <summary>
	/// Get the previously saved float value.
	/// </summary>

	static public float GetFloat (string name, float defaultValue) { return EditorPrefs.GetFloat(name, defaultValue); }

	/// <summary>
	/// Get the previously saved string value.
	/// </summary>

	static public string GetString (string name, string defaultValue) { return EditorPrefs.GetString(name, defaultValue); }

	/// <summary>
	/// Get a previously saved color value.
	/// </summary>

	static public Color GetColor (string name, Color c)
	{
		string strVal = GetString(name, c.r + " " + c.g + " " + c.b + " " + c.a);
		string[] parts = strVal.Split(' ');

		if (parts.Length == 4)
		{
			float.TryParse(parts[0], out c.r);
			float.TryParse(parts[1], out c.g);
			float.TryParse(parts[2], out c.b);
			float.TryParse(parts[3], out c.a);
		}
		return c;
	}

	/// <summary>
	/// Get a previously saved enum from settings.
	/// </summary>

	static public T GetEnum<T> (string name, T defaultValue)
	{
		string val = GetString(name, defaultValue.ToString());
		string[] names = System.Enum.GetNames(typeof(T));
		System.Array values = System.Enum.GetValues(typeof(T));

		for (int i = 0; i < names.Length; ++i)
		{
			if (names[i] == val)
				return (T)values.GetValue(i);
		}
		return defaultValue;
	}

	/// <summary>
	/// Get a previously saved object from settings.
	/// </summary>

	static public T Get<T> (string name, T defaultValue) where T : Object
	{
		string path = EditorPrefs.GetString(name);
		if (string.IsNullOrEmpty(path)) return null;

		T retVal = UGUIEditorTools.LoadAsset<T>(path);

		if (retVal == null)
		{
			int id;
			if (int.TryParse(path, out id))
				return EditorUtility.InstanceIDToObject(id) as T;
		}
		return retVal;
	}
	#endregion

	#region Convenience accessor properties

	static public bool showTransformHandles
	{
		get { return GetBool("UGUI Transform Handles", false); }
		set { SetBool("UGUI Transform Handles", value); }
	}

	static public bool minimalisticLook
	{
		get { return GetBool("UGUI Minimalistic", false); }
		set { SetBool("UGUI Minimalistic", value); }
	}

	static public bool unifiedTransform
	{
		get { return GetBool("UGUI Unified", false); }
		set { SetBool("UGUI Unified", value); }
	}

	static public Color color
	{
		get { return GetColor("UGUI Color", Color.white); }
		set { SetColor("UGUI Color", value); }
	}

	static public Color foregroundColor
	{
		get { return GetColor("UGUI FG Color", Color.white); }
		set { SetColor("UGUI FG Color", value); }
	}

	static public Color backgroundColor
	{
		get { return GetColor("UGUI BG Color", Color.black); }
		set { SetColor("UGUI BG Color", value); }
	}

	static public ColorMode colorMode
	{
		get { return GetEnum("UGUI Color Mode", ColorMode.Blue); }
		set { SetEnum("UGUI Color Mode", value); }
	}

//	static public Object ambigiousFont
//	{
//		get
//		{
//			Font fnt = Get<Font>("UGUI Dynamic Font", null);
//			if (fnt != null) return fnt;
//			return Get<UIFont>("UGUI Bitmap Font", null);
//		}
//		set
//		{
//			if (value == null)
//			{
//				Set("UGUI Bitmap Font", null);
//				Set("UGUI Dynamic Font", null);
//			}
//			else if (value is Font)
//			{
//				Set("UGUI Bitmap Font", null);
//				Set("UGUI Dynamic Font", value as Font);
//			}
//			else if (value is UIFont)
//			{
//				Set("UGUI Bitmap Font", value as UIFont);
//				Set("UGUI Dynamic Font", null);
//			}
//		}
//	}

	static public UGUIAtlas atlas
	{
		get { return Get<UGUIAtlas>("UGUI Atlas", null); }
		set { Set("UGUI Atlas", value); }
	}

	static public Texture texture
	{
		get { return Get<Texture>("UGUI Texture", null); }
		set { Set("UGUI Texture", value); }
	}

	static public Sprite sprite2D
	{
		get { return Get<Sprite>("UGUI Sprite2D", null); }
		set { Set("UGUI Sprite2D", value); }
	}

	static public string selectedSprite
	{
		get { return GetString("UGUI Sprite", null); }
		set { SetString("UGUI Sprite", value); }
	}
		

	static public int layer
	{
		get
		{
			int layer = GetInt("UGUI Layer", -1);
			if (layer == -1) layer = LayerMask.NameToLayer("UI");
			if (layer == -1) layer = LayerMask.NameToLayer("2D UI");
			return (layer == -1) ? 9 : layer;
		}
		set
		{
			SetInt("UGUI Layer", value);
		}
	}

	static public TextAsset fontData
	{
		get { return Get<TextAsset>("UGUI Font Data", null); }
		set { Set("UGUI Font Data", value); }
	}

	static public Texture2D fontTexture
	{
		get { return Get<Texture2D>("UGUI Font Texture", null); }
		set { Set("UGUI Font Texture", value); }
	}

	static public int fontSize
	{
		get { return GetInt("UGUI Font Size", 16); }
		set { SetInt("UGUI Font Size", value); }
	}

	static public int FMSize
	{
		get { return GetInt("UGUI FM Size", 16); }
		set { SetInt("UGUI FM Size", value); }
	}

	static public bool fontKerning
	{
		get { return GetBool("UGUI Font Kerning", true); }
		set { SetBool("UGUI Font Kerning", value); }
	}

	static public FontStyle fontStyle
	{
		get { return GetEnum("UGUI Font Style", FontStyle.Normal); }
		set { SetEnum("UGUI Font Style", value); }
	}

	static public Font dynamicFont
	{
		get { return Get<Font>("UGUI Dynamic Font", null); }
		set { Set("UGUI Dynamic Font", value); }
	}

	static public Font FMFont
	{
		get { return Get<Font>("UGUI FM Font", null); }
		set { Set("UGUI FM Font", value); }
	}

//	static public UIFont BMFont
//	{
//		get { return Get<UIFont>("UGUI BM Font", null); }
//		set { Set("UGUI BM Font", value); }
//	}
//
//	static public UILabel.Overflow overflowStyle
//	{
//		get { return GetEnum("UGUI Overflow", UILabel.Overflow.ShrinkContent); }
//		set { SetEnum("UGUI Overflow", value); }
//	}

	static public string partialSprite
	{
		get { return GetString("UGUI Partial", null); }
		set { SetString("UGUI Partial", value); }
	}

	static public int atlasPadding
	{
		get { return GetInt("UGUI Padding", 1); }
		set { SetInt("UGUI Padding", value); }
	}

	static public bool atlasTrimming
	{
		get { return GetBool("UGUI Trim", true); }
		set { SetBool("UGUI Trim", value); }
	}

	static public bool atlasPMA
	{
		get { return GetBool("UGUI PMA", false); }
		set { SetBool("UGUI PMA", value); }
	}

	static public bool unityPacking
	{
		get { return GetBool("UGUI Packing", true); }
		set { SetBool("UGUI Packing", value); }
	}

	static public bool trueColorAtlas
	{
		get { return GetBool("UGUI Truecolor", true); }
		set { SetBool("UGUI Truecolor", value); }
	}

	static public bool keepPadding
	{
		get { return GetBool("UGUI KeepPadding", false); }
		set { SetBool("UGUI KeepPadding", value); }
	}

	static public bool forceSquareAtlas
	{
		get { return GetBool("UGUI Square", false); }
		set { SetBool("UGUI Square", value); }
	}

	static public bool allow4096
	{
		get { return GetBool("UGUI 4096", true); }
		set { SetBool("UGUI 4096", value); }
	}

	static public bool showAllDCs
	{
		get { return GetBool("UGUI DCs", true); }
		set { SetBool("UGUI DCs", value); }
	}

	static public bool drawGuides
	{
		get { return GetBool("UGUI Guides", false); }
		set { SetBool("UGUI Guides", value); }
	}

	static public string charsToInclude
	{
		get { return GetString("UGUI Chars", ""); }
		set { SetString("UGUI Chars", value); }
	}

	#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
	static public string pathToFreeType
	{
	get
	{
	string path = Application.dataPath;
	if (Application.platform == RuntimePlatform.WindowsEditor) path += "/UGUI/Editor/FreeType.dll";
	else path += "/UGUI/Editor/FreeType.dylib";
	return GetString("UGUI FreeType", path);
	}
	set { SetString("UGUI FreeType", value); }
	}
	#else
	static public string pathToFreeType
	{
		get
		{
			string path = Application.dataPath;
			if (Application.platform == RuntimePlatform.WindowsEditor) path += "/UGUI/Editor/FreeType64.dll";
			else path += "/UGUI/Editor/FreeType64.dylib";
			return GetString("UGUI FreeType64", path);
		}
		set { SetString("UGUI FreeType64", value); }
	}
	#endif

	static public string searchField
	{
		get { return GetString("UGUI Search", null); }
		set { SetString("UGUI Search", value); }
	}

	static public string currentPath
	{
		get { return GetString("UGUI Path", "Assets/"); }
		set { SetString("UGUI Path", value); }
	}
	#endregion

	/// <summary>
	/// Convenience method -- add a widget.
	/// </summary>

//	static public UIWidget AddWidget (GameObject go)
//	{
//		UIWidget w = UGUITools.AddWidget<UIWidget>(go);
//		w.name = "Container";
//		w.pivot = pivot;
//		w.width = 100;
//		w.height = 100;
//		return w;
//	}

	/// <summary>
	/// Convenience method -- add a texture.
	/// </summary>

//	static public RawImage AddTexture (GameObject go)
//	{
//		RawImage w = UGUITools.AddWidget<RawImage>(go);
//		w.name = "Texture";
//		w.pivot = pivot;
//		w.mainTexture = texture;
//		w.width = 100;
//		w.height = 100;
//		return w;
//	}

	/// <summary>
	/// Convenience method -- add a UnityEngine.Sprite.
	/// </summary>

//	static public UI2DSprite Add2DSprite (GameObject go)
//	{
//		UI2DSprite w = UGUITools.AddWidget<UI2DSprite>(go);
//		w.name = "2D Sprite";
//		w.pivot = pivot;
//		w.sprite2D = sprite2D;
//		w.width = 100;
//		w.height = 100;
//		return w;
//	}

	/// <summary>
	/// Convenience method -- add a sprite.
	/// </summary>

//	static public UISprite AddSprite (GameObject go)
//	{
//		UISprite w = UGUITools.AddWidget<UISprite>(go);
//		w.name = "Sprite";
//		w.atlas = atlas;
//		w.spriteName = selectedSprite;
//
//		if (w.atlas != null && !string.IsNullOrEmpty(w.spriteName))
//		{
//			UISpriteData sp = w.atlas.GetSprite(w.spriteName);
//			if (sp != null && sp.hasBorder)
//				w.type = UISprite.Type.Sliced;
//		}
//
//		w.pivot = pivot;
//		w.width = 100;
//		w.height = 100;
//		w.MakePixelPerfect();
//		return w;
//	}

	/// <summary>
	/// Convenience method -- add a label with default parameters.
	/// </summary>

//	static public UILabel AddLabel (GameObject go)
//	{
//		UILabel w = UGUITools.AddWidget<UILabel>(go);
//		w.name = "Label";
//		w.ambigiousFont = ambigiousFont;
//		w.text = "New Label";
//		w.pivot = pivot;
//		w.width = 120;
//		w.height = Mathf.Max(20, GetInt("UGUI Font Height", 16));
//		w.fontStyle = fontStyle;
//		w.fontSize = fontSize;
//		w.applyGradient = true;
//		w.gradientBottom = new Color(0.7f, 0.7f, 0.7f);
//		w.AssumeNaturalSize();
//		return w;
//	}

	/// <summary>
	/// Convenience method -- add a new panel.
	/// </summary>

//	static public UIPanel AddPanel (GameObject go)
//	{
//		if (go == null) return null;
//		int depth = UIPanel.nextUnusedDepth;
//		UIPanel panel = UGUITools.AddChild<UIPanel>(go);
//		panel.depth = depth;
//		return panel;
//	}

	/// <summary>
	/// Copy the specified widget's parameters.
	/// </summary>

//	static public void CopyWidget (UIWidget widget)
//	{
//		SetInt("Width", widget.width);
//		SetInt("Height", widget.height);
//		SetInt("Depth", widget.depth);
//		SetColor("Widget Color", widget.color);
//		SetEnum("Widget Pivot", widget.pivot);
//
//		if (widget is UISprite) CopySprite(widget as UISprite);
//		else if (widget is UILabel) CopyLabel(widget as UILabel);
//	}

	/// <summary>
	/// Paste the specified widget's style.
	/// </summary>

//	static public void PasteWidget (UIWidget widget, bool fully)
//	{
//		widget.color = GetColor("Widget Color", widget.color);
//		widget.pivot = GetEnum<UIWidget.Pivot>("Widget Pivot", widget.pivot);
//
//		if (fully)
//		{
//			widget.width = GetInt("Width", widget.width);
//			widget.height = GetInt("Height", widget.height);
//			widget.depth = GetInt("Depth", widget.depth);
//		}
//
//		if (widget is UISprite) PasteSprite(widget as UISprite, fully);
//		else if (widget is UILabel) PasteLabel(widget as UILabel, fully);
//	}

	/// <summary>
	/// Copy the specified sprite's style.
	/// </summary>

//	static void CopySprite (UISprite sp)
//	{
//		SetString("Atlas", UGUIEditorTools.ObjectToGUID(sp.atlas));
//		SetString("Sprite", sp.spriteName);
//		SetEnum("Sprite Type", sp.type);
//		SetEnum("Left Type", sp.leftType);
//		SetEnum("Right Type", sp.rightType);
//		SetEnum("Top Type", sp.topType);
//		SetEnum("Bottom Type", sp.bottomType);
//		SetEnum("Center Type", sp.centerType);
//		SetFloat("Fill", sp.fillAmount);
//		SetEnum("FDir", sp.fillDirection);
//	}

	/// <summary>
	/// Copy the specified label's style.
	/// </summary>

//	static void CopyLabel (UILabel lbl)
//	{
//		SetString("Font", UGUIEditorTools.ObjectToGUID(lbl.ambigiousFont));
//		SetInt("Font Size", lbl.fontSize);
//		SetEnum("Font Style", lbl.fontStyle);
//		SetEnum("Overflow", lbl.overflowMethod);
//		SetBool("UseFloatSpacing", lbl.useFloatSpacing);
//		SetFloat("FloatSpacingX", lbl.floatSpacingX);
//		SetFloat("FloatSpacingY", lbl.floatSpacingY);
//		SetInt("SpacingX", lbl.spacingX);
//		SetInt("SpacingY", lbl.spacingY);
//		SetInt("MaxLines", lbl.maxLineCount);
//		SetBool("Encoding", lbl.supportEncoding);
//		SetBool("Gradient", lbl.applyGradient);
//		SetColor("Gradient B", lbl.gradientBottom);
//		SetColor("Gradient T", lbl.gradientTop);
//		SetEnum("Effect", lbl.effectStyle);
//		SetColor("Effect C", lbl.effectColor);
//		SetFloat("Effect X", lbl.effectDistance.x);
//		SetFloat("Effect Y", lbl.effectDistance.y);
//	}

	/// <summary>
	/// Paste the specified sprite's style.
	/// </summary>

//	static void PasteSprite (UISprite sp, bool fully)
//	{
//		if (fully) sp.atlas = UGUIEditorTools.GUIDToObject<UGUIAtlas>(GetString("Atlas", null));
//		sp.spriteName = GetString("Sprite", sp.spriteName);
//		sp.type = GetEnum<UISprite.Type>("Sprite Type", sp.type);
//		sp.leftType = GetEnum<UISprite.AdvancedType>("Left Type", UISprite.AdvancedType.Sliced);
//		sp.rightType = GetEnum<UISprite.AdvancedType>("Right Type", UISprite.AdvancedType.Sliced);
//		sp.topType = GetEnum<UISprite.AdvancedType>("Top Type", UISprite.AdvancedType.Sliced);
//		sp.bottomType = GetEnum<UISprite.AdvancedType>("Bottom Type", UISprite.AdvancedType.Sliced);
//		sp.centerType = GetEnum<UISprite.AdvancedType>("Center Type", UISprite.AdvancedType.Sliced);
//		sp.fillAmount = GetFloat("Fill", sp.fillAmount);
//		sp.fillDirection = GetEnum<UISprite.FillDirection>("FDir", sp.fillDirection);
//		UGUITools.SetDirty(sp);
//	}

	/// <summary>
	/// Paste the specified label's style.
	/// </summary>

//	static void PasteLabel (UILabel lbl, bool fully)
//	{
//		if (fully)
//		{
//			Object obj = UGUIEditorTools.GUIDToObject(GetString("Font", null));
//
//			if (obj != null)
//			{
//				if (obj.GetType() == typeof(Font))
//				{
//					lbl.ambigiousFont = obj as Font;
//				}
//				else if (obj.GetType() == typeof(GameObject))
//				{
//					lbl.ambigiousFont = (obj as GameObject).GetComponent<UIFont>();
//				}
//			}
//			lbl.fontSize = GetInt("Font Size", lbl.fontSize);
//			lbl.fontStyle = GetEnum<FontStyle>("Font Style", lbl.fontStyle);
//		}
//
//		lbl.overflowMethod = GetEnum<UILabel.Overflow>("Overflow", lbl.overflowMethod);
//		lbl.useFloatSpacing = GetBool("UseFloatSpacing", lbl.useFloatSpacing);
//		lbl.floatSpacingX = GetFloat("FloatSpacingX", lbl.floatSpacingX);
//		lbl.floatSpacingY = GetFloat("FloatSpacingY", lbl.floatSpacingY);
//		lbl.spacingX = GetInt("SpacingX", lbl.spacingX);
//		lbl.spacingY = GetInt("SpacingY", lbl.spacingY);
//		lbl.maxLineCount = GetInt("MaxLines", lbl.maxLineCount);
//		lbl.supportEncoding = GetBool("Encoding", lbl.supportEncoding);
//		lbl.applyGradient = GetBool("Gradient", lbl.applyGradient);
//		lbl.gradientBottom = GetColor("Gradient B", lbl.gradientBottom);
//		lbl.gradientTop = GetColor("Gradient T", lbl.gradientTop);
//		lbl.effectStyle = GetEnum<UILabel.Effect>("Effect", lbl.effectStyle);
//		lbl.effectColor = GetColor("Effect C", lbl.effectColor);
//
//		float x = GetFloat("Effect X", lbl.effectDistance.x);
//		float y = GetFloat("Effect Y", lbl.effectDistance.y);
//		lbl.effectDistance = new Vector2(x, y);
//		UGUITools.SetDirty(lbl);
//	}
}
