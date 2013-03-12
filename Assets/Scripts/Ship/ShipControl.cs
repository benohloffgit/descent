using System;
using UnityEngine;
using System.Collections;

public class ShipControl {
	private GameInput gameInput;
	
	private Ship ship;
	private Game game;
//	private RaycastHit hit;
	private Play play;
//	private Shot shotTemplate;
	
//	private static Vector3 GUN_POSITION = new Vector3(0f, 1.5f, 0f);
//	private static Vector3 LASER_POSITION = new Vector3(0f, -1.5f, 0f);
	private static Vector3 BREADCRUMB_POSITION = new Vector3(0f, 0f, 2.0f);
//	private static float MAX_RAYCAST_DISTANCE = 100.0f;
//	private static float RATE_OF_FIRE_SHOT = 0.5f;
//	private static float RATE_OF_FIRE_LASER = 0.3f;
	
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
			if ((Input.GetKeyDown("mouse 0") || Input.GetKeyDown(KeyCode.LeftControl))
						&& ship.weapons[Ship.WEAPON_POSITION_WING_LEFT] != null
						&& Time.time > ship.weapons[Ship.WEAPON_POSITION_WING_LEFT].lastShotTime + ship.weapons[Ship.WEAPON_POSITION_WING_LEFT].frequency) {
				ship.weapons[Ship.WEAPON_POSITION_WING_LEFT].Shoot();
				ship.weapons[Ship.WEAPON_POSITION_WING_LEFT].lastShotTime = Time.time;
				
/*				Vector3 bulletPath;
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, Game.LAYER_MASK_ENEMIES_CAVE)) {
					bulletPath = (hit.point - (ship.transform.position + ship.transform.TransformDirection(GUN_POSITION))).normalized;
				} else {
					bulletPath = ship.transform.forward;
				}
				
				GameObject newBullet = game.CreateFromPrefab().CreateGunBullet(ship.transform.position + ship.transform.TransformDirection(GUN_POSITION), ship.transform.rotation);
//				newBullet.GetComponent<Shot>().Initialize(play);				
				Vector3 bulletDirection = bulletPath * Shot.SPEED;
				newBullet.rigidbody.AddForce(new Vector3(bulletDirection.x, bulletDirection.y, bulletDirection.z + ship.rigidbody.velocity.z));
//				Debug.Log (shipRelativeVelocity + " " + (bulletPath * GUN_BULLET_SPEED));
				ship.lastShotTime = Time.time;*/
			}
			if ((Input.GetKeyDown("mouse 1") || Input.GetKeyDown(KeyCode.RightControl))
						&& ship.weapons[Ship.WEAPON_POSITION_WING_RIGHT] != null
						&& Time.time > ship.weapons[Ship.WEAPON_POSITION_WING_RIGHT].lastShotTime + ship.weapons[Ship.WEAPON_POSITION_WING_RIGHT].frequency) {
				ship.weapons[Ship.WEAPON_POSITION_WING_RIGHT].Shoot();
				ship.weapons[Ship.WEAPON_POSITION_WING_RIGHT].lastShotTime = Time.time;
			}
/*			if (Input.GetKeyDown("mouse 1") && Time.time > ship.lastLaserTime + RATE_OF_FIRE_LASER) {
				Vector3 laserPath;
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, Game.LAYER_MASK_ENEMIES_CAVE)) {
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
			}*/
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
			if (Input.GetKeyDown(KeyCode.F1)) {
				ship.CycleCamera();
			}
		}
	}
}



