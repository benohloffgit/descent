using UnityEngine;
using System.Collections;

public class WallLaser : Enemy {

	private Transform laserAnchor;
	private Transform laserBeam;
	private Vector3 baseForward;
	private Vector3 baseRight;
	private Vector3 aimForward;
	private RaycastHit hit;
	private float lastDamageTime;
	private Shot loadedShot;
	private Weapon weapon;
	
	private static float LASER_LENGTH_MODIFIER = 0.95f;
	private static float RAYCAST_DISTANCE = RoomMesh.MESH_SCALE * 8;
	private static float TURNING_SPEED = 0.005f;
	private static float UPDATE_FREQUENCY = 1.0f; // seconds

	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0, 0, 0)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
	
//		laserAnchor = transform.FindChild("Laser Anchor");
//		laserBeam = laserAnchor.FindChild("Laser Beam");

	void Start() {
		laserAnchor = weapon.weaponTransform;
		weapon.Mount();
		weapon.Reload();
		loadedShot = weapon.loadedShots[0];
		laserBeam = loadedShot.transform;
		loadedShot.enabled = false;
		baseForward = laserAnchor.forward;
		baseRight = laserAnchor.right;
		GetNewAimForward();
		lastDamageTime = Time.time;
	}
	
	public override void InitializeWeapon(int mount, int w, int m) {
		weapon = new Weapon(this, mount, transform, play, w, m, WEAPON_POSITIONS, WEAPON_ROTATIONS, Game.ENEMY,
				modelClazzAEquivalent + 1, spawn.isBoss, -1);
		secondaryWeapons.Add(weapon);
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {
		if (aimForward != laserAnchor.forward) {
			Vector3 rotateTo = Vector3.RotateTowards(laserAnchor.forward, aimForward, TURNING_SPEED, TURNING_SPEED);
			laserAnchor.forward = rotateTo;
			ResizeLaser();
		} else {
			GetNewAimForward();
		}

		if (Time.time > lastDamageTime + UPDATE_FREQUENCY && hit.collider != null && hit.collider.tag == Ship.TAG) {
			lastDamageTime = Time.time;
			loadedShot.ExecuteLaserBeamDamage(hit.point);
		}
	}
	
	private void ResizeLaser() {
		Vector3 aimAt;
		if (Physics.Raycast(laserAnchor.position, laserAnchor.forward, out hit, RAYCAST_DISTANCE, Game.LAYER_MASK_ALL)) {
			aimAt = hit.point;
		} else {
			aimAt = laserAnchor.position + laserAnchor.forward * RAYCAST_DISTANCE;
		}
		float laserLength = Vector3.Distance(laserAnchor.position, aimAt * LASER_LENGTH_MODIFIER);
		laserBeam.localScale = new Vector3(1.0f, 1.0f, laserLength);
		laserBeam.position = laserAnchor.position + laserAnchor.forward * (laserLength/2.0f);
	}
	
	private void GetNewAimForward() {
		aimForward = Quaternion.AngleAxis(Random.Range(1.0f, 45.0f), baseRight) * baseForward;
		aimForward = Quaternion.AngleAxis(Random.Range(1.0f, 360.0f), baseForward) * aimForward;
	}
	
}

