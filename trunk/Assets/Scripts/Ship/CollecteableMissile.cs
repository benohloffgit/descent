using System;
using UnityEngine;

public class CollecteableMissile : MonoBehaviour {		
	private Play play;
	
	private int type;
	private int amount;
	private float angle;
	
	public void Initialize(Play play_, int type_, int amount_) {
		play = play_;
		type = type_;
		amount = amount_;
		angle = 0f;
	}

	void FixedUpdate() {
		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
		if (isShipVisible != Vector3.zero) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			transform.RotateAround(transform.up, angle);
			transform.RotateAround(-transform.right, 70.0f);
			if (angle >= 360f) {
				angle -= 360f;
			}
		}
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
