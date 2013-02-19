using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spike : MonoBehaviour {
	private Play play;
	private Game game;
	private Cave cave;
	
	private Transform turret;
	private Rigidbody myRigidbody;
	private RaycastHit hit;
	private GridPosition targetPosition;
	private float lastShotTime;
	private Mode mode;
	private float currentAngleUp;
	private AStarThreadState aStarThreadState = new AStarThreadState();
	private bool isOnPath;

	private static float SHOOTING_FREQUENCY = 1.0f;
	private static float FORCE_MOVE = 7.5f;
	private static int CHASE_RANGE = 5; // measured in cubes
	private static int LOOK_AT_RANGE = 8; // measured in cubes
	private static float SHOOTING_DISTANCE = RoomMesh.MESH_SCALE * LOOK_AT_RANGE;
	private static float CHASE_DISTANCE = RoomMesh.MESH_SCALE * CHASE_RANGE;
	private static Vector3 BULLET_POSITION = new Vector3(0,0,2.0f);
	private static float LOOK_AT_ANGLE_TOLERANCE_AIMING = 0.5f;
	private static float TURRET_TURN_SPEED = 2.5f;
//	private static float TURRET_MAX_ROTATION = 0.005f;

	public enum Mode { ROAMING=0, SHOOTING=1, AIMING=2, PATHFINDING=3, CHASING=4 }
	
	void Awake() {
		myRigidbody = GetComponent<Rigidbody>();
		turret = transform.Find("Turret");
	}
	
	public void Initialize(Game g, Play p) {
		game = g;
		play = p;
		cave = play.cave;
	}
	
	void Start() {
		targetPosition = cave.GetGridFromPosition(transform.position);
		lastShotTime = Time.time;
		mode = Mode.CHASING;
		currentAngleUp = 0f;
		isOnPath = false;
	}

/*			Vector3 toShipLocal = turret.InverseTransformDirection(isVisible).normalized;
			Vector3 turretLocal = Vector3.forward; //turret.InverseTransformDirection(turret.forward);
			toShipLocal.y = 0f;
			turretLocal.y = 0f;
			float angle = Vector3.Angle(turretLocal, toShipLocal);
				
			
				Vector3 rot = Vector3.RotateTowards(turretLocal, toShipLocal, 0.01f, 0.01f);
				rot.y = 0f;
				turret.forward = turret.TransformDirection(rot);
			*/
					
	void FixedUpdate() {
		turret.RotateAround(transform.position, transform.TransformDirection(Vector3.forward), TURRET_TURN_SPEED);
		Vector3 isVisible = play.ship.IsVisibleFrom(turret.position);
		if (isVisible != Vector3.zero) {
			if (Time.time > lastShotTime + SHOOTING_FREQUENCY) {
				if (isVisible.magnitude <= SHOOTING_DISTANCE) {
					mode = Mode.SHOOTING;
				} else if (mode != Mode.PATHFINDING && mode != Mode.CHASING) {
					mode = Mode.PATHFINDING;
//					GridPosition gp1 = cave.GetGridFromPosition(transform.position);
//					GridPosition gp2 = cave.GetGridFromPosition(play.GetShipPosition());
//					Debug.Log ("requesting path : " + gp1 + " / " + gp2);
//					Room r = cave.GetCurrentZone().GetRoom(gp1);
//					Debug.Log (r.GetCellDensity(gp1.cellPosition) + " " + r.GetCellDensity(gp2.cellPosition));
					play.movement.AStarPath(aStarThreadState, cave.GetGridFromPosition(transform.position), cave.GetGridFromPosition(play.GetShipPosition()));
				}
				lastShotTime = Time.time;
			}
		}
		if (mode == Mode.PATHFINDING) {
			if (aStarThreadState.isFinishedNow()) {
				aStarThreadState.Complete();
//				Debug.Log (Time.frameCount);
				mode = Mode.CHASING;
				isOnPath = false;
//				Debug.Log ("Pathfinding finished");
//				aStarThreadState.path.RemoveFirst(); // because this is our starting pos
			} else {
				// TODO
			}
		}
		if (mode == Mode.CHASING) {
			//play.movement.Roam(myRigidbody, ref targetPosition, 3, 8, FORCE_MOVE);
			if (isOnPath) {
				play.movement.Chase(myRigidbody, targetPosition, FORCE_MOVE, ref isOnPath);
//				Debug.Log ("chasing " + isOnPath);
			} else {
				if (isVisible.magnitude > CHASE_DISTANCE) {
					LinkedListNode<AStarNode> n = aStarThreadState.path.First;
					targetPosition = n.Value.gridPos;
					aStarThreadState.path.RemoveFirst();
					isOnPath = true;
//					Debug.Log ("setting new target position " + targetPosition);
				} else {
					mode = Mode.AIMING;
					Debug.Log ("back to AIMING");
				}
			}					
		} else if (mode == Mode.SHOOTING) {
//			Shoot();
			mode = Mode.AIMING;
		}
		if (mode == Mode.AIMING) {
			play.movement.Roam(myRigidbody, ref targetPosition, 2, 4, FORCE_MOVE);
//			play.movement.LookAt(myRigidbody, play.ship.transform, LOOK_AT_DISTANCE, LOOK_AT_ANGLE_TOLERANCE_AIMING);
		}
		play.movement.LookAt(myRigidbody, play.ship.transform, LOOK_AT_RANGE, LOOK_AT_ANGLE_TOLERANCE_AIMING, ref currentAngleUp);
	}

	private void Shoot() {
		GameObject newBullet = game.CreateFromPrefab().CreateGunBullet(turret.position + turret.TransformDirection(BULLET_POSITION), transform.rotation);
		Vector3 bulletDirection = turret.forward * Game.GUN_BULLET_SPEED;
		newBullet.rigidbody.AddForce(bulletDirection);
	}

}


