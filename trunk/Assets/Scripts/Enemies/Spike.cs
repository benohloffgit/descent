using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spike : Enemy {
	private Cave cave;
	
	private RaycastHit hit;
	private GridPosition targetPosition;
	private Mode mode;
	private float currentAngleUp;
	private AStarThreadState aStarThreadState = new AStarThreadState();
	private bool isOnPath;

	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {Vector3.zero};
		
	public enum Mode { ROAMING=0, SHOOTING=1, AIMING=2, PATHFINDING=3, CHASING=4 }
	
	public override void InitializeWeapon(int ix, int w, int m) {
		weapons.Add(new Weapon(this, w, m, WEAPON_POSITIONS[ix]));
	}
	
	void Start() {
		cave = play.cave;
		targetPosition = cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
		currentAngleUp = 0f;
		isOnPath = false;
	}
					
	void FixedUpdate() {
		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
		if (isShipVisible.magnitude <= shootingRange) {
			Shoot();
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
//			Debug.Log ("Chasing ...");
			if (isOnPath) {
				play.movement.Chase(myRigidbody, targetPosition, movementForce, ref isOnPath);
//				Debug.Log ("chasing " + isOnPath);
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
				play.movement.AStarPath(aStarThreadState, cave.GetGridFromPosition(transform.position), play.GetShipGridPosition());
			} else {
				play.movement.Roam(myRigidbody, ref targetPosition, 2, 4, movementForce);
			}
		}
		play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceAiming, ref currentAngleUp, Movement.LookAtMode.None);
	}
	

}


