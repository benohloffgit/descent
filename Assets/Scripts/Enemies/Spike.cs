using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spike : Enemy {
	public int health;

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
	private static int CHASE_RANGE = 4; // measured in cubes 5
	private static int LOOK_AT_RANGE = 8; // measured in cubes 8
	private static float SHOOTING_DISTANCE = RoomMesh.MESH_SCALE * LOOK_AT_RANGE;
	private static float CHASE_DISTANCE = RoomMesh.MESH_SCALE * CHASE_RANGE;
	private static Vector3 BULLET_POSITION = new Vector3(0,0,2.0f);
	private static float LOOK_AT_ANGLE_TOLERANCE_AIMING = 0.5f;
	private static float TURRET_TURN_SPEED = 1.0f;
//	private static float TURRET_MAX_ROTATION = 0.005f;
	
	private static int HEALTH = 15;
	
	public enum Mode { ROAMING=0, SHOOTING=1, AIMING=2, PATHFINDING=3, CHASING=4 }
	
	void Awake() {
		myRigidbody = GetComponent<Rigidbody>();
		turret = transform.Find("Turret");
	}
	
	public void Initialize(Game g, Play p) {
		game = g;
		play = p;
		cave = play.cave;
		health = HEALTH;
	}
	
	void Start() {
		targetPosition = cave.GetGridFromPosition(transform.position);
		lastShotTime = Time.time;
		mode = Mode.ROAMING;
		currentAngleUp = 0f;
		isOnPath = false;
	}
					
	void FixedUpdate() {
//		turret.RotateAround(transform.position, transform.TransformDirection(Vector3.up), TURRET_TURN_SPEED);
		
		Vector3 isVisible = play.ship.IsVisibleFrom(turret.position);
	// TODO take into account if in another room...
		float distanceToShip = Vector3.Distance(turret.position, play.GetShipPosition());
			
		if (isVisible != Vector3.zero) {
			if (Time.time > lastShotTime + SHOOTING_FREQUENCY) {
				if (isVisible.magnitude <= SHOOTING_DISTANCE) {
					Shoot();
				}
				lastShotTime = Time.time;
			}
		}
		if (mode == Mode.PATHFINDING) {
			if (aStarThreadState.IsFinishedNow()) {
				aStarThreadState.Complete();
				mode = Mode.CHASING;
				isOnPath = false;
//				Debug.Log ("Pathfinding finished");
			}
		}
		if (mode == Mode.CHASING) {
			if (isOnPath) {
				play.movement.Chase(myRigidbody, targetPosition, FORCE_MOVE, ref isOnPath);
//				Debug.Log ("chasing " + isOnPath);
			} else {
				if (distanceToShip > CHASE_DISTANCE) {
					if (aStarThreadState.roomPath.Count > 0) {
						LinkedListNode<AStarNode> n = aStarThreadState.roomPath.First;
						targetPosition = n.Value.gridPos;
						aStarThreadState.roomPath.RemoveFirst();
						isOnPath = true;
	//					Debug.Log ("setting new target position " + targetPosition);
					} else {
						mode = Mode.ROAMING;
					}
				} else {
					mode = Mode.ROAMING;
//					Debug.Log ("back to ROAMING");
				}
			}					
		}
		if (mode == Mode.ROAMING) {
			if (distanceToShip > SHOOTING_DISTANCE) {
				mode = Mode.PATHFINDING;
				play.movement.AStarPath(aStarThreadState, cave.GetGridFromPosition(transform.position), play.GetShipGridPosition());
			} else {
				play.movement.Roam(myRigidbody, ref targetPosition, 2, 4, FORCE_MOVE);
			}
		}
		play.movement.LookAt(myRigidbody, play.ship.transform, LOOK_AT_RANGE, LOOK_AT_ANGLE_TOLERANCE_AIMING, ref currentAngleUp);
	}

	private void Shoot() {
		GameObject newBullet = game.CreateFromPrefab().CreateGunBullet(turret.position + turret.TransformDirection(BULLET_POSITION), transform.rotation);
		Vector3 bulletDirection = turret.forward * Game.GUN_BULLET_SPEED;
		newBullet.rigidbody.AddForce(bulletDirection);
	}
	
	public override void Damage(int damage) {
		health -= damage;
		if (health <= 0) {
			Destroy(gameObject);
		}
	}

}


