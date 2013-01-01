using UnityEngine;
using System.Collections;

public class WallLaser : MonoBehaviour {
	private Play play;
	private Game game;

	private Transform laserAnchor;
	private Transform laserBeam;
	private Vector3 baseForward;
	private Vector3 baseRight;
	private Vector3 aimForward;
	private RaycastHit hit;
	private float lastDamageTime;
	
	private static float LASER_LENGTH_MODIFIER = 0.95f;
	private static float RAYCAST_DISTANCE = 30.0f;
	private static float TURNING_SPEED = 0.005f;
	private static float DAMAGE_FREQUENCY = 1.0f; // seconds
	
	void Awake () {
		laserAnchor = transform.FindChild("Laser Anchor");
		laserBeam = laserAnchor.FindChild("Laser Beam");
	}
	
	public void Initialize(Game g, Play p) {
		game = g;
		play = p;
	}
	
	void Start() {
		baseForward = laserAnchor.forward;
		baseRight = laserAnchor.right;
		GetNewAimForward();
		lastDamageTime = Time.time;
	}
	
	void FixedUpdate() {
		if (Time.time > lastDamageTime + DAMAGE_FREQUENCY && Physics.Raycast(laserAnchor.position, laserAnchor.forward, out hit, RAYCAST_DISTANCE, Game.LAYER_MASK_SHIP)) {
			lastDamageTime = Time.time;
			Debug.Log("Wall Laser HIT");
		}
		
		if (aimForward != laserAnchor.forward) {
			Vector3 rotateTo = Vector3.RotateTowards(laserAnchor.forward, aimForward, TURNING_SPEED, TURNING_SPEED);
			laserAnchor.forward = rotateTo;
			ResizeLaser();
		} else {
			GetNewAimForward();
		}
	}
	
	private void ResizeLaser() {
		Vector3 aimAt;
		if (Physics.Raycast(laserAnchor.position, laserAnchor.forward, out hit, RAYCAST_DISTANCE, Game.LAYER_MASK_ALL)) {
			aimAt = hit.point;
		} else {
			aimAt = laserAnchor.position + laserAnchor.forward * RAYCAST_DISTANCE;
		}
		float laserLength = Vector3.Distance(laserAnchor.position, aimAt * LASER_LENGTH_MODIFIER);
		laserBeam.localScale = new Vector3(1.0f, 1.0f, laserLength);
		laserBeam.position = laserAnchor.position + laserAnchor.forward * (laserLength/2.0f);
	}
	
	private void GetNewAimForward() {
		aimForward = Quaternion.AngleAxis(Random.Range(1.0f, 45.0f), baseRight) * baseForward;
		aimForward = Quaternion.AngleAxis(Random.Range(1.0f, 360.0f), baseForward) * aimForward;
	}
	
}

