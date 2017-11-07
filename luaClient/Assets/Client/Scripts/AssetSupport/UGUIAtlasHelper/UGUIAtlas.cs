using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class UGUIAtlas:MonoBehaviour
{
	// List of all sprites inside the atlas. Name is kept only for backwards compatibility, it used to be public.
	//[HideInInspector]
	[HideInInspector][SerializeField] List<Sprite> mSprites = new List<Sprite>();

	// Replacement atlas can be used to completely bypass this atlas, pulling the data from another one instead.
	[HideInInspector][SerializeField] UGUIAtlas mReplacement;

	// Dictionary lookup to speed up sprite retrieval at run-time
	Dictionary<string, int> mSpriteIndices = new Dictionary<string, int>();



//	public List<Texture> textures = new List<Texture>();
//	public Texture2D mainTexture;


	/// <summary>
	/// Rebuild the sprite indices. Call this after modifying the spriteList at run time.
	/// </summary>

	public void MarkSpriteListAsChanged ()
	{
		#if UNITY_EDITOR
		if (Application.isPlaying)
		#endif
		{
			mSpriteIndices.Clear();
			for (int i = 0, imax = mSprites.Count; i < imax; ++i)
				mSpriteIndices[mSprites[i].name] = i;
		}
	}



	public Sprite GetSprite(string spriteName){
		if (mReplacement != null)
		{
			return mReplacement.GetSprite(spriteName);
		}
		else if (!string.IsNullOrEmpty(spriteName))
		{
			if (mSprites.Count == 0) Upgrade();
			if (mSprites.Count == 0) return null;

			// O(1) lookup via a dictionary
			#if UNITY_EDITOR
			if (Application.isPlaying)
			#endif
			{
				// The number of indices differs from the sprite list? Rebuild the indices.
				if (mSpriteIndices.Count != mSprites.Count)
					MarkSpriteListAsChanged();

				int index;
				if (mSpriteIndices.TryGetValue(spriteName, out index))
				{
					// If the sprite is present, return it as-is
					if (index > -1 && index < mSprites.Count) return mSprites[index];

					// The sprite index was out of range -- perhaps the sprite was removed? Rebuild the indices.
					MarkSpriteListAsChanged();

					// Try to look up the index again
					return mSpriteIndices.TryGetValue(spriteName, out index) ? mSprites[index] : null;
				}
			}

			// Sequential O(N) lookup.
			for (int i = 0, imax = mSprites.Count; i < imax; ++i)
			{
				Sprite s = mSprites[i];

				// string.Equals doesn't seem to work with Flash export
				if (!string.IsNullOrEmpty(s.name) && spriteName == s.name)
				{
					#if UNITY_EDITOR
					if (!Application.isPlaying) return s;
					#endif
					// If this point was reached then the sprite is present in the non-indexed list,
					// so the sprite indices should be updated.
					MarkSpriteListAsChanged();
					return s;
				}
			}
		}
		return null;
	}

	public void Destory(){
		//TODO
		Resources.UnloadAsset (mSprites [0].texture);
	}


	/// <summary>
	/// List of sprites within the atlas.
	/// </summary>

	public List<Sprite> spriteList
	{
		get
		{
			if (mReplacement != null) return mReplacement.spriteList;
			if (mSprites.Count == 0) Upgrade();
			return mSprites;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteList = value;
			}
			else
			{
				mSprites = value;
			}
		}
	}


	/// <summary>
	/// Convenience function that retrieves a list of all sprite names that contain the specified phrase
	/// </summary>

	public BetterList<string> GetListOfSprites (string match)
	{
		if (mReplacement) return mReplacement.GetListOfSprites(match);
		if (string.IsNullOrEmpty(match)) return GetListOfSprites();

		if (mSprites.Count == 0) Upgrade();
		BetterList<string> list = new BetterList<string>();

		// First try to find an exact match
		for (int i = 0, imax = mSprites.Count; i < imax; ++i)
		{
			Sprite s = mSprites[i];

			if (s != null && !string.IsNullOrEmpty(s.name) && string.Equals(match, s.name, StringComparison.OrdinalIgnoreCase))
			{
				list.Add(s.name);
				return list;
			}
		}

		// No exact match found? Split up the search into space-separated components.
		string[] keywords = match.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < keywords.Length; ++i) keywords[i] = keywords[i].ToLower();

		// Try to find all sprites where all keywords are present
		for (int i = 0, imax = mSprites.Count; i < imax; ++i)
		{
			Sprite s = mSprites[i];

			if (s != null && !string.IsNullOrEmpty(s.name))
			{
				string tl = s.name.ToLower();
				int matches = 0;

				for (int b = 0; b < keywords.Length; ++b)
				{
					if (tl.Contains(keywords[b])) ++matches;
				}
				if (matches == keywords.Length) list.Add(s.name);
			}
		}
		return list;
	}

		


	/// <summary>
	/// Convenience function that retrieves a list of all sprite names.
	/// </summary>

	public BetterList<string> GetListOfSprites ()
	{
		if (mReplacement != null) return mReplacement.GetListOfSprites();
		if (mSprites.Count == 0) Upgrade();

		BetterList<string> list = new BetterList<string>();

		for (int i = 0, imax = mSprites.Count; i < imax; ++i)
		{
			Sprite s = mSprites[i];
			if (s != null && !string.IsNullOrEmpty(s.name)) list.Add(s.name);
		}
		return list;
	}

	/// <summary>
	/// Performs an upgrade from the legacy way of specifying data to the new one.
	/// </summary>

	bool Upgrade ()
	{
		return false;
	}




	/// <summary>
	/// Helper function that determines whether the two atlases are related.
	/// </summary>

	static public bool CheckIfRelated (UGUIAtlas a, UGUIAtlas b)
	{
		if (a == null || b == null) return false;
		return a == b || a.References(b) || b.References(a);
	}


	/// <summary>
	/// Helper function that determines whether the atlas uses the specified one, taking replacements into account.
	/// </summary>

	bool References (UGUIAtlas atlas)
	{
		if (atlas == null) return false;
		if (atlas == this) return true;
		return (mReplacement != null) ? mReplacement.References(atlas) : false;
	}

	/// <summary>
	/// Setting a replacement atlas value will cause everything using this atlas to use the replacement atlas instead.
	/// Suggested use: set up all your widgets to use a dummy atlas that points to the real atlas. Switching that atlas
	/// to another one (for example an HD atlas) is then a simple matter of setting this field on your dummy atlas.
	/// </summary>

	public UGUIAtlas replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UGUIAtlas rep = value;
			if (rep == this) rep = null;

			if (mReplacement != rep)
			{
				if (rep != null && rep.replacement == this) rep.replacement = null;
				if (mReplacement != null) MarkAsChanged();
				mReplacement = rep;
//				if (rep != null) material = null;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Mark all widgets associated with this atlas as having changed.
	/// </summary>

	public void MarkAsChanged ()
	{
//		#if UNITY_EDITOR
//		UGUITools.SetDirty(gameObject);
//		#endif
//		if (mReplacement != null) mReplacement.MarkAsChanged();
//
//		Image[] list = UGUITools.FindActive<Image>();
//
//		for (int i = 0, imax = list.Length; i < imax; ++i)
//		{
//			Image sp = list[i];
//
//			if (CheckIfRelated(this, sp.atlas))
//			{
//				UGUIAtlas atl = sp.atlas;
//				sp.atlas = null;
//				sp.atlas = atl;
//				#if UNITY_EDITOR
//				UGUITools.SetDirty(sp);
//				#endif
//			}
//		}
//
//		UIFont[] fonts = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];
//
//		for (int i = 0, imax = fonts.Length; i < imax; ++i)
//		{
//			UIFont font = fonts[i];
//
//			if (CheckIfRelated(this, font.atlas))
//			{
//				UGUIAtlas atl = font.atlas;
//				font.atlas = null;
//				font.atlas = atl;
//				#if UNITY_EDITOR
//				UGUITools.SetDirty(font);
//				#endif
//			}
//		}
//
//		UILabel[] labels = UGUITools.FindActive<UILabel>();
//
//		for (int i = 0, imax = labels.Length; i < imax; ++i)
//		{
//			UILabel lbl = labels[i];
//
//			if (lbl.bitmapFont != null && CheckIfRelated(this, lbl.bitmapFont.atlas))
//			{
//				UIFont font = lbl.bitmapFont;
//				lbl.bitmapFont = null;
//				lbl.bitmapFont = font;
//				#if UNITY_EDITOR
//				UGUITools.SetDirty(lbl);
//				#endif
//			}
//		}
	}

	#if UNITY_EDITOR
	public void AddSprite(Sprite sprite){
		mSprites.Add (sprite);
//		if (mainTexture == null) {
//			mainTexture = sprite.texture;
//		}
//		if (textures.IndexOf (sprite.texture) == -1) {
//			textures.Add (sprite.texture);
//		}
	}
	#endif
}
