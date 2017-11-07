//using System;
//using UnityEngine;
//
//public class SpriteEntry : Sprite
//{
//	// Sprite texture -- original texture or a temporary texture
//	public Texture2D tex;
//
//	// Temporary game object -- used to prevent Unity from unloading the texture
//	public GameObject tempGO;
//
//	// Temporary material -- same usage as the temporary game object
//	public Material tempMat;
//
//	// Whether the texture is temporary and should be deleted
//	public bool temporaryTexture = false;
//
//	/// <summary>
//	/// HACK: Prevent Unity from unloading temporary textures.
//	/// Discovered by "alexkring": http://www.tasharen.com/forum/index.php?topic=3079.45
//	/// </summary>
//
//	public void SetTexture (Color32[] newPixels, int newWidth, int newHeight)
//	{
////		Release();
////
////		temporaryTexture = true;
////
////		tex = new Texture2D(newWidth, newHeight);
////		tex.name = name;
////		tex.SetPixels32(newPixels);
////		tex.Apply();
////
////		tempMat = new Material(UGUISettings.atlas.spriteMaterial);
////		tempMat.hideFlags = HideFlags.HideAndDontSave;
////		tempMat.SetTexture("_MainTex", tex);
////
////		tempGO = EditorUtility.CreateGameObjectWithHideFlags(name, HideFlags.HideAndDontSave, typeof(MeshRenderer));
////		tempGO.GetComponent<MeshRenderer>().sharedMaterial = tempMat;
//	}
//
//	/// <summary>
//	/// Release temporary resources.
//	/// </summary>
//
//	public void Release ()
//	{
////		if (temporaryTexture)
////		{
////			Object.DestroyImmediate(tempGO);
////			Object.DestroyImmediate(tempMat);
////			Object.DestroyImmediate(tex);
////
////			tempGO = null;
////			tempMat = null;
////			tex = null;
////			temporaryTexture = false;
////		}
//	}
//}
