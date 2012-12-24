using UnityEngine;
using System.Collections;

public class LightBulb : MonoBehaviour {
	private Play play;
	private Game game;
		
	private Rigidbody rigidbody;
	private RaycastHit hit;
	private Vector3 targetCubePosition;

	private static float FORCE_MOVE = 5.0f;
	private static int LOOK_AT_DISTANCE = 4; // measured in cubes
	
	void Awake() {
		rigidbody = GetComponent<Rigidbody>();
	}
	
	public void Initialize(Game g, Play p) {
		game = g;
		play = p;
	}
	
	void Start() {
		targetCubePosition = Room.GetCubePosition(transform.position);
	}
		
	void FixedUpdate() {
		play.movement.Roam(rigidbody, ref targetCubePosition, 2, 4, FORCE_MOVE);
		play.movement.LookAt(rigidbody, play.ship.transform, LOOK_AT_DISTANCE);
		
	}		
}
