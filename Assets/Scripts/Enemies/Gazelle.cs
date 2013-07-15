using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Normal Roaming/Aiming behaviour according to generated model values
 * Chases ship if < shootingRange
 * 
 */
public class Gazelle : Enemy {
	private RaycastHit hit;
	private GridPosition targetPosition;
	private Mode mode;
	private AStarThreadState aStarThreadState = new AStarThreadState();
	private bool isOnPath;

	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0.497f, 2.078f, 0f), new Vector3(-0.497f, 2.078f, 0f), new Vector3(0f, 0.548f, 0f)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
		
	public enum Mode { ROAMING=0, PATHFINDING=1, CHASING=2 }
	
	public override void InitializeWeapon(int mount, int type) {
		if (mount == Weapon.PRIMARY) {
			primaryWeapons.Add(new Weapon(this, mount, transform, play, type, WEAPON_POSITIONS,
				WEAPON_ROTATIONS, Game.ENEMY, spawn.isBoss));
		}
	}
	
	void Start() {
		targetPosition = cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
		isOnPath = false;
		canBeDeactivated = false;
	}
					
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {
		if (isShipVisible != Vector3.zero && isShipVisible.magnitude <= shootingRange) {
			aggressiveness = Enemy.AGGRESSIVENESS_ON;
		}
		float distanceToShip = Vector3.Distance(transform.position, play.GetShipPosition());
		
		if (mode == Mode.PATHFINDING) {
			if (aStarThreadState.IsFinishedNow()) {
				aStarThreadState.Complete();
				mode = Mode.CHASING;
				isOnPath = false;
			}
		}
		if (mode == Mode.CHASING) {
			if (isOnPath) {
				play.movement.Chase(myRigidbody, currentGridPosition, targetPosition, movementForce, ref isOnPath);
			} else {
				if (distanceToShip > chasingRange) {
					if (aStarThreadState.roomPath.Count > 0) {
						LinkedListNode<AStarNode> n = aStarThreadState.roomPath.First;
						targetPosition = n.Value.gridPos;
						aStarThreadState.roomPath.RemoveFirst();
						isOnPath = true;
					} else {
						mode = Mode.ROAMING;
					}
				} else {
					mode = Mode.ROAMING;
				}
			}					
		}
		if (mode == Mode.ROAMING) {
			if (distanceToShip > shootingRange && play.isShipInPlayableArea) {
				mode = Mode.PATHFINDING;
				play.movement.AStarPath(aStarThreadState, currentGridPosition, play.GetShipGridPosition());
			} else {
				play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			}
		}
		if (aggressiveness > Enemy.AGGRESSIVENESS_OFF) {
			play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude), lookAtToleranceAiming,
				ref currentAngleUp, ref dotProductLookAt, Movement.LookAtMode.None);
		} else {
			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceAiming, ref currentAngleUp,
				ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
		}
		
		//clazz = "Gaz " + mode;
	}

}



