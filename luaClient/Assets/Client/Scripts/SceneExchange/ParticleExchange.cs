using UnityEngine;
using System.Collections;
using LuaInterface;
using System.Collections.Generic;


public class ParticleExchange:MonoBehaviour
{
	public string uri;
	public GameObject particleObj;
	public ParticleSystem[] pas;

	public List<ParticleArgsExchange> particleArgsExchanges = new List<ParticleArgsExchange> ();
	public List<ParticleColorArgsExchange> particleColorExchanges = new List<ParticleColorArgsExchange> ();

	void OnDestroy(){
		GameObject.Destroy (particleObj);
		particleObj = null;
		pas = null;
		uri = "";

		foreach (var pa in particleArgsExchanges) {
			GameObject.Destroy (pa);
		}
		particleArgsExchanges.Clear ();

		foreach (var pa in particleColorExchanges) {
			GameObject.Destroy (pa);
		}
		particleColorExchanges.Clear ();
	}

	private bool preUnLoading = false;

	public void StartUnloadResource(float unloadTime){
		if (preUnLoading)
			return;
		preUnLoading = true;
		StartCoroutine (_StartUnloadResource (unloadTime));
	}

	private IEnumerator _StartUnloadResource(float unloadTime){
		yield return new WaitForSeconds (unloadTime);
		GameObject.Destroy (this);
	}

	public void StartLoadResource(Transform parentTrans, string uri,float loadTime){
		preUnLoading = false;
		this.uri = uri;
		Object obj = ResourceManager.Instance.LoadResourceBlock (uri);
		particleObj = GameObject.Instantiate (obj) as GameObject;
		particleObj.transform.SetParent (parentTrans);
		particleObj.transform.localPosition = Vector3.zero;
		particleObj.transform.localRotation = Quaternion.identity;
		particleObj.transform.localScale = Vector3.one;

		pas = particleObj.GetComponentsInChildren<ParticleSystem> ();

		particleObj.SetActive (false);
		StartCoroutine (_StartLoadResource (loadTime));
	}

	private IEnumerator _StartLoadResource(float loadTime){
		if (loadTime > 0) {
			yield return new WaitForSeconds (loadTime);
		}
		particleObj.SetActive (true);
	}

	public void StartEnterWeatherParticle(string argsName,float startValue,float targetValue,float startTime,float duration){
		ParticleArgsExchange particleArgsExchange = gameObject.AddComponent<ParticleArgsExchange> ();
		particleArgsExchange.particleExchange = this;
		particleArgsExchanges.Add (particleArgsExchange);
		particleArgsExchange.StartEnterParticle ( argsName, startValue, targetValue, startTime,duration);
	}

	public void StartLeaveWeatherParticle(string argsName,float targetValue,float startTime,float duration){
		ParticleArgsExchange particleArgsExchange = particleArgsExchanges.Find (a => (a.argsName == argsName));
		if (particleArgsExchange != null) {
			particleArgsExchange.StartLeaveParticle (argsName, targetValue, startTime, duration);
		}
	}


	public void StartExchangeParticleColor(string argsName,string targetValue,float startTime,float duration){
		ParticleColorArgsExchange particleColorExchange = particleColorExchanges.Find (a => (a.argsName == argsName));
		if (particleColorExchange == null) {
			particleColorExchange = gameObject.AddComponent<ParticleColorArgsExchange> ();
			particleColorExchange.particleExchange = this;
			particleColorExchanges.Add (particleColorExchange);
		}	
		particleColorExchange.StartExchangeParticleColor (argsName, targetValue, startTime, duration);
	}

}
