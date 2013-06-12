using UnityEngine;
using System.Collections;

/*
 * Normal Roaming/Aiming behaviour according to generated model values
 * 
 */
public class Wombat : Enemy {	
	private GridPosition targetPosition;
	private Mode mode;
	private float aimingStart;
	private bool isReloaded;
	
	private static float AIMING_TIME = 1.0f;
	
	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0, 0, 0),new Vector3(0, 0, 0),new Vector3(0, 0, 0)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
		
	public enum Mode { ROAMING=0, SHOOTING=1, AIMING=2 }
	
	void Start() {
		targetPosition = play.cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
		isReloaded = false;
	}
	
	public override void InitializeWeapon(int mount, int type) {
		if (mount == Weapon.SECONDARY) {
			int ammo = Mathf.FloorToInt(modelClazzAEquivalent/6.0f)+1;
			secondaryWeapons.Add
				(new Weapon(this, mount, transform, play, Weapon.TYPE_MISSILE, WEAPON_POSITIONS,
					WEAPON_ROTATIONS, Game.ENEMY, spawn.isBoss, ammo));
		}
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {
		if (secondaryWeapons[currentSecondaryWeapon].ammunition > 0) {
			if (!isReloaded && secondaryWeapons[currentSecondaryWeapon].IsReloaded()) {
				aimingStart = Time.fixedTime;
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
			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceRoaming, ref currentAngleUp,
				ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);		
		} else if (mode == Mode.AIMING) {
			play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude), lookAtToleranceAiming,
				ref currentAngleUp, ref dotProductLookAt, Movement.LookAtMode.None);
			if (isShipVisible != Vector3.zero && dotProductLookAt > 0.95f && Time.fixedTime > aimingStart + AIMING_TIME) {
				ShootSecondary();
				isReloaded = false;
			}
		}
		
	}

}
