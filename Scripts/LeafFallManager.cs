﻿#if !UNITY_WEBPLAYER
using UnityEngine;
using System.Collections;

public class LeafFallManager : Singleton<LeafFallManager> {

	public float minInterval, maxInterval;
	public AnimationCurve leafFallOverYear;
	public int minEmission, maxEmission;
	public int minEnergy, maxEnergy;
	public float horizontalShift, verticalShift;
	public AnimationCurve windinessToShift;
	public float minSpeed, maxSpeed;
	public float gravity;

	void Start () {

		CreateLeafParticles ();
	}

	public Color[] colors;
	public GameObject particleBase;
	void CreateLeafParticles () {

		int copyCount = colors.Length;
		foreach (Color color in colors) {
			GameObject particleCopy = Instantiate(particleBase) as GameObject;
			particleCopy.transform.parent = transform;
			particleCopy.transform.position = particleBase.transform.position;
			InitializeLeafParticles(particleCopy, copyCount, color);
		}
		Destroy (particleBase);
	}

	void InitializeLeafParticles (GameObject particleCopy, int copyCount, Color originalColor) {

		LeafParticles leafParticles = particleCopy.AddComponent<LeafParticles>();
		leafParticles.minInterval = minInterval;
		leafParticles.maxInterval = maxInterval;
		leafParticles.leafFallOverYear = leafFallOverYear;
		leafParticles.minEmission = minEmission / copyCount;
		leafParticles.maxEmission = maxEmission / copyCount;
		leafParticles.minEnergy = minEnergy;
		leafParticles.maxEnergy = maxEnergy;
		leafParticles.horizontalShift = horizontalShift;
		leafParticles.verticalShift = verticalShift;
		leafParticles.windinessToShift = windinessToShift;
		leafParticles.minSpeed = minSpeed;
		leafParticles.maxSpeed = maxSpeed;
		leafParticles.gravity = gravity;
		leafParticles.originalColor = originalColor;
		leafParticles.Init ();
	}
}
#endif