using UnityEngine;  
using UnityEngine.UI;  
using System.Collections;  
using UnityEngine.EventSystems;
using System.Collections.Generic;
using LuaInterface;  

public class UGUILuaEventListener:EventTrigger  
{  
	public delegate void VoidDelegate(LuaTable go);  
	public delegate void BoolDelegate(LuaTable go, bool isValue);  
	public delegate void FloatDelegate(LuaTable go, float fValue);  
	public delegate void IntDelegate(LuaTable go, int iIndex);  
	public delegate void StringDelegate(LuaTable go, string strValue);  

	public VoidDelegate onSubmit;  
	public VoidDelegate onClick;  
	public BoolDelegate onHover;  
	public BoolDelegate onToggleChanged;  
	public FloatDelegate onSliderChanged;  
	public FloatDelegate onScrollbarChanged;  
	public IntDelegate onDrapDownChanged;  
	public StringDelegate onInputFieldChanged;  

	public LuaTable luaTable;


	public override void OnSubmit(BaseEventData eventData)  
	{  
		if (onSubmit != null)  
			onSubmit(luaTable);  
	}  
	public override void OnPointerEnter(PointerEventData eventData)  
	{  
		Debug.Log ("OnPointerEnter" + gameObject.name);
		if (onHover != null)  
			onHover(luaTable, true);  
	}  
	public override void OnPointerClick(PointerEventData eventData)  
	{  
		Debug.Log ("OnPointerClick" + gameObject.name);

		if (onClick != null)  
			onClick(luaTable);  
		if (onToggleChanged != null)  
			onToggleChanged(luaTable, gameObject.GetComponent<Toggle>().isOn);  

	}  
	public override void OnPointerExit(PointerEventData eventData)  
	{  
		Debug.Log ("OnPointerExit" + gameObject.name);
		if (onHover != null)  
			onHover(luaTable, false);  
	}  
	public override void OnDrag(PointerEventData eventData)  
	{  
		if (onSliderChanged != null)  
			onSliderChanged(luaTable, gameObject.GetComponent<Slider>().value);  
		if (onScrollbarChanged != null)  
			onScrollbarChanged(luaTable, gameObject.GetComponent<Scrollbar>().value);  

	}  
	public override void OnSelect(BaseEventData eventData)  
	{  
		if (onDrapDownChanged != null)  
			onDrapDownChanged(luaTable, gameObject.GetComponent<Dropdown>().value);  
	}  
	public override void OnUpdateSelected(BaseEventData eventData)  
	{  
		if (onInputFieldChanged != null)  
			onInputFieldChanged(luaTable, gameObject.GetComponent<InputField>().text);  
	}  
	public override void OnDeselect(BaseEventData eventData)  
	{  
		if (onInputFieldChanged != null)  
			onInputFieldChanged(luaTable, gameObject.GetComponent<InputField>().text);  
	}  

	public static UGUILuaEventListener Get(LuaTable _table)  
	{  
		GameObject go = _table ["gameObject"] as GameObject;
		UGUILuaEventListener listener =go.GetComponent<UGUILuaEventListener>();  
		if (listener == null) {
			listener = go.AddComponent<UGUILuaEventListener> ();  
			listener.luaTable = _table;
		}
		return listener;  
	}  
}  