using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Normal Roaming/Aiming behaviour according to generated model values
 * Hides 
 * 
 */
public class Bat : Enemy {
	private GridPosition targetPosition;
	private GridPosition coverPosition;
	private Mode mode;
	private bool isOnPath;
	private float roamingStart;
	private float aimingStart;

	private AStarThreadState aStarThreadState = new AStarThreadState();
	private CoverFindThreadState coverFindThreadState = new CoverFindThreadState();
	
	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(-0.453f, 0, 0), new Vector3(0.453f, 0, 0), new Vector3(0, 0, 0)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
		
	public enum Mode { ROAMING=0, SHOOTING=1, AIMING=2, HIDING=3, PATHFINDING=4, COVERFINDING=5 }

	private static float MAX_ROAMING_TIME = 6.0f;
	private static float MAX_AIMING_TIME = 6.0f;
	
	public override void InitializeWeapon(int mount, int type) {
		if (mount == Weapon.PRIMARY) {
			primaryWeapons.Add(new Weapon(this, mount, transform, play, type, WEAPON_POSITIONS,
				WEAPON_ROTATIONS, Game.ENEMY, spawn.isBoss));
//		} else {
//			secondaryWeapons.Add(new Weapon(this, mount, transform, play, w, m, WEAPON_POSITIONS[1], Game.ENEMY, modelClazzAEquivalent + 1, spawn.isBoss));
		}
	}
	
	void Start() {
		targetPosition = play.cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
		isOnPath = false;
		roamingStart = Time.time;
	}

	public override void DispatchFixedUpdate(Vector3 isShipVisible) {		
		if (aggressiveness > Enemy.AGGRESSIVENESS_OFF && mode != Mode.AIMING) {
			if (mode == Mode.COVERFINDING) {
				coverFindThreadState.Complete();
			} else if (mode == Mode.PATHFINDING) {
				aStarThreadState.Complete();
			}
			mode = Mode.AIMING;
			aimingStart = Time.time;
		}
		
		if (mode == Mode.AIMING && Time.time > aimingStart + MAX_AIMING_TIME) {
			mode = Mode.COVERFINDING;
//				Debug.Log ("Mode.COVERFINDING 1" );
			if (coverPosition == GridPosition.ZERO) {
				coverPosition = currentGridPosition;
			}
			play.movement.CoverFind(coverFindThreadState, coverPosition, play.GetShipGridPosition());
		}

		if (mode == Mode.COVERFINDING) {
			if (coverFindThreadState.IsFinishedNow()) {
//				Debug.Log ("Mode.COVERFINDING finished " + coverFindThreadState.coverPosition);
				coverFindThreadState.Complete();
				if (coverFindThreadState.coverPosition == GridPosition.ZERO) { // no cover found
					mode = Mode.ROAMING;
					roamingStart = Time.time;
//					Debug.Log ("No cover found, Mode.ROAMING" );					
				} else {
					coverPosition = coverFindThreadState.coverPosition;
					if (coverPosition != currentGridPosition) {
//						Debug.Log ("Mode.PATHFINDING" );					
						mode = Mode.PATHFINDING;
						play.movement.AStarPath(aStarThreadState, currentGridPosition, coverPosition);
					} else {
						mode = Mode.ROAMING;
						roamingStart = Time.time;
						coverPosition = GridPosition.ZERO;
//						Debug.Log ("We are already on cover pos : Mode.ROAMING" );					
					}
				}
			}
		}
		if (mode == Mode.PATHFINDING) {
			if (aStarThreadState.IsFinishedNow()) {
				aStarThreadState.Complete();
				mode = Mode.HIDING;
				isOnPath = false;
//				Debug.Log ("Pathfinding finished");
			}
		}
		if (mode == Mode.HIDING) {
			if (isOnPath) {
				play.movement.Chase(myRigidbody, currentGridPosition, targetPosition, movementForce, ref isOnPath);
//				Debug.Log ("chasing " + isOnPath);
			} else {
				if (aStarThreadState.roomPath.Count > 0) {
					LinkedListNode<AStarNode> n = aStarThreadState.roomPath.First;
					targetPosition = n.Value.gridPos;
					aStarThreadState.roomPath.RemoveFirst();
					isOnPath = true;
//					Debug.Log ("setting new target position " + targetPosition);
				} else {
					mode = Mode.ROAMING;
					roamingStart = Time.time;
//					Debug.Log ("Mode.ROAMING" );
				}
			}
		}
		if (mode == Mode.ROAMING || mode == Mode.AIMING) {
			if (isShipVisible != Vector3.zero && isShipVisible.magnitude <= shootingRange) {
				aggressiveness = Enemy.AGGRESSIVENESS_ON;
			}
		}
		
		if (mode == Mode.ROAMING) {
			if (Time.time > roamingStart + MAX_ROAMING_TIME && aggressiveness == Enemy.AGGRESSIVENESS_OFF) {
				mode = Mode.COVERFINDING;
//				Debug.Log ("Mode.COVERFINDING 2" );
				if (coverPosition == GridPosition.ZERO) {
					coverPosition = currentGridPosition;
				}
				play.movement.CoverFind(coverFindThreadState, coverPosition, play.GetShipGridPosition());
			} else {
				play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
				play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceRoaming, ref currentAngleUp,
					ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
			}
		} else if (mode == Mode.AIMING) {
			play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude), lookAtToleranceAiming,
				ref currentAngleUp, ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
		} else {
			play.movement.LookAt(myRigidbody, play.ship.transform, 0, lookAtToleranceAiming, ref currentAngleUp,
				ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
		}
	}

}


