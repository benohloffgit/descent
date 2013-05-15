using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spike : Enemy {
	private RaycastHit hit;
	private GridPosition targetPosition;
	private Mode mode;
	private float currentAngleUp;
	private AStarThreadState aStarThreadState = new AStarThreadState();
	private bool isOnPath;

	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {Vector3.zero, new Vector3(0, 0, 0), new Vector3(0, 0, 0)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
		
	public enum Mode { ROAMING=0, PATHFINDING=1, CHASING=2 }
	
	public override void InitializeWeapon(int mount, int w, int m) {
		if (mount == Weapon.PRIMARY) {
			primaryWeapons.Add(new Weapon(this, mount, transform, play, w, m, WEAPON_POSITIONS,
				WEAPON_ROTATIONS, Game.ENEMY, modelClazzAEquivalent + 1, spawn.isBoss));
//		} else {
//			secondaryWeapons.Add(new Weapon(this, mount, transform, play, w, m, WEAPON_POSITIONS[1], Game.ENEMY, modelClazzAEquivalent + 1, spawn.isBoss));
		}
	}
	
	void Start() {
		targetPosition = cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
		currentAngleUp = 0f;
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
//				Debug.Log ("Pathfinding finished " + aStarThreadState.roomPath.Count);
			}
		}
		if (mode == Mode.CHASING) {
//			Debug.Log ("Chasing ..." + chasingRange + " " + shootingRange);
			if (isOnPath) {
				play.movement.Chase(myRigidbody, currentGridPosition, targetPosition, movementForce, ref isOnPath);
//				Debug.Log ("chasing " + isOnPath + " "  + Time.frameCount);
			} else {
				if (distanceToShip > chasingRange) {
					if (aStarThreadState.roomPath.Count > 0) {
						LinkedListNode<AStarNode> n = aStarThreadState.roomPath.First;
						targetPosition = n.Value.gridPos;
						aStarThreadState.roomPath.RemoveFirst();
						isOnPath = true;
//						Debug.Log ("setting new target position " + targetPosition);
					} else {
						mode = Mode.ROAMING;
//						Debug.Log ("back to ROAMING 01");
					}
				} else {
					mode = Mode.ROAMING;
//					Debug.Log ("back to ROAMING 02");
				}
			}					
		}
		if (mode == Mode.ROAMING) {
//			Debug.Log ("Roaming ...");
			if (distanceToShip > shootingRange) {
//				Debug.Log ("PATHFINDING");
				mode = Mode.PATHFINDING;
				play.movement.AStarPath(aStarThreadState, currentGridPosition, play.GetShipGridPosition());
			} else {
				play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			}
		}
		if (aggressiveness > Enemy.AGGRESSIVENESS_OFF) {
			play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude), lookAtToleranceAiming, ref currentAngleUp, Movement.LookAtMode.None);
		} else {
			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceAiming, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);
		}
		
	}
	

}


