using System;
using UnityEngine;

public class Explosion : MonoBehaviour {
	
	private Play play;
		
	public void Initialize(Play p) {
		play = p;
	}
	
	void Start() {
		Invoke ("DestroySelf", 0.5f);
	}
	
	void Update() {
		transform.localScale *= 1.2f;
	}

	private void DestroySelf() {
		Destroy(gameObject);
	}
	
}

