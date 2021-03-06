﻿using UnityEngine;
using System.Collections;

public class AmbientLightingChanger : Singleton <AmbientLightingChanger> {

	void Start () {

		SceneManager.instance.OnNewDay += SetTodaysMiddayColor;
		SetTodaysMiddayColor ();
	}

	private Color midayColorOfDay;
	void SetTodaysMiddayColor () {

		midayColorOfDay = middayOverYear.Evaluate (SceneManager.curvePos);
	}

	void Update () {

		UpdateAmbientLight ();
	}

	public float minDarkness, maxDarkness;
	public float lightningDarkness;
	void UpdateAmbientLight () {

		float darkness = Mathf.Lerp (maxDarkness, minDarkness, SkyManager.instance.intensityLerp);
		float darknessAfterLightning = Mathf.Lerp (darkness, lightningDarkness, CloudControl.instance.lightningDarkness);
		SetMiddayAmbience (darknessAfterLightning);
	}
	
	public Gradient nightToDusk;
	public float snowInfluence;
	public Color NightToDusk (float lerp) {

		Color initColor = nightToDusk.Evaluate (lerp);
		float darkness = Mathf.Lerp (maxDarkness, minDarkness, SkyManager.instance.intensityLerp);
		float snowEffect = SnowManager.instance.snowLevel * snowInfluence;
		darkness += snowEffect;
		darkness = Mathf.Clamp01 (darkness);
		Color darkened = SetDarkness (initColor, darkness);
		return darkened;
	}

	public Gradient middayOverYear;
	public Gradient _midday;
	[HideInInspector] public Color midday;
	void SetMiddayAmbience (float darkness) {

		midayColorOfDay = middayOverYear.Evaluate (SceneManager.curvePos);
		Color middayFullSnow = _midday.Evaluate (CloudControl.instance.middayValue);
		Color middayAfterSnow = Color.Lerp (midayColorOfDay, middayFullSnow, SnowManager.instance.snowLevel);
		midday = SetDarkness (middayAfterSnow, darkness);
	}
	
	public Color SetDarkness(Color color, float darkness) {

		color.r = Mathf.Clamp01(color.r - darkness);
		color.b = Mathf.Clamp01(color.b - darkness);
		color.g = Mathf.Clamp01(color.g - darkness);
		return color;
	}

	public AnimationCurve sunriseToBrightness, sunsetToBrightness;
	public float GetParticleBrightness () {

		float brightness = 0;
		if (SkyManager.instance.sunsetProgress == 0) {
			brightness = sunriseToBrightness.Evaluate(SkyManager.instance.sunriseProgress);
		} else {
			brightness = sunsetToBrightness.Evaluate(1 - SkyManager.instance.sunsetProgress);
		}
		return brightness;
	}
}
