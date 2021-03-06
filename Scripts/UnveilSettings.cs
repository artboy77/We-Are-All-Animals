﻿using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class AnimalInfo {

	public AnimalAnimator animalAnimator;
	public Vector2 spawnTime;
	public DateTime dateTime;
	[HideInInspector] public bool spawned;

}

public class UnveilSettings : MonoBehaviour {
	
	void OnEnable () {

		SceneManager.instance.OnNewDay += CheckDate;
	}

	public int unveilMonth, unveilDay;
	void CheckDate () {

		if (SceneManager.realDate.Month == unveilMonth && SceneManager.realDate.Day == unveilDay) {
			StartCoroutine (Initialize ());
			active = true;
		} else {
			active = false;
		}
	}

	IEnumerator Initialize () {

		for (int delay = 0; delay < 1; delay ++)
			yield return null;
		SkyManager.instance.SetPhaseTimes (4f, 6f, 20.75f, 22f);
		InitializeDateTimes ();
		InitializeAccumulationTimes ();
		ApplyAnimalSettings ();
		SpawnSnow ();
	}

	public float birdSpawnChance;
	void ApplyAnimalSettings () {

		//The spawner is turned back on by the BirthdayManager by the next day
		AnimationSpawner.instance.on = false;
		AnimationSpawner.instance.ClearAnimations();
		//the spawn chance will reset in AnimationSpawner the next day
		AnimationSpawner.instance.birdSpawnChance = birdSpawnChance;
		foreach (AnimalInfo animalInfo in animals) 
			animalInfo.spawned = false;
	}

	void InitializeDateTimes () {

		for (int i = 0; i < animals.Length; i++) {
			Vector2 spawnTime = animals[i].spawnTime;
			DateTime dateTime = new DateTime(SceneManager.currentDate.Year, 
			                                 unveilMonth, unveilDay, (int)spawnTime.x, (int)spawnTime.y, 0);
			animals[i].dateTime = dateTime;
		}
	}
	
	void InitializeAccumulationTimes () {

		accumStart = (float)SceneManager.minsAtDayStart + (18.77f * 60);
		accumEnd = (float)SceneManager.minsAtDayStart + (20f * 60);
	}

	void SpawnSnow () {

		WeatherControl.instance.TurnOff();
		WeatherInfo weatherType = WeatherControl.instance.weatherTypes [0];
		float startTime = (float)SceneManager.minsAtDayStart ;
		WeatherControl.instance.EnableWeather(weatherType, startTime, 1440, 15, 1, 0.25f);
	}

	private bool active;
	void Update () {

		if (!active) return;
		MakeAnimalSpawnAttempt ();
		OverwriteSnow ();
	}

	public AnimalInfo[] animals;
	void MakeAnimalSpawnAttempt () {

		foreach (AnimalInfo animalInfo in animals) {
			if (animalInfo.spawned) continue;
			if (SceneManager.realDate >= animalInfo.dateTime) {
				AnimationSpawner.instance.Spawn(animalInfo.animalAnimator);
				animalInfo.spawned = true;
			}
		}
	}

	private float accumStart, accumEnd;
	public float maxSnowLevel;
	void OverwriteSnow () {

		float linearSnowLevel = Mathf.InverseLerp (accumStart, accumEnd, (float)SceneManager.currentMinutes);
		linearSnowLevel = Mathf.Clamp (linearSnowLevel, 0, maxSnowLevel);
		SnowManager.instance.snowLevel = SnowManager.instance.GetSnowLevel (linearSnowLevel);
		SnowManager.instance.TriggerSnowChange (SnowManager.instance.snowLevel);
	}
}
