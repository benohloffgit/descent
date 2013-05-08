using UnityEngine;
using System.Collections;

public class Wombat : Enemy {	
	private GridPosition targetPosition;
	private Mode mode;
	private float currentAngleUp;
	private float angleForward;
	
	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0, 0, 0)};
		
	public enum Mode { ROAMING=0, SHOOTING=1, AIMING=2 }
	
	void Start() {
		targetPosition = play.cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
		angleForward = 0f;
	}
	
	public override void InitializeWeapon(int mount, int w, int m) {
		if (mount == Weapon.SECONDARY) {
			int ammo = Mathf.FloorToInt(modelClazzAEquivalent/6.0f)+1;
			secondaryWeapons.Add
				(new Weapon(this, mount, transform, play, w, m, WEAPON_POSITIONS[0], Game.ENEMY, modelClazzAEquivalent + 1f, spawn.isBoss, ammo));
		}
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {
		secondaryWeapons[currentSecondaryWeapon].IsReloaded();
		
		if (isShipVisible != Vector3.zero && secondaryWeapons[currentSecondaryWeapon].ammunition > 0) {
			aggressiveness = Enemy.AGGRESSIVENESS_ON;
		}
		
		if (aggressiveness > Enemy.AGGRESSIVENESS_OFF) {
			mode = Mode.AIMING;
		} else {
			mode = Mode.ROAMING;
		}
		
		if (mode == Mode.ROAMING) {
			play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			angleForward = play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceRoaming, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);		
		} else if (mode == Mode.AIMING) {
			play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			angleForward = play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude), lookAtToleranceAiming, ref currentAngleUp, Movement.LookAtMode.None);
		}
		
		if (isShipVisible != Vector3.zero && currentAngleUp < 2.5f) {
			ShootSecondary();
		}
	}

}
