using System;
using UnityEngine;

public class Hit : MonoBehaviour {
	
	private Play play;
	private float randomSizer;
		
	public void Initialize(Play p) {
		play = p;
	}
	
	void Start() {
		Invoke ("DestroySelf", 0.3f);
		transform.Rotate(Vector3.forward, 360.0f * UnityEngine.Random.value);
		randomSizer = 1.05f + 0.05f * UnityEngine.Random.value;
	}
	
	void Update() {
		transform.localScale *= randomSizer;
		transform.Rotate(Vector3.forward, 1.0f);
	}

	private void DestroySelf() {
		Destroy(gameObject);
	}
	
}

