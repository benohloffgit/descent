using System;
using UnityEngine;

public class CollecteablePowerUp : MonoBehaviour {
	public static string TAG = "PowerUp";
	
	private Play play;
	
	private float angle;
	private int weaponType;
	private int wType;
	
//	private Mesh mesh;
	void Awake() {
		gameObject.layer = Game.LAYER_COLLECTEABLES;
	}
	
	public void Initialize(Play play_, int weaponType_, int wType_) {
		play = play_;
		weaponType = weaponType_;
		wType = wType_;
//		mesh = Resources.Load("Gun", typeof(Mesh)) as Mesh;
		angle = 0f;
	}

	void FixedUpdate() {
		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
		if (isShipVisible != Vector3.zero) {
			transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			transform.RotateAround(transform.up, angle);
			transform.RotateAround(transform.right, 10.0f);
			//transform.RotateAroundLocal(Vector3.right, 30.0f);
			if (angle >= 360f) {
				angle -= 360f;
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			play.CollectWeapon(weaponType, wType);
			Destroy(gameObject);
		}
	}
}
