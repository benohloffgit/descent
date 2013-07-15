using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Searches for suitable exit cell in his room
 * holds this position and roams around it slightely
 * 
 */
public class Bull : Enemy {	
	private RaycastHit hit;
	private GridPosition targetPosition;
	private GridPosition holdingPosition;
	private Mode mode;
	private AStarThreadState aStarThreadState = new AStarThreadState();
	private bool isOnPath;
	private int exitIndex;
//	private bool searchForExitPos;
	private Room room;
	private float holdingInterval;
	private float holdingTimer;

	private IntTriple[] exitPositions;

	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0, 0.81f, 0.16f), new Vector3(0, -0.81f, 0.16f), new Vector3(0, 0, 0)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
		
	public enum Mode { ROAMING=0, PATHFINDING=1, CHASING=2, HOLDING=3, SEARCHFOREXIT=4 }
	
	public override void InitializeWeapon(int mount, int type) {
		if (mount == Weapon.PRIMARY) {
			primaryWeapons.Add(new Weapon(this, mount, transform, play, type, WEAPON_POSITIONS,
				WEAPON_ROTATIONS, Game.ENEMY, spawn.isBoss));
		}
	}
	
	void Start() {
		spawn.ActivateEnemy(this);
		canBeDeactivated = false;
		isOnPath = false;
		mode = Mode.SEARCHFOREXIT;
//		searchForExitPos = true;
		room = play.cave.zone.GetRoom(play.cave.GetGridFromPosition(transform.position));
		exitPositions = new IntTriple[room.exits.Count];
		int i=0;
		System.Collections.Generic.Dictionary<IntTriple, Cell>.Enumerator en = room.exits.GetEnumerator();
		while (en.MoveNext()) {
			exitPositions[i] = en.Current.Value.pos;
			i++;
		}
		exitIndex = UnityEngine.Random.Range(0, exitPositions.Length);
//		Debug.Log ("Lookin for exit " + exitPositions[exitIndex]);
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
//				Debug.Log ("Bull path found " + aStarThreadState.roomPath.Count);
			}
		}
		if (mode == Mode.CHASING) {
			if (isOnPath) {
				play.movement.Chase(myRigidbody, currentGridPosition, targetPosition, movementForce*2f, ref isOnPath);
			} else {
				if (aStarThreadState.roomPath.Count > 0) {
					LinkedListNode<AStarNode> n = aStarThreadState.roomPath.First;
					targetPosition = n.Value.gridPos;
					aStarThreadState.roomPath.RemoveFirst();
					isOnPath = true;
				} else {
					mode = Mode.HOLDING;
					holdingPosition = targetPosition;
//					Debug.Log ("Exit pos reached on " + holdingPosition +  " " + play.cave.zone.GetRoom(holdingPosition).GetCell(holdingPosition.cellPosition).isExit);
				}
			}					
		}
		if (mode == Mode.SEARCHFOREXIT) {
			mode = Mode.PATHFINDING;
			GridPosition exitPos = new GridPosition(exitPositions[exitIndex], room.pos);
			play.movement.AStarPath(aStarThreadState, currentGridPosition, exitPos);
		} else if (mode == Mode.HOLDING) {
			if (Time.fixedTime > holdingTimer + holdingInterval) {
				if (currentGridPosition == holdingPosition) {
					mode = Mode.ROAMING;
					holdingInterval = UnityEngine.Random.Range (3f, 7f);
					holdingTimer = Time.fixedTime;
//					Debug.Log ("Switch to ROAMING " + targetPosition + " " + currentGridPosition + " " + holdingPosition);
				}
			} else {
				play.movement.Chase(myRigidbody, currentGridPosition, holdingPosition, movementForce*2f, ref isOnPath);
			}
		} else if (mode == Mode.ROAMING) {
			if (Time.fixedTime > holdingTimer + holdingInterval) {
				if (currentGridPosition != targetPosition) {
					mode = Mode.HOLDING;
					holdingInterval = UnityEngine.Random.Range (3f, 7f);
					holdingTimer = Time.fixedTime;
//					Debug.Log ("chasing back from " + currentGridPosition + " to " + holdingPosition);
				}
			} else {
				play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, 1, 1, movementForce*2f);
			}
		}
		
		if (aggressiveness > Enemy.AGGRESSIVENESS_OFF) {
			play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude), lookAtToleranceAiming,
				ref currentAngleUp, ref dotProductLookAt, Movement.LookAtMode.None);
		} else {
			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceAiming, ref currentAngleUp,
				ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
		}
		
		clazz = "Bull " + mode;
	}

}


