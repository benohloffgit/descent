using System;
using UnityEngine;

public class CollecteableShield : Collecteable {		
	
	private int amount;
	private float angle;

	public void Initialize(Play play_, int amount_) {
		play = play_;
		amount = amount_;
		angle = 0f;
	}

	public override void DispatchFixedUpdate() {
//		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
//		if (isShipVisible != Vector3.zero) {
		if (transform.renderer.isVisible) {
//		if (isShipVisible != Vector3.zero || (!play.isShipInPlayableArea || play.GetRoomOfShip().id == play.cave.secretCaveRoomID)) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			transform.RotateAround(transform.up, angle);
			if (angle >= 360f) {
				angle -= 360f;
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			play.RemoveCollecteable(this);
			play.ShieldShip(amount);
			play.ship.PlaySound(Game.SOUND_TYPE_VARIOUS, 23);
			Destroy(gameObject);
		}
	}
	
}
