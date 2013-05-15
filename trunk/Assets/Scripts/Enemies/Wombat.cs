using UnityEngine;
using System.Collections;

public class Wombat : Enemy {	
	private GridPosition targetPosition;
	private Mode mode;
	private float currentAngleUp;
//	private float angleForward;
	private float aimingStart;
	private bool isReloaded;
	
	private static float AIMING_TIME = 1.0f;
	
	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0, 0, 0),new Vector3(0, 0, 0),new Vector3(0, 0, 0)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
		
	public enum Mode { ROAMING=0, SHOOTING=1, AIMING=2 }
	
	void Start() {
		targetPosition = play.cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
//		angleForward = 0f;
		isReloaded = false;
	}
	
	public override void InitializeWeapon(int mount, int w, int m) {
		if (mount == Weapon.SECONDARY) {
			int ammo = Mathf.FloorToInt(modelClazzAEquivalent/6.0f)+1;
			secondaryWeapons.Add
				(new Weapon(this, mount, transform, play, w, m, WEAPON_POSITIONS,
					WEAPON_ROTATIONS, Game.ENEMY, modelClazzAEquivalent + 1f, spawn.isBoss, ammo));
		}
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {
		if (secondaryWeapons[currentSecondaryWeapon].ammunition > 0) {
			if (!isReloaded && secondaryWeapons[currentSecondaryWeapon].IsReloaded()) {
				aimingStart = Time.time;
				isReloaded = true;
			}
			if (isShipVisible != Vector3.zero) {
				aggressiveness = Enemy.AGGRESSIVENESS_ON;
			}
		} else {
			aggressiveness = Enemy.AGGRESSIVENESS_OFF;
		}
		
		if (aggressiveness > Enemy.AGGRESSIVENESS_OFF) {
			mode = Mode.AIMING;
		} else {
			mode = Mode.ROAMING;
		}
		
		if (mode == Mode.ROAMING) {
			play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceRoaming, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);		
		} else if (mode == Mode.AIMING) {
			play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude), lookAtToleranceAiming, ref currentAngleUp, Movement.LookAtMode.None);
			if (isShipVisible != Vector3.zero && currentAngleUp < 2.5f && Time.time > aimingStart + AIMING_TIME) {
				ShootSecondary();
				isReloaded = false;
			}
		}
		
	}

}
