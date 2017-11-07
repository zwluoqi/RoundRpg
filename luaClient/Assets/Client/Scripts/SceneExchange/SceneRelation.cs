using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

public class SceneRelation : MonoBehaviour {


	public Light sceneLight;
	public Light playerLight;
	public Transform weatherTransform;
	public Camera mainCamera;
	public GameObject bg;
	public Transform[] attacks_pos;
	public Transform[] defences_pos;

	public List<SystemArgsExchange> SystemArgsExchanges = new List<SystemArgsExchange> ();
	public List<ParticleExchange> particleExchanges = new List<ParticleExchange>();
	public List<ColorArgsExchange> colorExchanges = new List<ColorArgsExchange>();


	void Start(){
		SceneCameraController sceneCameraController = gameObject.AddComponent<SceneCameraController> ();
		sceneCameraController.mainCameraTrans = mainCamera.transform.parent;
	}

	public void PreExchangeWeather(){
		foreach (var cor in SystemArgsExchanges) {
			GameObject.Destroy (cor);
		}
		SystemArgsExchanges.Clear ();

		foreach (var cor in colorExchanges) {
			GameObject.Destroy (cor);
		}
		colorExchanges.Clear ();
	}

	public void StartExchangeColor(string argsName,Color targetValue,float startTime,float duration){
		ColorArgsExchange colorExchange = gameObject.AddComponent<ColorArgsExchange> ();
		colorExchange.sceneRelation = this;
		colorExchanges.Add (colorExchange);
		colorExchange.StartExchangeFogColor (argsName, targetValue, startTime, duration);	
	}

	public void StartExchangeLight(string lightType,string argsName,float targetValue,float startTime,float duration){
		Light currentLight = null;
		if (lightType == "player_light") {
			currentLight = playerLight;
		} else {
			currentLight = sceneLight;
		}

		if (currentLight == null)
			return;
		SystemArgsExchange lightArgsExchange = gameObject.AddComponent<SystemArgsExchange> ();
		SystemArgsExchanges.Add (lightArgsExchange);
		lightArgsExchange.StartExchangeLight (currentLight, argsName, targetValue, startTime, duration);
	}

	public ParticleExchange StartLoadResource(string uri,float loadTime){
		ParticleExchange particleExchange = gameObject.AddComponent<ParticleExchange> ();
		particleExchanges.Add (particleExchange);
		particleExchange.StartLoadResource(weatherTransform,uri,loadTime);
		return particleExchange;
	}

	public ParticleExchange StartUnLoadResource(string uri,float unloadTime){
		ParticleExchange particleExchange = particleExchanges.Find (a => (a.uri == uri));
		if (particleExchange != null) {
			particleExchanges.Remove (particleExchange);
			particleExchange.StartUnloadResource (unloadTime);
		}
		return particleExchange;
	}


}
