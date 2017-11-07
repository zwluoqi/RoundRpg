using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckResUI:MonoBehaviour
{
	public Text textDesc;


	public void ShowCheck(){
		textDesc.text = "checking resource";
	}


	public void ShowDownLoading(){
		textDesc.text = "downloading resource";
	}


}