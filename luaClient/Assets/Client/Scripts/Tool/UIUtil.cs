using System;
using UnityEngine;
using UnityEngine.UI;

public class UIUtil
{


	public static void ResetRectPos(GameObject go){
		RectTransform rectTrans = go.GetComponent<RectTransform> ();

		rectTrans.anchoredPosition = Vector2.zero;
		rectTrans.sizeDelta = Vector2.zero;
		rectTrans.localScale = Vector3.one;


	}


	public static void ResetTransPos(GameObject go){




	}
}