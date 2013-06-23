using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitHelper : MonoBehaviour {
	private Play play;
//	private Ship ship;
//	private Game game;
	private Cave cave;
	private Rigidbody myRigidbody;
	private float currentAngleUp;
	private float dotProductLookAt;
	private RaycastHit hit;
	private GridPosition caveExit;
	private GridPosition targetPosition;
	private Vector3 targetPositionV3;
	private GridPosition currentGridPosition;
	private Mode mode;
	private AStarThreadState aStarThreadState = new AStarThreadState();
	private bool isOnPath;
	private float lastTimeSoundPlayed;
	private int myAudioSourceID = AudioSourcePool.NO_AUDIO_SOURCE;
	private float nextSoundTime;
	
	private static float MOVEMENT_FORCE = 20f;
	private static int ROAM_MIN_RANGE = 1;
	private static int ROAM_MAX_RANGE = 2;
	private static float MAX_DISTANCE_TO_SHIP = RoomMesh.MESH_SCALE * 4;
	private static float MIN_DISTANCE_TO_SHIP = RoomMesh.MESH_SCALE * 2;
	
	private static int SOUND_MIN_ID = 12;
	private static int SOUND_MAX_ID = 18;
	
	public enum Mode { Deactive=0, Roaming=1, FindingPathToExit=2, FindingPathToShip=3, MovingToExit=4, MovingToShip=5  }
	
	void Awake() {
		myRigidbody = transform.rigidbody;
	}
	
	public void Initialize(Play play_) {
		play = play_;
//		ship = play.ship;
//		game = play.game;
		cave = play.cave;
		mode = Mode.Deactive;
	}
	
	void FixedUpdate() {
		if (mode != Mode.Deactive && play.isShipInPlayableArea) {
			PlaySound();
			currentGridPosition = cave.GetGridFromPosition(transform.position);
//			float distanceToCaveExit = targetPositionV3-transform.position;
			
//			Vector3 isShipVisible = Vector3.zero;
				
			if (mode == Mode.FindingPathToExit || mode == Mode.FindingPathToShip) {
				if (aStarThreadState.IsFinishedNow()) {
					aStarThreadState.Complete();
					isOnPath = false;
					if (mode == Mode.FindingPathToExit) {
						mode = Mode.MovingToExit;
					} else {
						mode = Mode.MovingToShip;
					}
				}
			}
			if (mode == Mode.MovingToExit || mode == Mode.MovingToShip) {
				if (isOnPath) {
					play.movement.Chase(myRigidbody, currentGridPosition, targetPosition, MOVEMENT_FORCE, ref isOnPath);
					play.movement.LookAt(myRigidbody, transform, -1,
						5f, ref currentAngleUp, ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
				} else {
					float distanceToShip = Vector3.Distance(transform.position, play.GetShipPosition());
					if (aStarThreadState.roomPath.Count > 0
							&& ( 	(mode == Mode.MovingToExit && distanceToShip <= MIN_DISTANCE_TO_SHIP)
								 || (mode == Mode.MovingToShip && distanceToShip > MIN_DISTANCE_TO_SHIP)
							   )
						) {
						LinkedListNode<AStarNode> n = aStarThreadState.roomPath.First;
						targetPosition = n.Value.gridPos;
						aStarThreadState.roomPath.RemoveFirst();
						isOnPath = true;
					} else {
						if (targetPosition == play.placeShipBeforeExitDoor) {
//							Debug.Log ("at exit");
							mode = Mode.Roaming;
						} else {
							if (distanceToShip > MIN_DISTANCE_TO_SHIP) {
								if (distanceToShip > MAX_DISTANCE_TO_SHIP) {
//									Debug.Log ("distance to ship too big");
									FindPathToShip();
								} else {
									play.movement.LookAt(myRigidbody, play.ship.transform, 100,
										5f, ref currentAngleUp, ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
								}
							} else {
								FindPathToExit();
							}
						}
					}
				}					
			}
			if (mode == Mode.Roaming) {
				play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, ROAM_MIN_RANGE, ROAM_MAX_RANGE, MOVEMENT_FORCE);
			}
		}
	}
	
	public void Activate() {
		caveExit = play.placeShipBeforeExitDoor;
		currentGridPosition = cave.GetGridFromPosition(transform.position);
		currentAngleUp = 0f;
		isOnPath = false;
		FindPathToExit();
		lastTimeSoundPlayed = -1f;
	}
	
	private void FindPathToExit() {
//		Debug.Log ("FindPathToExit");
		mode = Mode.FindingPathToExit;
		play.movement.AStarPath(aStarThreadState, currentGridPosition, caveExit);
	}
	
	private void FindPathToShip() {
//		Debug.Log ("FindPathToShip");
		mode = Mode.FindingPathToShip;
		play.movement.AStarPath(aStarThreadState, currentGridPosition, play.GetShipGridPosition());
	}

	public void Deactivate() {
		mode = Mode.Deactive;
	}
	
	private void PlaySound() {
		if (lastTimeSoundPlayed == -1f || Time.fixedTime > lastTimeSoundPlayed + nextSoundTime) {
			myAudioSourceID = play.game.PlaySound(myAudioSourceID, transform, Game.SOUND_TYPE_VARIOUS, Random.Range(SOUND_MIN_ID, SOUND_MAX_ID+1));
			lastTimeSoundPlayed = Time.fixedTime;
			nextSoundTime = Random.Range (5f, 20f);
		}
	}

}



