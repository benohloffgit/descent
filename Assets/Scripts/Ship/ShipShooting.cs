using System;
using UnityEngine;
using System.Collections;

public class ShipShooting : MonoBehaviour {
	private GameInput gameInput;
	
	private Ship ship;
	private Game game;
	private RaycastHit hit;
//	private Shot shotTemplate;
	
	private static Vector3 GUN_POSITION = new Vector3(0f, 0f, 0.5f);
	private static Vector3 LASER_POSITION = new Vector3(0f, 0f, -0.5f);
	private static float GUN_BULLET_SPEED = 100.0f;
	private static float LASER_SHOT_SPEED = 200.0f;
	private static float MAX_RAYCAST_DISTANCE = 100.0f;
		
	void Awake() {
	}
	
	public void Initialize(Ship s, Game g, GameInput gI) {
		ship = s;
		game = g;
		gameInput = gI;
		
//		InitializeWeapons();
	}

	public void DispatchGameInput() {
		if (gameInput.isMobile) {
		} else {
			if (Input.GetKeyDown("mouse 0")) {
				Vector3 bulletPath;
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
					bulletPath = (hit.point - (ship.transform.position + GUN_POSITION)).normalized;
				} else {
					bulletPath = ship.transform.forward;
				}
				
				GameObject newBullet = game.CreateFromPrefab().CreateGunBullet(ship.transform.position + GUN_POSITION, ship.transform.rotation);
				Vector3 bulletDirection = bulletPath * GUN_BULLET_SPEED;
				newBullet.rigidbody.AddForce( new Vector3(bulletDirection.x, bulletDirection.y, bulletDirection.z + ship.rigidbody.velocity.z));
//				Debug.Log (shipRelativeVelocity + " " + (bulletPath * GUN_BULLET_SPEED));
			}
			if (Input.GetKeyDown("mouse 1")) {
				Vector3 laserPath;
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
					laserPath = (hit.point - (ship.transform.position + LASER_POSITION)).normalized;
				} else {
					laserPath = ship.transform.forward;
				}
				
				GameObject newLaserShot = game.CreateFromPrefab().CreateLaserShot(ship.transform.position + LASER_POSITION, ship.transform.rotation);
				Vector3 laserShotDirection = laserPath * LASER_SHOT_SPEED;
				newLaserShot.rigidbody.AddForce( new Vector3(laserShotDirection.x, laserShotDirection.y, laserShotDirection.z));
//				Debug.Log (shipRelativeVelocity + " " + (bulletPath * GUN_BULLET_SPEED));
			}
		}
	}
	
}



