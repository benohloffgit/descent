using UnityEngine;
using System.Collections;

public class Bug : Enemy {	
	private GridPosition targetPosition;
//	private Mode mode;
	private float currentAngleUp;

//	public enum Mode { ROAMING=0 }
	
	void Start() {
//		mode = Mode.ROAMING;
	}
	
	public override void InitializeWeapon(int mount, int type) {
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {		
		play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
		play.movement.LookAt(myRigidbody, play.ship.transform, 0, lookAtToleranceRoaming, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);
	}

}
