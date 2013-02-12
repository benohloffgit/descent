using UnityEngine;
using System.Collections;

public class LightBulb : MonoBehaviour {
	private Play play;
	private Game game;
		
	private Rigidbody myRigidbody;
	private RaycastHit hit;
	private GridPosition targetPosition;

	private static float FORCE_MOVE = 5.0f;
	private static int LOOK_AT_DISTANCE = 4; // measured in cubes
	private static float LOOK_AT_ANGLE_TOLERANCE = 30.0f;
	
	void Awake() {
		myRigidbody = GetComponent<Rigidbody>();
	}
	
	public void Initialize(Game g, Play p) {
		game = g;
		play = p;
	}
	
	void Start() {
		targetPosition = Cave.GetGridFromPosition(transform.position);
	}
		
	void FixedUpdate() {
		play.movement.Roam(myRigidbody, ref targetPosition, 2, 4, FORCE_MOVE);
		play.movement.LookAt(myRigidbody, play.ship.transform, LOOK_AT_DISTANCE, LOOK_AT_ANGLE_TOLERANCE);
	}		
}
