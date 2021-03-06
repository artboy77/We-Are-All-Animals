﻿using UnityEngine;
using System.Collections;
using System;

public class MonthInfo {
	public int monthStart;
	public int day = 1;
}

[System.Serializable]
public class PrefabInfo: MonthInfo {
	public GameObject prefab;
}

[System.Serializable]
public class TextureInfo: MonthInfo {
	public Texture texture;
}

public class TreeProperties : MonoBehaviour {

	public enum type {Generated, Model};
	public type treeType;
	ModeledTreeManager modeledTreeManager;
	GeneratedTreeManager generatedTreeManager;
	public PrefabInfo[] trees;
	public int barkMatNumber = 1;
	public int leafMatNumber = 0;
	public TextureInfo[] bark;
	public TextureInfo[] leaves;
	
	void Start () {

		if (treeType == type.Model) 
			InitModeledTree();
		else 
			InitGeneratedTree();
	}

	void InitModeledTree () {

		modeledTreeManager = gameObject.AddComponent<ModeledTreeManager>() as ModeledTreeManager;
		modeledTreeManager.barkMatNumber = barkMatNumber;
		modeledTreeManager.leafMatNumber = leafMatNumber;
		modeledTreeManager.bark = bark;
		modeledTreeManager.leaves = leaves;
		modeledTreeManager.Init ();
	}

	public float minLeafAmount; //in TreeProperties to allow for unique values for instances
	public float fallTintMultiplier = 1;
	public int currentMonthStart;
	void InitGeneratedTree () {

		generatedTreeManager = gameObject.AddComponent<GeneratedTreeManager>() as GeneratedTreeManager;
		generatedTreeManager.treeProperties = this;
		generatedTreeManager.trees = trees;
		generatedTreeManager.Init ();
	}
}
