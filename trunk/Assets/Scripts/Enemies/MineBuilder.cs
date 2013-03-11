using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MineBuilder : Enemy {
	private Cave cave;
		
	private RaycastHit hit;
	private GridPosition targetPosition;
	private GridPosition coverPosition;
	private float currentAngleUp;
	private Mode mode;
	private AStarThreadState aStarThreadState = new AStarThreadState();
	private CoverFindThreadState coverFindThreadState = new CoverFindThreadState();
	private bool isOnPath;
	private float roamingStart;

	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {Vector3.up, Vector3.down, Vector3.zero, Vector3.zero, Vector3.zero};
	private static float FORCE_MOVE = 5.0f;
	private static int LOOK_AT_DISTANCE = 4; // measured in cubes
	private static float LOOK_AT_ANGLE_TOLERANCE = 30.0f;
	private static float MAX_ROAMING_TIME = 4.0f;
	private static int HEALTH = 10;

	public enum Mode { ROAMING=0, HIDING=1, PATHFINDING=3, COVERFINDING=4 }
	
	public override void InitializeWeapon(int ix, int w, int m) {
		weapons.Add(new Weapon(transform, play, w, m, WEAPON_POSITIONS[ix], Game.ENEMY));
	}
	
	void Start() {
		targetPosition = play.cave.GetGridFromPosition(transform.position);
		coverPosition = GridPosition.ZERO;
		mode = Mode.ROAMING;
		currentAngleUp = 0f;
		isOnPath = false;
		roamingStart = Time.time;
		health = HEALTH;
		cave = play.cave;
	}
		
	void FixedUpdate() {
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
					if (coverPosition != cave.GetGridFromPosition(transform.position)) {
//						Debug.Log ("Mode.PATHFINDING" );					
						mode = Mode.PATHFINDING;
						play.movement.AStarPath(aStarThreadState, cave.GetGridFromPosition(transform.position), coverFindThreadState.coverPosition);
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
				play.movement.Chase(myRigidbody, targetPosition, FORCE_MOVE, ref isOnPath);
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
					coverPosition = cave.GetGridFromPosition(transform.position);
				}
				play.movement.CoverFind(coverFindThreadState, coverPosition, play.GetShipGridPosition());
			} else {
				play.movement.Roam(myRigidbody, ref targetPosition, 2, 4, FORCE_MOVE);
			}
		}
		
		play.movement.LookAt(myRigidbody, play.ship.transform, LOOK_AT_DISTANCE, LOOK_AT_ANGLE_TOLERANCE, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);
	}		
}


