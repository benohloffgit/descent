using System;
using UnityEngine;

public class CollecteableScroll : MonoBehaviour {		
	private Play play;
	
	private float angle;

/*	public void Initialize(Play play_) {
		play = play_;
		angle = 0f;
	}

	void FixedUpdate() {
		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
		if (isShipVisible != Vector3.zero) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			transform.RotateAround(transform.up, angle);
			transform.RotateAround(transform.right, 70.0f);
			if (angle >= 360f) {
				angle -= 360f;
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			play.ScrollFound();
			Destroy(gameObject);
		}
	}*/
	
}

