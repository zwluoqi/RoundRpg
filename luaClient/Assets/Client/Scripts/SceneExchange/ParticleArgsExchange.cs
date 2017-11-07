using UnityEngine;
using System.Collections;
using LuaInterface;
using System.Collections.Generic;


public class ParticleArgsExchange:MonoBehaviour
{
	public ParticleExchange particleExchange;

	public string argsName;

	public void StartEnterParticle(string argsName,float startValue,float targetValue,float startTime,float duration){

		this.argsName = argsName;
		StartCoroutine (_StartEnterParticle (argsName, startValue, targetValue, startTime, duration));

	}

	private IEnumerator _StartEnterParticle(string argsName,float startValue,float targetValue,float startTime,float duration){
		Debug.LogWarning ("_StartLeaveParticle,"+" argsName:"+argsName+" targetValue:"+targetValue+" startTime:"+startTime+" duration:"+duration);

		switch (argsName) {
		case "max_particle":
			foreach (var pa in particleExchange.pas) {
				pa.maxParticles = (int)startValue;
			}
			break;
		default:
			Debug.LogError ("not support args:"+argsName);
			break;
		}

		if (startTime > 0) {
			yield return new WaitForSeconds (startTime);
		}
			

		float currentTime = 0;
		while (currentTime < duration) {
			currentTime += Time.deltaTime;

			switch (argsName) {
			case "max_particle":
				foreach (var pa in particleExchange.pas) {
					pa.maxParticles = (int)Mathf.Lerp(startValue,targetValue,currentTime/duration);
				}
				break;
			default:
				Debug.LogError ("not support args");
				break;
			}
			yield return null;
		}
		Debug.LogWarning ("end _StartLeaveParticle");


	}

	public void StartLeaveParticle(string argsName,float targetValue,float startTime,float duration){

		StopCoroutine ("_StartEnterParticle");

		StartCoroutine (_StartLeaveParticle(argsName, targetValue, startTime, duration));
	}

	private IEnumerator _StartLeaveParticle(string argsName,float targetValue,float startTime,float duration){


		Debug.LogWarning ("_StartLeaveParticle,"+argsName+" targetValue:"+targetValue+" startTime:"+startTime+" duration:"+duration);
		if (startTime > 0) {
			yield return new WaitForSeconds (startTime);
		}

		float startVal = 0;
		switch (argsName) {
		case "max_particle":
			foreach (var pa in particleExchange.pas) {
				startVal = pa.maxParticles;
			}
			break;
		default:
			Debug.LogError ("not support args");
			break;
		}

		float currentTime = 0;

		while (currentTime < duration) {
			currentTime += Time.deltaTime;

			switch (argsName) {
			case "max_particle":
				foreach (var pa in particleExchange.pas) {
					pa.maxParticles = (int)Mathf.Lerp (startVal, targetValue, currentTime / duration);
				}
				break;
			default:
				Debug.LogError ("not support args");
				break;
			}
			yield return null;
		}

		Debug.LogWarning ("end _StartLeaveParticle");
	}


}



