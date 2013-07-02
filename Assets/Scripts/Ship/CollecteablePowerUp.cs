using System;
using UnityEngine;

public class CollecteablePowerUp : MonoBehaviour {
	public static string TAG = "PowerUp";
	
	private Play play;
	
	private float angle;
	private int type;
	private int id;
	
	private float tiltAngle;
	private Renderer myRenderer;
	
	void Awake() {
		gameObject.layer = Game.LAYER_COLLECTEABLES;
	}
	
	public void Initialize(Play play_, int type_, int id_) {
		play = play_;
		type = type_;
		id = id_;
		angle = 0f;
		if (type == Game.POWERUP_SPECIAL) {
			tiltAngle = 0;
		} else {
			tiltAngle = 55f;
		}
		myRenderer = GetComponentInChildren<Renderer>();
	}

	void FixedUpdate() {
		if (myRenderer.isVisible) {
//		if (!play.isPaused && (!play.isShipInPlayableArea || play.GetRoomOfShip().id == play.cave.secretCaveRoomID)) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			transform.RotateAround(transform.up, angle);
			transform.Rotate(-Vector3.right, tiltAngle);
			if (angle >= 360f) {
				angle -= 360f;
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			play.CollectPowerUp(type, id);
			play.ship.PlaySound(Game.SOUND_TYPE_VARIOUS, 22);
			Destroy(gameObject);
		}
	}
}
