using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MineBuilder : Enemy {
	private GridPosition targetPosition;
	private Mode mode;
	private AStarThreadState aStarThreadState = new AStarThreadState();
	private bool isOnPath;
	private int exitIndex;
	
	private int minesAlive;
	private int maxMines;
	private bool searchForNewMineLayingPos;
	private Room room;
	private IntTriple[] exitPositions;
	
	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0, 1.0f, 0),new Vector3(0, 1.0f, 0),new Vector3(0, 1.0f, 0)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};

	public enum Mode { ROAMING=0, PATHFINDING=3, CHASING=4 }
	
	void Start() {
		spawn.ActivateEnemy(this);
		canBeDeactivated = false;
		mode = Mode.ROAMING;
		searchForNewMineLayingPos = true;
		room = play.cave.zone.GetRoom(play.cave.GetGridFromPosition(transform.position));
		exitIndex = 0;
		exitPositions = new IntTriple[room.exits.Count];
		int i=0;
		System.Collections.Generic.Dictionary<IntTriple, Cell>.Enumerator en = room.exits.GetEnumerator();
		if (en.MoveNext()) {
			exitPositions[i] = en.Current.Value.pos;
			i++;
		}
	}
	
	public override void InitializeWeapon(int mount, int type) {
		minesAlive = Mathf.FloorToInt(modelClazzAEquivalent/10.0f)+4;
		maxMines = minesAlive;
		secondaryWeapons.Add
			(new Weapon(this, mount, transform, play, type, WEAPON_POSITIONS, WEAPON_ROTATIONS, Game.ENEMY,
				spawn.isBoss, minesAlive));
//		Debug.Log (modelClazzAEquivalent + " " + secondaryWeapons[0].damage);
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {		
		if (mode == Mode.PATHFINDING) {
			if (aStarThreadState.IsFinishedNow()) {
				aStarThreadState.Complete();
				mode = Mode.CHASING;
				isOnPath = false;
			}
		}
		if (mode == Mode.CHASING) {
//			Debug.Log ("Chasing ..." + chasingRange + " " + shootingRange);
			if (isOnPath) {
				play.movement.Chase(myRigidbody, currentGridPosition, targetPosition, movementForce, ref isOnPath);
//				Debug.Log ("chasing " + isOnPath + " "  + Time.frameCount);
			} else {
				if (aStarThreadState.roomPath.Count > 0) {
					LinkedListNode<AStarNode> n = aStarThreadState.roomPath.First;
					targetPosition = n.Value.gridPos;
					aStarThreadState.roomPath.RemoveFirst();
					isOnPath = true;
				} else {
					mode = Mode.ROAMING;
					searchForNewMineLayingPos = false;
				}
			}					
		}
		if (mode == Mode.ROAMING) {
//			Debug.Log ("Roaming ...");
			if (searchForNewMineLayingPos && minesAlive == maxMines) {
//				Debug.Log ("PATHFINDING");
				mode = Mode.PATHFINDING;
				GridPosition exitPos = new GridPosition(exitPositions[exitIndex], room.pos);
				play.movement.AStarPath(aStarThreadState, currentGridPosition, exitPos);
				exitIndex++;
				if (exitIndex >= exitPositions.Length) {
					exitIndex = 0;
				}
			} else {
				play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
				ShootSecondary();
			}
		}
		play.movement.LookAt(myRigidbody, play.ship.transform, 0, lookAtToleranceRoaming, ref currentAngleUp, ref dotProductLookAt,
			Movement.LookAtMode.IntoMovingDirection);
		if (secondaryWeapons[currentSecondaryWeapon].ammunition == 0) {
			canBeDeactivated = true;
		}
	}
	
	public void MineDestroyed() {
		minesAlive--;
		if (minesAlive == 0) {
			secondaryWeapons[currentSecondaryWeapon].ammunition = maxMines;
			minesAlive = maxMines;
			searchForNewMineLayingPos = true;
			canBeDeactivated = false;
		}
	}
}

// determine room, go to 1 exit, lay mines with normal roam mode. go to other exit with pathfinding, switch to roam


