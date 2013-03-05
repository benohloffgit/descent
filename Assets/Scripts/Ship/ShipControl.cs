using System;
using UnityEngine;
using System.Collections;

public class ShipControl {
	private GameInput gameInput;
	
	private Ship ship;
	private Game game;
	private RaycastHit hit;
	private Play play;
//	private Shot shotTemplate;
	
	private static Vector3 GUN_POSITION = new Vector3(0f, 1.5f, 0f);
	private static Vector3 LASER_POSITION = new Vector3(0f, -1.5f, 0f);
	private static Vector3 BREADCRUMB_POSITION = new Vector3(0f, 0f, 2.0f);
	private static float MAX_RAYCAST_DISTANCE = 100.0f;
	private static float RATE_OF_FIRE_SHOT = 0.5f;
	private static float RATE_OF_FIRE_LASER = 0.3f;
	
	public ShipControl() {
	}
	
	public void Initialize(Ship s, Game g, Play p, GameInput gI) {
		ship = s;
		game = g;
		play = p;
		gameInput = gI;
		
//		InitializeWeapons();
	}

	public void DispatchGameInput() {
		if (gameInput.isMobile) {
		} else {
			if ((Input.GetKeyDown("mouse 0") || Input.GetKeyDown(KeyCode.RightControl))  && Time.time > ship.lastShotTime + RATE_OF_FIRE_SHOT) {
				Vector3 bulletPath;
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
					bulletPath = (hit.point - (ship.transform.position + ship.transform.TransformDirection(GUN_POSITION))).normalized;
				} else {
					bulletPath = ship.transform.forward;
				}
				
				GameObject newBullet = game.CreateFromPrefab().CreateGunBullet(ship.transform.position + ship.transform.TransformDirection(GUN_POSITION), ship.transform.rotation);
//				newBullet.GetComponent<Shot>().Initialize(play);				
				Vector3 bulletDirection = bulletPath * Shot.SPEED;
				newBullet.rigidbody.AddForce(new Vector3(bulletDirection.x, bulletDirection.y, bulletDirection.z + ship.rigidbody.velocity.z));
//				Debug.Log (shipRelativeVelocity + " " + (bulletPath * GUN_BULLET_SPEED));
				ship.lastShotTime = Time.time;
			}
			if (Input.GetKeyDown("mouse 1") && Time.time > ship.lastLaserTime + RATE_OF_FIRE_LASER) {
				Vector3 laserPath;
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
					laserPath = (hit.point - (ship.transform.position + ship.transform.TransformDirection(LASER_POSITION))).normalized;
				} else {
					laserPath = ship.transform.forward;
				}
				
				GameObject newLaserShot = game.CreateFromPrefab().CreateLaserShot(ship.transform.position + ship.transform.TransformDirection(LASER_POSITION), ship.transform.rotation);
//				newLaserShot.GetComponent<Shot>().Initialize(play);				
				Vector3 laserShotDirection = laserPath * Shot.LASER_SPEED;
				newLaserShot.rigidbody.AddForce(new Vector3(laserShotDirection.x, laserShotDirection.y, laserShotDirection.z));
//				Debug.Log (shipRelativeVelocity + " " + (bulletPath * GUN_BULLET_SPEED));
				ship.lastLaserTime = Time.time;
			}
			if (Input.GetKeyDown(KeyCode.B)) {
				game.CreateFromPrefab().CreateBreadcrumb(ship.transform.position + ship.transform.TransformDirection(BREADCRUMB_POSITION), Quaternion.identity);
			}
			if (Input.GetKeyDown(KeyCode.L)) {
				ship.SwitchHeadlight();
			}
			if (Input.GetKeyDown(KeyCode.M)) {
				play.SwitchMiniMap();
			}
			if (Input.GetKeyDown(KeyCode.F)) {
				play.SwitchMiniMapFollow();
			}
		}
	}
}


