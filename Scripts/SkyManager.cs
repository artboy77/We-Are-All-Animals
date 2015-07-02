﻿using UnityEngine;
using Tools;
using System.Collections;

public class SkyManager : Singleton<SkyManager>{
	
	public SunProperties sun;
	public MoonProperties moon;
	public float sunriseAngle, sunsetAngle;
	public Gradient sunNightToDusk;
	[HideInInspector]
	public float intensityLerp;
	
	public Material SkyBoxMaterial1;
	public Material SkyBoxMaterial2;
	
	public GameObject Water;
	public bool IncludeWater = false;
	public Color WaterNight;
	public Color WaterDay;
	
	private float sunriseAstroTime, sunriseTime, sunsetTime, sunsetAstroTime;
	[HideInInspector]
	public float sunrisePosInDay, sunsetPosInDay;
	[HideInInspector]
	public float nightDayLerp;
	public Gradient sunMiddayTint;

	void OnEnable () { 
		
		SceneManager.instance.OnNewDay += UpdatePhaseTimes;
	}
	
	void Start () {

		UpdatePhaseTimes ();
	}
	
	void UpdatePhaseTimes () {
		
		sunriseAstroTime = SunControl.instance.sunriseAstroTime;
		sunriseTime = SunControl.instance.sunriseTime;
		sunsetTime = SunControl.instance.sunsetTime;
		sunsetAstroTime = SunControl.instance.sunsetAstroTime;
		double minsAtSunise = sunriseTime * 60 + SceneManager.minsAtDayStart;
		float sunrisePos = (float)(minsAtSunise / SceneManager.minsInYear);
		sunrisePosInDay = SunControl.instance.dayCurve.Evaluate (sunrisePos);
		double minsAtSunset = sunsetTime * 60 + SceneManager.minsAtDayStart;
		float sunsetPos = (float)(minsAtSunset / SceneManager.minsInYear);
		sunsetPosInDay = SunControl.instance.dayCurve.Evaluate (sunsetPos);
	}
	
	void Update () {
		
		ApplyPhaseChanges ();
		AdjustSunAndMoon ();
	}
	
	void ApplyPhaseChanges () {
		
		float time = SceneManager.curvePosDay * 24;
		if (time > sunsetAstroTime || time < sunriseAstroTime)
			SetNightSettings(time);
		if (time > sunriseAstroTime && time < sunriseTime)
			SetDuskSettings(time);
		if (time > sunriseTime && time < sunriseTime + 2) 
			SetDuskToMidaySettings(time);
		if (time > sunriseTime + 2 && time < sunsetTime - 2)
			SetMiddaySettings(time);
		if (time > sunsetTime - 2 && time < sunsetTime) 
			SetMiddayToDawnSettings(time);
		if (time > sunsetTime && time < sunsetAstroTime)
			SetDawnSettings(time);
	}

	public float testDarkness;
	void SetNightSettings (float time) {

		SetSkyBox (1, 0);
		Color nightTint = CloudControl.instance.nightToDusk.Evaluate(0);
		RenderSettings.ambientLight = AmbientLightingChanger.instance.NightToDusk(0);
		RenderSettings.fogColor = FogControl.instance.NightToDusk(0);	
		nightDayLerp = testDarkness;
		sun.light.intensity = 0;
		moon.light.intensity = moon.currentIntesity;
		DarkenSky (1);
	}

	public AnimationCurve daytimeToIntensity;
	void SetDuskSettings (float time) {

		float lerp = Mathf.InverseLerp (sunriseAstroTime, sunriseTime, time);
		SetSkyBox (1, lerp);
		SkyBoxMaterial1.SetColor ("_Tint", CloudControl.instance.NightToDusk(lerp));
		RenderSettings.ambientLight = AmbientLightingChanger.instance.NightToDusk (lerp);
		RenderSettings.fogColor = FogControl.instance.NightToDusk(lerp);
		nightDayLerp = Mathf.InverseLerp(sunriseAstroTime, sunriseTime + 2, time);
		SetSunColor (lerp, 0);
		float daytimeInfluence = daytimeToIntensity.Evaluate (nightDayLerp);
		sun.light.intensity = Tools.Math.Convert (daytimeInfluence,0, 1, 0, sun.currentIntesity);
		moon.light.intensity = Mathf.Lerp(moon.currentIntesity, 0, lerp);
		DarkenSky (1);
	}

