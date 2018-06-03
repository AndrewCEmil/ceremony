using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensityController : MonoBehaviour {

	public Light moon;
	public WindZone wind;

	private float maxMoonIntensity;
	private float maxWindMain;
	private float maxWindTurbulance;
	void Start () {
		maxMoonIntensity = 3f;
		maxWindMain = 20f;
		maxWindTurbulance = 5f;
	}

	public bool LightOff() {
		return moon.intensity <= 0;
	}

	public void IncreaseIntensity() {
		ChangeIntensity (10f);
		RenderSettings.fogDensity -= .001f;
	}

	public void DecreaseIntensity() {
		ChangeIntensity (-75f);
		RenderSettings.fogDensity += .0075f;
	}

	private void ChangeIntensity(float positivity) {
		moon.intensity = Mathf.Clamp (moon.intensity + .001f * positivity, 0f, maxMoonIntensity);
		wind.windMain = Mathf.Clamp (wind.windMain + .007f * positivity, 0f, maxWindMain);
		wind.windTurbulence = Mathf.Clamp (wind.windTurbulence + .002f * positivity, 0f, maxWindTurbulance);
	}
}
