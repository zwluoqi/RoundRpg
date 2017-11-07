using System;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.Editor.UI
{
	internal class UIUtils
	{
		public static GUIStyle richBoldFoldout;
		public static GUIStyle line;

		public static void SetupStyles()
		{
			if (richBoldFoldout != null) return;

			richBoldFoldout = new GUIStyle(EditorStyles.foldout);
			richBoldFoldout.richText = true;
			richBoldFoldout.fontStyle = FontStyle.Bold;
					
			line = new GUIStyle(GUI.skin.box);
		}

		public static void Separator(int padding = 0)
		{
			if (padding != 0) GUILayout.Space(padding);

			Rect position = EditorGUILayout.GetControlRect(false, 1f);
			position = EditorGUI.PrefixLabel(position, GUIContent.none);
			Rect texCoords = new Rect(0f, 1f, 1f, 1f - 1f / line.normal.background.height);
			GUI.DrawTextureWithTexCoords(position, line.normal.background, texCoords);

			if (padding != 0) GUILayout.Space(padding);
		}

		public static void Header(string header)
		{
			Rect rect = EditorGUILayout.GetControlRect(false, 24);
			rect.y += 8f;
			rect = EditorGUI.IndentedRect(rect);
			GUI.Label(rect, header, EditorStyles.boldLabel);
		}

		public static void Indent(int topPadding = 2)
		{
			EditorGUI.indentLevel++;
			GUILayout.Space(topPadding);
		}

		public static void Unindent(int bottomPadding = 5)
		{
			EditorGUI.indentLevel--;
			GUILayout.Space(bottomPadding);
		}

		public static bool Foldout(SerializedProperty foldout, string caption)
		{
			Separator(5);
			GUILayout.BeginHorizontal();
			GUILayout.Space(13);
            foldout.boolValue = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), foldout.boolValue, caption, true, richBoldFoldout);
            GUILayout.EndHorizontal();
			return foldout.boolValue;
		}

		public static bool ToggleFoldout(SerializedProperty toggle, SerializedProperty foldout, string caption)
		{
			Separator(5);
			GUILayout.BeginHorizontal();
			toggle.boolValue = EditorGUILayout.Toggle(GUIContent.none, toggle.boolValue, GUILayout.Width(13));
			GUILayout.Space(13);
            foldout.boolValue = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), foldout.boolValue, caption, true, richBoldFoldout);
			GUILayout.EndHorizontal();

			return toggle.boolValue;
		}

		public static void DrawProperty(SerializedProperty property, Action setter, params GUILayoutOption[] options)
		{
			DrawProperty(property, (GUIContent)null, setter, options);
		}

		public static void DrawProperty(SerializedProperty property, string content, Action setter, params GUILayoutOption[] options)
		{
			DrawProperty(property, new GUIContent(content), setter, options);
		}

		public static void DrawProperty(SerializedProperty property, GUIContent content, Action setter, params GUILayoutOption[] options)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(property, content, options);
			if (EditorGUI.EndChangeCheck())
			{
				setter.Invoke();
			}
		}

		public class HorizontalBlock : IDisposable
		{
			public HorizontalBlock(params GUILayoutOption[] options)
			{
				GUILayout.BeginHorizontal(options);
			}

			public void Dispose()
			{
				GUILayout.EndHorizontal();
            }
		}

		public class VerticalBlock : IDisposable
		{
			public VerticalBlock(params GUILayoutOption[] options)
			{
				GUILayout.BeginVertical(options);
			}

			public void Dispose()
			{
				GUILayout.EndVertical();
			}
		}
	}
}