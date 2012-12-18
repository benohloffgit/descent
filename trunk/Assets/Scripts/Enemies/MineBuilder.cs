using UnityEngine;
using System.Collections;

public class MineBuilder : MonoBehaviour {
	private Play play;
	private Game game;
		
	private Rigidbody rigidbody;
	private int layerMask;
	private RaycastHit hit;

	private static float FORCE_MOVE = 100.0f;
	private static float RAYCAST_DISTANCE = 2.0f;
	private static Vector3[] directions = new Vector3[] { Vector3.forward, -Vector3.forward, Vector3.up, -Vector3.up, Vector3.right, -Vector3.right };
	
	void Awake() {
		rigidbody = GetComponent<Rigidbody>();
		layerMask = ~((1 << Game.LAYER_SHIP) | (1 << Game.LAYER_ENEMIES) | (1 << Game.LAYER_BULLETS) | (1 << Game.LAYER_GUI));
	}
	
	public void Initialize(Game g, Play p) {
		game = g;
		play = p;
	}
	
	void Start() {
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
	}
		
	void FixedUpdate() {
		if (rigidbody.velocity == Vector3.zero) {
//			Debug.Log ("setting new velocity in frame " + Time.frameCount);
			Vector3 vel = Vector3.zero;
			
			for (int i=0; i<directions.Length; i++) {
				if (Physics.Raycast(transform.position, directions[i], out hit, RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
					vel = vel + ((hit.normal * (RAYCAST_DISTANCE/hit.distance))).normalized;
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
			if (vel == Vector3.zero) {
				vel = EnemyDistributor.RandomVector() * FORCE_MOVE;
			} else {
				vel *= FORCE_MOVE;
			}
			rigidbody.AddForce(vel);
		}
	}
		
}


