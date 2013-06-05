using UnityEngine;
using System.Collections;

public class WallGunNew : Enemy {

	private Transform gunAnchor;
	private Vector3 baseForward;
	private Vector3 baseRight;
	private Weapon weapon;
	
	private static float RAYCAST_DISTANCE = RoomMesh.MESH_SCALE * 8;
	private static float TURNING_SPEED = 0.005f;
	private static float UPDATE_FREQUENCY = 1.0f; // seconds

	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0,0,0.3f), new Vector3(0, 0, 0), new Vector3(0, 0, 0)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};

	void Awake() {
		gunAnchor = transform.Find ("Anchor");
	}
	
	void Start() {
		weapon.Mount();
		baseForward = gunAnchor.forward;
		baseRight = gunAnchor.right;
	}
	
	public override void InitializeWeapon(int mount, int type) {
		weapon = new Weapon(this, mount, gunAnchor, play, type, WEAPON_POSITIONS, WEAPON_ROTATIONS, Game.ENEMY,
				spawn.isBoss, -1);
		primaryWeapons.Add(weapon);
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {
		if (isShipVisible != Vector3.zero) {
			Vector3 rotateTo = Vector3.RotateTowards(gunAnchor.forward, isShipVisible.normalized, TURNING_SPEED, TURNING_SPEED);
			if (Vector3.Angle(baseForward, rotateTo) < 45.0f) {
				aggressiveness = Enemy.AGGRESSIVENESS_ON;
				gunAnchor.forward = rotateTo;
			}
		}		
	}	
}


