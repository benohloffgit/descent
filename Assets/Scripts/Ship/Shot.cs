using System;
using UnityEngine;

public class Shot : MonoBehaviour {
	public Game.Shot shotType;
		
	void Start() {
		Invoke ("DestroySelf", 10.0f);
	}
	
	void OnCollisionEnter(Collision c) {
		CancelInvoke("DestroySelf");
		if (c.collider.tag == Ship.TAG) {
			Debug.Log ("HIT Ship");
		}
		Destroy(gameObject);
	}	

	private void DestroySelf() {
		Destroy(gameObject);
	}
}
