using System;
using UnityEngine;

public class CollecteablePowerUp : MonoBehaviour {
	public static string TAG = "PowerUp";
	
	private Play play;
	
	private float angle;
	private int type;
	private int id;
	
	private float tiltAngle;
	
//	private Mesh mesh;
	void Awake() {
		gameObject.layer = Game.LAYER_COLLECTEABLES;
	}
	
	public void Initialize(Play play_, int type_, int id_) {
		play = play_;
		type = type_;
		id = id_;
//		mesh = Resources.Load("Gun", typeof(Mesh)) as Mesh;
		angle = 0f;
		if (type == Game.POWERUP_SPECIAL) {
			tiltAngle = 0;
		} else {
			tiltAngle = 55f;
		}
	}

	void FixedUpdate() {
		if (!play.isPaused && (!play.isShipInPlayableArea || play.GetRoomOfShip().id == play.cave.secretCaveRoomID)) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			transform.RotateAround(transform.up, angle);
			transform.Rotate(-Vector3.right, tiltAngle);
			//transform.RotateAroundLocal(Vector3.right, 30.0f);
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