	void SetDuskToMidaySettings (float time) {

		float lerp = Mathf.InverseLerp (sunriseTime, sunriseTime + 2, time);
		SetSkyBox (2, lerp);
		Color duskTint = CloudControl.instance.NightToDusk(1);
		SkyBoxMaterial2.SetColor ("_Tint", Color.Lerp (duskTint, CloudControl.instance.midday, lerp));
		Color duskAmbience = AmbientLightingChanger.instance.NightToDusk (1);
		RenderSettings.ambientLight = Color.Lerp (duskAmbience, AmbientLightingChanger.instance.midday, lerp);
		Color duskFog = FogControl.instance.NightToDusk(1);
		RenderSettings.fogColor = Color.Lerp(duskFog, FogControl.instance.midday, lerp);
		nightDayLerp = Mathf.InverseLerp(sunriseAstroTime, sunriseTime + 2, time);
		SetSunColor (1, lerp);
		float daytimeInfluence = daytimeToIntensity.Evaluate (nightDayLerp);
		sun.light.intensity = Tools.Math.Convert (daytimeInfluence,0, 1, 0, sun.currentIntesity);
		moon.light.intensity = 0;
		DarkenSky (1 - lerp);
	}

	void SetMiddaySettings (float time) {

		SetSkyBox (2, 1);
		SkyBoxMaterial2.SetColor ("_Tint", CloudControl.instance.midday);
		RenderSettings.ambientLight = AmbientLightingChanger.instance.midday;
		RenderSettings.fogColor = FogControl.instance.midday;
		SetSunColor (0, 1);
		sun.light.intensity = sun.currentIntesity;
		moon.light.intensity = 0;
		DarkenSky (0);
	}

	void SetMiddayToDawnSettings (float time) {

		float lerp = Mathf.InverseLerp (sunsetTime, sunsetTime - 2, time);
		SetSkyBox (2, lerp);
		Color duskTint = CloudControl.instance.NightToDusk(1);
		SkyBoxMaterial2.SetColor ("_Tint", Color.Lerp (duskTint, CloudControl.instance.midday, lerp));
		Color duskAmbience = AmbientLightingChanger.instance.NightToDusk (1);
		RenderSettings.ambientLight = Color.Lerp (duskAmbience, AmbientLightingChanger.instance.midday, lerp);
		Color duskFog = FogControl.instance.NightToDusk(1);
		RenderSettings.fogColor = Color.Lerp(duskFog, FogControl.instance.midday, lerp);
		nightDayLerp = Mathf.InverseLerp(sunsetAstroTime, sunsetTime - 2, time);
		SetSunColor (1, lerp);
		sun.light.intensity = Mathf.Lerp(0, sun.currentIntesity, lerp);
		moon.light.intensity = 0;
		DarkenSky (1 - lerp);
	}

	void SetDawnSettings (float time) {

		float lerp = Mathf.InverseLerp (sunsetAstroTime, sunsetTime, time);
		SetSkyBox (1, lerp);
		SkyBoxMaterial1.SetColor ("_Tint", CloudControl.instance.NightToDusk(lerp));
		RenderSettings.ambientLight = AmbientLightingChanger.instance.NightToDusk (lerp);
		RenderSettings.fogColor = FogControl.instance.NightToDusk(lerp);
		nightDayLerp = Mathf.InverseLerp(sunsetAstroTime, sunsetTime - 2, time);
		SetSunColor (lerp, 0);
		sun.light.intensity = 0;
		moon.light.intensity = Mathf.Lerp(moon.currentIntesity, 0, lerp);
		DarkenSky (1);
	}

	void AdjustSunAndMoon () {

		float currentIntensity = sun.light.intensity + moon.light.intensity;
		intensityLerp = Mathf.InverseLerp(0, sun.maxIntensity, currentIntensity);
		float posInDay = Mathf.Clamp01 (SunControl.instance.posInDay);
		float sunAngle = Math.Convert (posInDay, sunrisePosInDay, sunsetPosInDay, sunriseAngle, sunsetAngle);
		sun.transform.localEulerAngles = new Vector3(sunAngle, 0, 0);
		float posInNight = Mathf.Clamp01 (SunControl.instance.posInNight);
		float moonAngle = Math.Convert (posInNight, 0, 1, sunriseAngle, sunsetAngle);
		moon.transform.localEulerAngles = new Vector3 (moonAngle, 0, 0);
	}

	void DarkenSky (float skyDarkness) {

		Color skySnowTint = new Color (0, 0, 0, skyDarkness * CloudControl.instance.overcast);
		RenderSettings.skybox.SetColor ("_SnowColor", skySnowTint);
	}

	void SetSunColor (float nightToDuskValue, float middayValue, bool desaturate = false) {

		Color colorAroundNight = sunNightToDusk.Evaluate(nightToDuskValue);
		Color colorAtMidday = sun.GetMiddayColor ();
		Color currentColor = Color.Lerp (colorAroundNight, colorAtMidday, middayValue);
		sun.light.color = currentColor;
	}

	void SetSkyBox (int index, float blend) {

		RenderSettings.skybox = index == 1 ? SkyBoxMaterial1 : SkyBoxMaterial2;
		RenderSettings.skybox.SetFloat("_Blend", blend);
	}
}
