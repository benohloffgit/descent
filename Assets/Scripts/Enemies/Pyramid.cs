using UnityEngine;
using System.Collections;

public class Pyramid : MonoBehaviour {
	private Play play;
	private Game game;
		
	private Rigidbody myRigidbody;
	private RaycastHit hit;
	private GridPosition targetPosition;
	private float lastShotTime;
	private Mode mode;
	private float currentAngleUp;

	private static float SHOOTING_FREQUENCY = 1.0f;
	private static float FORCE_MOVE = 5.0f;
	private static int LOOK_AT_DISTANCE = 8; // measured in cubes
	private static float SHOOTING_RANGE = RoomMesh.MESH_SCALE * LOOK_AT_DISTANCE;
	private static Vector3 BULLET_POSITION = new Vector3(0,0,2.0f);
	private static float LOOK_AT_ANGLE_TOLERANCE_AIMING = 0.5f;
	private static float LOOK_AT_ANGLE_TOLERANCE_ROAMING = 30.0f;

	public enum Mode { ROAMING=0, SHOOTING=1, AIMING=2 }
	
	void Awake() {
		myRigidbody = GetComponent<Rigidbody>();
	}
	
	public void Initialize(Game g, Play p) {
		game = g;
		play = p;
	}
	
	void Start() {
		targetPosition = play.cave.GetGridFromPosition(transform.position);
		lastShotTime = Time.time;
		mode = Mode.ROAMING;
	}
		
	void FixedUpdate() {
		if (Time.time > lastShotTime + SHOOTING_FREQUENCY) {
			Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
			if (isShipVisible.magnitude <= SHOOTING_RANGE) {
				mode = Mode.SHOOTING;
			} else {
				mode = Mode.ROAMING;
			}
			lastShotTime = Time.time;
		}
		if (mode == Mode.ROAMING) {
			play.movement.Roam(myRigidbody, ref targetPosition, 3, 8, FORCE_MOVE);
			play.movement.LookAt(myRigidbody, play.ship.transform, LOOK_AT_DISTANCE, LOOK_AT_ANGLE_TOLERANCE_ROAMING, ref currentAngleUp, Movement.LookAtMode.None);
		} else if (mode == Mode.SHOOTING) {
			Shoot();
			mode = Mode.AIMING;
		}
		if (mode == Mode.AIMING) {
			play.movement.Roam(myRigidbody, ref targetPosition, 2, 4, FORCE_MOVE);
			play.movement.LookAt(myRigidbody, play.ship.transform, LOOK_AT_DISTANCE, LOOK_AT_ANGLE_TOLERANCE_AIMING, ref currentAngleUp, Movement.LookAtMode.None);
		}
	}

	private void Shoot() {
		GameObject newBullet = game.CreateFromPrefab().CreateGunBullet(transform.position + transform.TransformDirection(BULLET_POSITION), transform.rotation);
		Vector3 bulletDirection = transform.forward * Shot.SPEED;
		newBullet.rigidbody.AddForce(bulletDirection);
	}

}

