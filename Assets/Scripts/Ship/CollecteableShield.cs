using System;
using UnityEngine;

public class CollecteableShield : MonoBehaviour {		
	private Play play;
	
	private int amount; // in percentage

	void Awake() {
		enabled = false;
	}	
	public void Initialize(Play play_, int amount_) {
		play = play_;
		amount = amount_;
	}

	void FixedUpdate() {
		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
		if (isShipVisible != Vector3.zero) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			play.ShieldShip(amount);
			Destroy(gameObject);
		}
	}
	
/*	void OnCollisionEnter(Collision c) {
		if (c.collider.tag == Ship.TAG) {
			play.ShieldShip(amount);
			Destroy(gameObject);
		}
	}*/
}
