using UnityEngine;
using System.Collections;

public class Bull : Enemy {	
	private GridPosition targetPosition;
	private Mode mode;
	private float currentAngleUp;
	
	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0, 0.81f, 0.16f), new Vector3(0, -0.81f, 0.16f)};
		
	public enum Mode { ROAMING=0, SHOOTING=1, AIMING=2 }
	
	void Start() {
		targetPosition = play.cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
	}
	
	public override void InitializeWeapon(int mount, int w, int m) {
		if (mount == Weapon.PRIMARY) {
			primaryWeapons.Add(
				new Weapon(mount, transform, play, w, m, WEAPON_POSITIONS[0], Game.ENEMY, modelClazzAEquivalent + 1, spawn.isBoss));
		} else {
			secondaryWeapons.Add
				(new Weapon(mount, transform, play, w, m, WEAPON_POSITIONS[1], Game.ENEMY, modelClazzAEquivalent + 1, spawn.isBoss));
		}
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {		
		if (isShipVisible.magnitude <= shootingRange) {
			aggressiveness = Enemy.AGGRESSIVENESS_ON;
		}
		
		if (aggressiveness > Enemy.AGGRESSIVENESS_OFF) {
			mode = Mode.AIMING;
		} else {
			mode = Mode.ROAMING;
		}
		
		if (mode == Mode.ROAMING) {
			play.movement.Roam(myRigidbody, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceRoaming, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);		
		} else if (mode == Mode.AIMING) {
			play.movement.Roam(myRigidbody, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude), lookAtToleranceAiming, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);
		}
	}

}


