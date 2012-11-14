using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	
	private Transform ship;
	
	void Start() {
		ship = GameObject.Find("Ship(Clone)").transform;
	}
	
	void LateUpdate() {
		transform.position = ship.position;
		transform.rotation = ship.rotation;
	}
}

