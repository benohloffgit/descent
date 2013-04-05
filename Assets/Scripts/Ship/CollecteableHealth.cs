using System;
using UnityEngine;

public class CollecteableHealth : MonoBehaviour {		
	private Play play;
	
	private int amount; // in percentage
//	private Vector3 tangent;
//	private Vector3 binormal;
	private Vector3 worldUp;
	
	void Awake() {
		enabled = false;
//		tangent = Vector3.zero;
//		binormal = Vector3.zero;
	}
	
	public void Initialize(Play play_, int amount_) {
		play = play_;
		amount = amount_;
	}
	
	void FixedUpdate() {
		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
		if (isShipVisible != Vector3.zero) {
//			Vector3.OrthoNormalize(ref isShipVisible, ref tangent, ref binormal);
//			worldUp = Vector3.Angle(transform.up, tangent) < Vector3.Angle(transform.up, binormal) ? tangent : binormal;
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
		}
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