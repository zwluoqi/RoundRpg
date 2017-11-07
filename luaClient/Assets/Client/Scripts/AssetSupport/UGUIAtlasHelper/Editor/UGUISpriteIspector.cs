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

[CustomEditor(typeof(UGUISprite))]
public class UGUISpriteIspector : Editor {


	/// <summary>
	/// Draw the inspector properties.
	/// </summary>

	public override void OnInspectorGUI ()
	{
		UGUIEditorTools.SetLabelWidth(80f);
		EditorGUILayout.Space();

		serializedObject.Update();

		EditorGUI.BeginDisabledGroup(!ShouldDrawProperties());
		DrawCustomProperties();
		EditorGUI.EndDisabledGroup();
		DrawFinalProperties();

		serializedObject.ApplyModifiedProperties();
	}

	public virtual void DrawCustomProperties(){
		UGUISprite uguiSprite = target as UGUISprite;

		EditorGUILayout.ObjectField ("Sprite",uguiSprite.sprite, typeof(Sprite), false);


		uguiSprite.color = EditorGUILayout.ColorField ("Color",uguiSprite.color);
		uguiSprite.material = EditorGUILayout.ObjectField ("Material",uguiSprite.material, typeof(Material), false) as Material;
		uguiSprite.raycastTarget = EditorGUILayout.Toggle ("Raycast Target", uguiSprite.raycastTarget);
		uguiSprite.type = (Image.Type)EditorGUILayout.EnumPopup ("Image Type", uguiSprite.type);

		switch (uguiSprite.type) {
		case Image.Type.Simple:
			uguiSprite.preserveAspect = EditorGUILayout.Toggle ("preserveAspect", uguiSprite.preserveAspect);
			if (GUILayout.Button ("Set Native Size")) {
				uguiSprite.SetNativeSize ();
			}
			break;
		case Image.Type.Sliced:
			uguiSprite.fillCenter = EditorGUILayout.Toggle ("Fill Center", uguiSprite.fillCenter);
			break;
		case Image.Type.Tiled:
			uguiSprite.fillCenter = EditorGUILayout.Toggle ("Fill Center", uguiSprite.fillCenter);
			break;
		case Image.Type.Filled:
			{
				uguiSprite.fillMethod = (Image.FillMethod)EditorGUILayout.EnumPopup ("FillMethod Type", uguiSprite.fillMethod);
				switch (uguiSprite.fillMethod) {
				case Image.FillMethod.Horizontal:
					uguiSprite.fillOrigin = (int)(Image.OriginHorizontal)EditorGUILayout.EnumPopup ("FillMethod Origin", (Image.OriginHorizontal)(uguiSprite.fillOrigin));

					break;
				case Image.FillMethod.Vertical:
					uguiSprite.fillOrigin = (int) (Image.OriginVertical)EditorGUILayout.EnumPopup ("FillMethod Origin", (Image.OriginVertical)(uguiSprite.fillOrigin));

					break;
				case Image.FillMethod.Radial360:
					uguiSprite.fillOrigin = (int) (Image.Origin360)EditorGUILayout.EnumPopup ("FillMethod Origin", (Image.Origin360)(uguiSprite.fillOrigin));

					break;
				case Image.FillMethod.Radial180:
					uguiSprite.fillOrigin = (int)(Image.Origin180)EditorGUILayout.EnumPopup ("FillMethod Origin", (Image.Origin180)(uguiSprite.fillOrigin));

					break;
				case Image.FillMethod.Radial90:
					uguiSprite.fillOrigin = (int)(Image.Origin90)EditorGUILayout.EnumPopup ("FillMethod Origin", (Image.Origin90)(uguiSprite.fillOrigin));

					break;
				}
				uguiSprite.fillAmount = EditorGUILayout.Slider("fillAmount Value ",uguiSprite.fillAmount,0,1);
				uguiSprite.fillClockwise = EditorGUILayout.Toggle ("fill Clockwise", uguiSprite.fillClockwise);;

				uguiSprite.preserveAspect = EditorGUILayout.Toggle ("preserveAspect", uguiSprite.preserveAspect);
			}
			break;
		}


	}

	public virtual void DrawFinalProperties(){

	}
		

	/// <summary>
	/// Atlas selection callback.
	/// </summary>

	void OnSelectAtlas (Object obj)
	{
		serializedObject.Update();
		SerializedProperty sp = serializedObject.FindProperty("mAtlas");
		sp.objectReferenceValue = obj;
		serializedObject.ApplyModifiedProperties();
		UGUITools.SetDirty(serializedObject.targetObject);
		UGUISettings.atlas = obj as UGUIAtlas;


		SelectSprite (UGUISettings.selectedSprite);
	}

	/// <summary>
	/// Sprite selection callback function.
	/// </summary>

	void SelectSprite (string spriteName)
	{
		serializedObject.Update();
		SerializedProperty atlas = serializedObject.FindProperty("mAtlas");

		UGUISprite uguiSprite = target as UGUISprite;
		uguiSprite.sprite =  (atlas.objectReferenceValue as UGUIAtlas).GetSprite (spriteName);

		serializedObject.ApplyModifiedProperties();
		UGUITools.SetDirty(serializedObject.targetObject);
		UGUISettings.selectedSprite = spriteName;
	}

	/// <summary>
	/// Draw the atlas and sprite selection fields.
	/// </summary>

	protected virtual bool ShouldDrawProperties ()
	{
		GUILayout.BeginHorizontal();
		if (UGUIEditorTools.DrawPrefixButton("Atlas"))
			ComponentSelector.Show<UGUIAtlas>(OnSelectAtlas);
		SerializedProperty atlas = UGUIEditorTools.DrawProperty("", serializedObject, "mAtlas", GUILayout.MinWidth(20f));

		if (GUILayout.Button("Edit", GUILayout.Width(40f)))
		{
			if (atlas != null)
			{
				UGUIAtlas atl = atlas.objectReferenceValue as UGUIAtlas;
				UGUISettings.atlas = atl;
				UGUIEditorTools.Select(atl.gameObject);
			}
		}
		GUILayout.EndHorizontal();

		UGUISprite uguiSprite = target as UGUISprite;
		string spriteName = uguiSprite.sprite == null ? "" : uguiSprite.sprite.name;
		UGUIEditorTools.DrawAdvancedSpriteField(atlas.objectReferenceValue as UGUIAtlas,spriteName , SelectSprite, true);
		return true;
	}

	/// <summary>
	/// All widgets have a preview.
	/// </summary>

	public override bool HasPreviewGUI ()
	{
		return (Selection.activeGameObject == null || Selection.gameObjects.Length == 1);
	}

	/// <summary>
	/// Draw the sprite preview.
	/// </summary>

	public override void OnPreviewGUI (Rect rect, GUIStyle background)
	{
		UGUISprite uguiSprite = target as UGUISprite;


		Texture2D tex = uguiSprite.mainTexture as Texture2D;
		if (tex == null) return;

//		UISpriteData sd = sprite.atlas.GetSprite(sprite.spriteName);
		UGUIEditorTools.DrawSprite(tex, rect, uguiSprite.sprite, uguiSprite.color);
	}
}
