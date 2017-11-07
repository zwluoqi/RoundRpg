using System;
using UnityEngine;
using System.Collections;


public class ParticleColorArgsExchange:MonoBehaviour
{

	public ParticleExchange particleExchange;

	public string argsName;

	public void StartExchangeParticleColor(string argsName,string targetValue,float startTime,float duration){
		this.argsName = argsName;
		StartCoroutine (_StartExchangeParticleColor (argsName,targetValue,startTime,duration));
	}

	private IEnumerator _StartExchangeParticleColor(string argsName,string targetValue,float startTime,float duration){
		Debug.LogWarning ("_StartExchangeParticleColor,"+argsName+" targetValue:"+targetValue+" startTime:"+startTime+" duration:"+duration);
		if (startTime > 0) {
			yield return new WaitForSeconds (startTime);
		}
		string[] color_vals = StringUtil.GetSplitString (targetValue, ",");
		int count = particleExchange.pas.Length;
		Color[] targetColorValues =  new Color[particleExchange.pas.Length];
		Color[] startVals = new Color[particleExchange.pas.Length];
		switch (argsName) {
		case "start_color":

			for (int i = 0; i < count; i++) {
				startVals [i] = particleExchange.pas [i].startColor;
				targetColorValues[i] = new Color (
					float.Parse(color_vals[0])/255f,
					float.Parse(color_vals[1])/255f,
					float.Parse(color_vals[2])/255f,
					float.Parse(color_vals[3])/255f);
			}
			break;
		case "start_color_a":

			for (int i = 0; i < count; i++) {
				startVals [i] = particleExchange.pas [i].startColor;
				targetColorValues[i]  = new Color (startVals [i].r,startVals [i].g,startVals [i].b,
					float.Parse(color_vals[0])/255f);
			}


			break;
		default:
			Debug.LogError ("not support args:"+argsName);
			break;
		}

		float currentTime = 0;

		while (currentTime < duration) {
			currentTime += Time.deltaTime;

			switch (argsName) {
			case "start_color":
			case "start_color_a":
				for (int i = 0; i < count; i++) {
					particleExchange.pas [i].startColor = Color.Lerp(startVals[i],targetColorValues[i],currentTime/duration);
				}
				break;
			default:
				Debug.LogError ("not support args");
				break;
			}
			yield return null;
		}

		Debug.LogWarning ("end _StartExchangeParticleColor");
	}

}