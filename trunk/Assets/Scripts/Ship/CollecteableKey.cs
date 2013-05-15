using System;
using UnityEngine;

public class CollecteableKey : MonoBehaviour {		
	private Play play;
	
	private float angle;
	
	public static int TYPE_SILVER = 0;
	public static int TYPE_GOLD = 1;
	
	private int type;

	public void Initialize(Play play_, int type_) {
		play = play_;
		type = type_;
		angle = 0f;
	}

	void FixedUpdate() {
		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
		if (isShipVisible != Vector3.zero) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			transform.RotateAround(transform.up, angle);
			transform.Rotate(Vector3.right, 30.0f);
			if (angle >= 360f) {
				angle -= 360f;
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			play.KeyFound(type);
			Destroy(gameObject);
		}
	}
	
}

