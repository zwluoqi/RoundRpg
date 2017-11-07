using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using LuaInterface;

public class SystemArgsExchange : MonoBehaviour {
	

	public void StartExchangeLight(Light currentLight, string argsName,float targetValue,float startTime,float duration){

		StartCoroutine (_StartExchangeLight (currentLight,argsName,targetValue,startTime,duration));
	}

	private IEnumerator _StartExchangeLight(Light currentLight, string argsName,float targetValue,float startTime,float duration){
		Debug.LogWarning ("_StartExchangeLight,"+argsName+" targetValue:"+targetValue+" startTime:"+startTime+" duration:"+duration);
		if (startTime > 0) {
			yield return new WaitForSeconds (startTime);
		}

		float startVal = 0;
		switch (argsName) {
		case "intensity":
			startVal = currentLight.intensity;
			break;
		case "fog_density":
			startVal = RenderSettings.fogDensity;
			break;
		default:
			Debug.LogError ("not support args:"+argsName);
			yield break;
			break;
		}

		float currentTime = 0;

		while (currentTime < duration) {
			currentTime += Time.deltaTime;

			switch (argsName) {
				case "intensity":
				currentLight.intensity = Mathf.Lerp(startVal,targetValue,currentTime/duration);
					break;
			case "fog_density":
				RenderSettings.fogDensity = Mathf.Lerp(startVal,targetValue,currentTime/duration);;
				break;
				default:
					Debug.LogError ("not support args");
					break;
			}
			yield return null;
		}

		Debug.LogWarning ("end _StartExchangeLight");

	}


}