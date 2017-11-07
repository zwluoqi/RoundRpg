using UnityEngine;  
using UnityEngine.UI;  
using System.Collections;  
using UnityEngine.EventSystems;
using System.Collections.Generic;
using LuaInterface;  

public class UGUIEventListener:EventTrigger  
{  
	public delegate void VoidDelegate(GameObject go);  
	public delegate void BoolDelegate(GameObject go, bool isValue);  
	public delegate void FloatDelegate(GameObject go, float fValue);  
	public delegate void IntDelegate(GameObject go, int iIndex);  
	public delegate void StringDelegate(GameObject go, string strValue);  

	public VoidDelegate onSubmit;  
	public VoidDelegate onClick;  
	public BoolDelegate onHover;  
	public BoolDelegate onToggleChanged;  
	public FloatDelegate onSliderChanged;  
	public FloatDelegate onScrollbarChanged;  
	public IntDelegate onDrapDownChanged;  
	public StringDelegate onInputFieldChanged;  


	public void OnClick()
	{
		Debug.Log("error");
	}
	public override void OnSubmit(BaseEventData eventData)  
	{  
		if (onSubmit != null)  
			onSubmit(gameObject);  
	}  
	public override void OnPointerEnter(PointerEventData eventData)  
	{  
		Debug.Log ("OnPointerEnter" + gameObject.name);
		if (onHover != null)  
			onHover(gameObject, true);  
	}  
	public override void OnPointerClick(PointerEventData eventData)  
	{  
		Debug.Log ("OnPointerClick" + gameObject.name);

		if (onClick != null)  
			onClick(gameObject);  
		if (onToggleChanged != null)  
			onToggleChanged(gameObject, gameObject.GetComponent<Toggle>().isOn);  

	}  
	public override void OnPointerExit(PointerEventData eventData)  
	{  
		Debug.Log ("OnPointerExit" + gameObject.name);
		if (onHover != null)  
			onHover(gameObject, false);  
	}  
	public override void OnDrag(PointerEventData eventData)  
	{  
		if (onSliderChanged != null)  
			onSliderChanged(gameObject, gameObject.GetComponent<Slider>().value);  
		if (onScrollbarChanged != null)  
			onScrollbarChanged(gameObject, gameObject.GetComponent<Scrollbar>().value);  

	}  
	public override void OnSelect(BaseEventData eventData)  
	{  
		if (onDrapDownChanged != null)  
			onDrapDownChanged(gameObject, gameObject.GetComponent<Dropdown>().value);  
	}  
	public override void OnUpdateSelected(BaseEventData eventData)  
	{  
		if (onInputFieldChanged != null)  
			onInputFieldChanged(gameObject, gameObject.GetComponent<InputField>().text);  
	}  
	public override void OnDeselect(BaseEventData eventData)  
	{  
		if (onInputFieldChanged != null)  
			onInputFieldChanged(gameObject, gameObject.GetComponent<InputField>().text);  
	}  

	public static UGUIEventListener Get(GameObject go)  
	{  
		UGUIEventListener listener =go.GetComponent<UGUIEventListener>();  
		if(listener==null) listener=go.AddComponent<UGUIEventListener>();  
		return listener;  
	}  
}  