using UnityEngine;
using System.Collections;

public class MineBuilder : MonoBehaviour {
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

/*		Vector3 cubePosition = Room.GetCubePosition(transform.position);
		if (cubePosition == targetCubePosition) {
			targetCubePosition = play.room.GetRandomEmptyCubePositionFrom(cubePosition, Random.Range(2,4));
//			Debug.Log ("current " + cubePosition + ", setting new target " + targetCubePosition + " in frame " + Time.frameCount);
		}
		if (cubePosition != targetCubePosition) {
			Vector3 avoidance = Vector3.zero;		
			for (int i=0; i<Room.DIRECTIONS.Length; i++) {
				if (Physics.Raycast(transform.position, Room.DIRECTIONS[i], out hit, RAYCAST_DISTANCE, layerMaskAll)) {
					avoidance += hit.normal * (RAYCAST_DISTANCE/hit.distance);
				}
			}
			Vector3 target = (Room.GetPositionFromCube(targetCubePosition) - transform.position).normalized;
			// if obstacle in target direction, get new target
			if (Physics.Raycast(transform.position, target, out hit, RAYCAST_DISTANCE, layerMaskMoveables)) {
				targetCubePosition = play.room.GetRandomEmptyCubePositionFrom(cubePosition, Random.Range(2,4));
			}
//			Debug.Log (avoidance + " " + target);
		
			rigidbody.AddForce((avoidance.normalized + target) * FORCE_MOVE);
		}*/


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

