using UnityEngine;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour {

	void Awake()
	{
		GameObject.DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
