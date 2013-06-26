using System;
using UnityEngine;

public class Hit : MonoBehaviour {
		
	void Start() {
		Invoke ("DestroySelf", 1f);
	}
	
	private void DestroySelf() {
		Destroy(gameObject);
	}
	
}

