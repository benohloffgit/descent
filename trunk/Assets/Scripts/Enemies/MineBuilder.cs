using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MineBuilder : Enemy {
	private GridPosition targetPosition;
	private Mode mode;
	private float currentAngleUp;

	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0, 1.0f, 0)};

	public enum Mode { ROAMING=0 }
	
	void Start() {
		mode = Mode.ROAMING;
	}
	
	public override void InitializeWeapon(int mount, int w, int m) {
		secondaryWeapons.Add
			(new Weapon(mount, transform, play, w, m, WEAPON_POSITIONS[0], Game.ENEMY,
				modelClazzAEquivalent + 1, spawn.isBoss, Mathf.FloorToInt(modelClazzAEquivalent/10.0f)+4));
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {		
		play.movement.Roam(myRigidbody, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
		play.movement.LookAt(myRigidbody, play.ship.transform, 0, lookAtToleranceRoaming, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);
	}
}


