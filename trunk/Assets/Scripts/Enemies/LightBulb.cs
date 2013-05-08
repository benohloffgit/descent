using UnityEngine;
using System.Collections;

public class LightBulb : Enemy {
	private RaycastHit hit;
	private GridPosition targetPosition;
	private float currentAngleUp;
	
	private static float FORCE_MOVE = 5.0f;
	private static int LOOK_AT_DISTANCE = 4; // measured in cubes
	private static float LOOK_AT_ANGLE_TOLERANCE = 30.0f;

//	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0.53f, 0.63f, 0.750f), new Vector3(-0.53f, 0.63f, 0.750f)};
		
	public override void InitializeWeapon(int mount, int w, int m) {
/*		if (mount == Weapon.PRIMARY) {
			primaryWeapons.Add(new Weapon(this, mount, transform, play, w, m, WEAPON_POSITIONS[0], Game.ENEMY, modelClazzAEquivalent + 1, spawn.isBoss));
		} else {
			secondaryWeapons.Add(new Weapon(this, mount, transform, play, w, m, WEAPON_POSITIONS[1], Game.ENEMY, modelClazzAEquivalent + 1, spawn.isBoss));
		}*/
	}
	
	void Start() {
		targetPosition = play.cave.GetGridFromPosition(transform.position);
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {		
/*		play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, 2, 4, FORCE_MOVE);
		play.movement.LookAt(myRigidbody, currentGridPosition, play.ship.transform, LOOK_AT_DISTANCE, LOOK_AT_ANGLE_TOLERANCE, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);*/
	}		
}
