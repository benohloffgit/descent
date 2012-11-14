using System;
using UnityEngine;

public class Dim : MonoBehaviour {
//	private MyGUI myGUI;
	private TouchDelegate touchDelegate;
	
	void Awake() {
	}
	
	public Vector3 GetSize() {
		return renderer.bounds.size;
	}
	
	public void Initialize(MyGUI mG, TouchDelegate tD) {
//		myGUI = mG;
		touchDelegate = tD;
	}
	
	public void Select() {
		touchDelegate();
	}

}
