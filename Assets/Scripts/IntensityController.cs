using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensityController : MonoBehaviour {

	public Light moon;
	public WindZone wind;

	private float maxMoonIntensity;
	private float maxWindMain;
	private float maxWindTurbulance;
    private NM_Wind nm_wind;
	void Start () {
		maxMoonIntensity = 3f;
		maxWindMain = 20f;
		maxWindTurbulance = 5f;
        nm_wind = wind.GetComponent<NM_Wind>();
	}

	public bool LightOff() {
		return moon.intensity <= 0;
	}

	public void IncreaseIntensity() {
		ChangeIntensity (10f);
		//RenderSettings.fogDensity -= .001f;
	}

	public void DecreaseIntensity() {
		ChangeIntensity (-35f);
		//RenderSettings.fogDensity += .0075f;
	}

	private void ChangeIntensity(float positivity) {
		moon.intensity = Mathf.Clamp (moon.intensity + .001f * positivity, 0f, maxMoonIntensity);
		//wind.windMain = Mathf.Clamp (wind.windMain + .007f * positivity, 0f, maxWindMain);
		//wind.windTurbulence = Mathf.Clamp (wind.windTurbulence + .002f * positivity, 0f, maxWindTurbulance);
        nm_wind.WindSpeed = nm_wind.WindSpeed + .03f * positivity;
        nm_wind.Turbulence = nm_wind.Turbulence + .0004f * positivity;
	}
}
