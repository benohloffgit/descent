using System;
using UnityEngine;

public class CollecteableMissile : Collecteable {		
	private int type;
	private int amount;
	private float angle;
	
	public void Initialize(Play play_, int type_, int amount_) {
		play = play_;
		type = type_;
		amount = amount_;
		angle = 0f;
	}

	public override void DispatchFixedUpdate() {
//		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
		if (transform.renderer.isVisible) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			transform.RotateAround(transform.up, angle);
			transform.Rotate(-Vector3.right, 70.0f);
//			transform.RotateAround(-transform.right, 70.0f);
			if (angle >= 360f) {
				angle -= 360f;
			}
		}
	}
	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			if (play.AddMissile(type, amount)) {
				play.RemoveCollecteable(this);
				Destroy(gameObject);
			}
		}
	}

/*	void OnCollisionEnter(Collision c) {
		if (c.collider.tag == Ship.TAG) {
			play.AddMissile(type, amount);
			Destroy(gameObject);
		}
	}*/
}
