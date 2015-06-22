﻿using UnityEngine;
using System.Collections;

public class BloomManager : MonoBehaviour {

	void Start () {

		glowEffect = Camera.main.GetComponent<GlowEffect> ();
	}

	void Update () {

		SetGlowValues ();
	}

	public AnimationCurve tintOverYear;
	private GlowEffect glowEffect;
	void SetGlowValues () {

		float tintAmount = tintOverYear.Evaluate (SceneManager.curvePos);
		glowEffect.glowTint = Color.Lerp (Color.black, Color.white, tintAmount);
	}
}
