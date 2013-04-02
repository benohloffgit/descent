using System;
using UnityEngine;

public class CollecteableMissile : MonoBehaviour {		
	private Play play;
	
	private int type;
	private int amount;
	
	public void Initialize(Play play_, int type_, int amount_) {
		play = play_;
		type = type_;
		amount = amount_;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			play.AddMissile(type, amount);
			Destroy(gameObject);
		}
	}

/*	void OnCollisionEnter(Collision c) {
		if (c.collider.tag == Ship.TAG) {
			play.AddMissile(type, amount);
			Destroy(gameObject);
		}
	}*/
}
