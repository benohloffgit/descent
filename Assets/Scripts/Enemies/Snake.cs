using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Snake : Enemy {
	private GridPosition targetPosition;
	private GridPosition coverPosition;
	private Mode mode;
	private float currentAngleUp;
	private bool isOnPath;
	private float roamingStart;

	private AStarThreadState aStarThreadState = new AStarThreadState();
	private CoverFindThreadState coverFindThreadState = new CoverFindThreadState();
	
	public enum Mode { ROAMING=0, HIDING=1, PATHFINDING=3, COVERFINDING=4   }

	private static float MAX_ROAMING_TIME = 4.0f;
	
	void Start() {
		targetPosition = play.cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
		isOnPath = false;
		roamingStart = Time.time;
	}
	
	public override void InitializeWeapon(int mount, int w, int m) {
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {		
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
						play.movement.AStarPath(aStarThreadState, currentGridPosition, coverFindThreadState.coverPosition);
					} else {
						mode = Mode.ROAMING;
						roamingStart = Time.time;
						coverPosition =GridPosition.ZERO;
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
		if (mode == Mode.ROAMING) {
			if (Time.time > roamingStart + MAX_ROAMING_TIME) {
				mode = Mode.COVERFINDING;
//				Debug.Log ("Mode.COVERFINDING" );
				if (coverPosition == GridPosition.ZERO) {
					coverPosition = currentGridPosition;
				}
				play.movement.CoverFind(coverFindThreadState, coverPosition, play.GetShipGridPosition());
			} else {
				play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			}
		}
		
		play.movement.LookAt(myRigidbody, play.ship.transform, 0, lookAtToleranceAiming, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);
	}

}

