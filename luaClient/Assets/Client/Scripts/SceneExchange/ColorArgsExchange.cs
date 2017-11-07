using System;
using UnityEngine;
using System.Collections;


public class ColorArgsExchange:MonoBehaviour
{
	public SceneRelation sceneRelation;
	public void StartExchangeFogColor(string argsName,Color targetValue,float startTime,float duration){
		StartCoroutine (_StartExchangeFogColor (argsName,targetValue,startTime,duration));
	}

	private IEnumerator _StartExchangeFogColor(string argsName,Color targetValue,float startTime,float duration){
		Debug.LogWarning ("_StartExchangeFogColor,"+argsName+" targetValue:"+targetValue+" startTime:"+startTime+" duration:"+duration);
		if (startTime > 0) {
			yield return new WaitForSeconds (startTime);
		}

		Color startVal = Color.white;
		switch (argsName) {
		case "fog_color":
			startVal = RenderSettings.fogColor;
			break;
		case "player_light_color":
			startVal = sceneRelation.playerLight.color;
			break;
		case "scene_light_color":
			startVal = sceneRelation.sceneLight.color;
			break;
		default:
			Debug.LogError ("not support args:"+argsName);
			break;
		}

		float currentTime = 0;

		while (currentTime < duration) {
			currentTime += Time.deltaTime;

			switch (argsName) {
			case "fog_color":
				RenderSettings.fogColor = Color.Lerp(startVal,targetValue,currentTime/duration);
				break;
			case "player_light_color":
				sceneRelation.playerLight.color = Color.Lerp(startVal,targetValue,currentTime/duration);;
				break;
			case "scene_light_color":
				sceneRelation.sceneLight.color = Color.Lerp(startVal,targetValue,currentTime/duration);;
				break;
			default:
				Debug.LogError ("not support args");
				break;
			}
			yield return null;
		}

		Debug.LogWarning ("end _StartExchangeFogColor");
	}

}