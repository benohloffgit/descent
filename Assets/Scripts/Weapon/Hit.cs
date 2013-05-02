using System;
using UnityEngine;

public class Hit : MonoBehaviour {
	
	private Play play;
	private float randomSizer;
	private Vector3 towardsShip;
		
	public void Initialize(Play p) {
		play = p;
		towardsShip = (play.ship.transform.position-transform.position).normalized;
	}
	
	void Start() {
		Invoke ("DestroySelf", 0.3f);
		transform.Rotate(Vector3.forward, 360.0f * UnityEngine.Random.value);
		randomSizer = 1.05f + 0.05f * UnityEngine.Random.value;
	}
	
	void FixedUpdate() {
		transform.localScale *= randomSizer;
		transform.Rotate(Vector3.forward, 1.0f);
		transform.Translate(towardsShip * 2.0f * Time.deltaTime, Space.World);
	}

	private void DestroySelf() {
		Destroy(gameObject);
	}
	
}

