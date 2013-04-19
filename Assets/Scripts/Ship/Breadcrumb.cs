using System;
using UnityEngine;

public class Breadcrumb : MonoBehaviour {		
	private Play play;
	
	private float angle;
	
	public void Initialize(Play play_) {
		play = play_;
		angle = 0f;
	}

	void FixedUpdate() {
		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
		if (isShipVisible != Vector3.zero) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			transform.RotateAround(transform.forward, angle);
			if (angle >= 360f) {
				angle -= 360f;
			}
		}
	}	
}

