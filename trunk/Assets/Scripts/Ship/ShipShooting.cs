using System;
using UnityEngine;
using System.Collections;

public class ShipShooting : MonoBehaviour {
	public GameObject gunBulletPrefab;
	
	private GameInput gameInput;
	
	private Ship ship;
	private RaycastHit hit;
	private GameObject gunBulletTemplate;
	private Shot shotTemplate;
	
	private static Vector3 GUN_POSITION = new Vector3(0f, 0f, 0.5f);
	private static float GUN_BULLET_SPEED = 100.0f;
	private static float MAX_RAYCAST_DISTANCE = 100.0f;
	private static int LAYER_CAVE = 8;
		
	void Awake() {
	}
	
	public void Initialize(Ship s, GameInput gI) {
		ship = s;
		gameInput = gI;
		
		InitializeWeapons();
	}

	public void DispatchGameInput() {
		if (gameInput.isMobile) {
		} else {
			if (Input.GetKeyDown("mouse 0")) {
				Vector3 bulletPath;
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, 1 << LAYER_CAVE)) {
					bulletPath = (hit.point - (ship.transform.position + GUN_POSITION)).normalized;
				} else {
					bulletPath = ship.transform.forward;
				}
				
				shotTemplate.shotType = Game.Shot.Bullet;
				GameObject newBullet = Instantiate(gunBulletTemplate, ship.transform.position + GUN_POSITION, ship.transform.rotation) as GameObject;
				Vector3 shipRelativeVelocity = ship.rigidbody.velocity;
				Vector3 bulletDirection = bulletPath * GUN_BULLET_SPEED;
				newBullet.rigidbody.AddForce( new Vector3(bulletDirection.x, bulletDirection.y, bulletDirection.z + shipRelativeVelocity.z));
//				Debug.Log (shipRelativeVelocity + " " + (bulletPath * GUN_BULLET_SPEED));
			}
		}
	}
	
	private void InitializeWeapons() {
		// primary
		gunBulletTemplate = GameObject.Instantiate(gunBulletPrefab) as GameObject;
		shotTemplate = gunBulletTemplate.GetComponent<Shot>();
		shotTemplate.enabled = false;
		
		// secondary
		
	}

}



