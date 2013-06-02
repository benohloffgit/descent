using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitHelper : MonoBehaviour {
	private Play play;
	private Game game;
	private Cave cave;
	private Rigidbody myRigidbody;
	private float currentAngleUp;
	private RaycastHit hit;
	private GridPosition targetPosition;
	private GridPosition currentGridPosition;
	private Mode mode;
	private AStarThreadState aStarThreadState = new AStarThreadState();
	private bool isOnPath;
	private float aimingStart;
	private bool isReloaded;
	
	private static float MOVEMENT_FORCE = 100f;
	private static int CHASING_RANGE = 4;
	
	public enum Mode { ROAMING=0, PATHFINDING=1, CHASING=2 }
	
	void Awake() {
		myRigidbody = transform.rigidbody;
	}
	
	public void Initialize(Play play_) {
		play = play_;
		game = play.game;
		cave = play.cave;
	}
	
	void Start() {
		targetPosition = cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
		currentAngleUp = 0f;
		isOnPath = false;
	}
					
	void FixedUpdate() {
		Vector3 isShipVisible = Vector3.zero;
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
	//			play.movement.Chase(myRigidbody, currentGridPosition, targetPosition, MOVEMENT_FORCE, ref isOnPath);
			} else {
				if (distanceToShip > CHASING_RANGE) {
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
//			if (distanceToShip > shootingRange) {
//				mode = Mode.PATHFINDING;
//				play.movement.AStarPath(aStarThreadState, currentGridPosition, play.GetShipGridPosition());
//			} else {
//				play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, MOVEMENT_FORCE);
//			}
		}
		if (true) {//aggressiveness > Enemy.AGGRESSIVENESS_OFF) {
//			play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude), lookAtToleranceAiming,
//				ref currentAngleUp, ref dotProductLookAt, Movement.LookAtMode.None);
//			if (isShipVisible != Vector3.zero && dotProductLookAt > 0.95f && Time.time > aimingStart + AIMING_TIME) {
//			}			
		} else {
//			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceAiming, ref currentAngleUp,
//				ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
		}
		
	}
	
}



