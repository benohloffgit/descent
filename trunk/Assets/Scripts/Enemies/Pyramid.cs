using UnityEngine;
using System.Collections;

public class Pyramid : Enemy {
	private GridPosition targetPosition;
	private Mode mode;
	private float currentAngleUp;

	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0, 0.81f, 0.16f), new Vector3(0, -0.81f, 0.16f)};
		
	public enum Mode { ROAMING=0, SHOOTING=1, AIMING=2 }
	
	public override void InitializeWeapon(int ix, int w, int m) {
		weapons.Add(new Weapon(transform, play, w, m, WEAPON_POSITIONS[ix], Game.ENEMY));
	}
	
	void Start() {
		targetPosition = play.cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {
		if (isShipVisible != Vector3.zero && isShipVisible.magnitude <= shootingRange) {
			Shoot();
		}
		
		if (isShipVisible.magnitude <= shootingRange) {
			mode = Mode.AIMING;
		} else {
			mode = Mode.ROAMING;
		}
		
		if (mode == Mode.ROAMING) {
			play.movement.Roam(myRigidbody, ref targetPosition, 3, 8, movementForce);
			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceRoaming, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);		
		}
		if (mode == Mode.AIMING) {
			play.movement.Roam(myRigidbody, ref targetPosition, 2, 4, movementForce);
			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceAiming, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);
		}
	}

}

