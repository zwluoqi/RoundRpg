using UnityEngine;
using System.Collections;

public class UIPage : MonoBehaviour {

	public enum PageType{
		COVER = 0,
		FULL_SCREEN = 1,
	}

	public PageType page_type = PageType.FULL_SCREEN;

	public string[] sigle_atla_names;

}
