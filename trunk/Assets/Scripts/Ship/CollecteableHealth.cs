using System;
using UnityEngine;

public class CollecteableHealth : MonoBehaviour {		
	private Play play;
	
	private int amount; // in percentage
	
	public void Initialize(Play play_, int amount_) {
		play = play_;
		amount = amount_;
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			play.HealShip(amount);
			Destroy(gameObject);
		}
	}
	
/*	void OnCollisionEnter(Collision c) {
		if (c.collider.tag == Ship.TAG) {
			play.HealShip(amount);
			Destroy(gameObject);
		}
	}*/
}