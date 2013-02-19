using UnityEngine;
using System.Collections;

public class MineBuilder : MonoBehaviour {
	private Play play;
	private Game game;
		
	private Rigidbody myRigidbody;
	private RaycastHit hit;
	private GridPosition targetPosition;
//	private Vector3 targetCubePosition;
//	private IntTriple roomPos;
	private float currentAngleUp;

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
		targetPosition = play.cave.GetGridFromPosition(transform.position);
	}
		
	void FixedUpdate() {
		play.movement.Roam(myRigidbody, ref targetPosition, 2, 4, FORCE_MOVE);
		play.movement.LookAt(myRigidbody, play.ship.transform, LOOK_AT_DISTANCE, LOOK_AT_ANGLE_TOLERANCE, ref currentAngleUp);
		
	}		
}


			/*Collider[] colliders = Physics.OverlapSphere(transform.position, 1.0f, 1 << Game.LAYER_CAVE);
			foreach (Collider c in colliders) {
				//vel += hit.normal * FORCE_MOVE; //(RAYCAST_DISTANCE/hit.distance);
				Debug.Log(c.name + " " + c.ClosestPointOnBounds(transform.position) + " " + transform.position + " " + (c.ClosestPointOnBounds(transform.position) - transform.position).magnitude + " " + vel);
			}*/
			
/*			RaycastHit[] hits = Physics.SphereCastAll(transform.position, RAYCAST_DISTANCE, transform.position, 0.01f, layerMask);
			foreach (RaycastHit hit in hits) {
				vel += hit.normal * FORCE_MOVE; //(RAYCAST_DISTANCE/hit.distance);
				Debug.Log(hit.collider.name + " " + hit.normal + " " + hit.distance + " " + vel);
			}*/

		/*
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, 20.0f, transform.up, 20.0f);
		Debug.Log ("hitting");
		foreach (RaycastHit hit in hits) {
			Debug.Log(hit.collider.name + " " + hit.normal);
		}
		
		Debug.Log ("hitting2");
		Collider[] colliders = Physicshysics.OverlapSphere(transform.position, 10.0f)
		foreach (Collider c in colliders) {
			Debug.Log(c.name);
		}
		*/

