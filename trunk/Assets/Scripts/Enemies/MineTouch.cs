using UnityEngine;
using System.Collections;

public class MineTouch : MonoBehaviour {
	private Play play;
	private Game game;
	
	private Vector3 basePosition;
	private Vector3 targetPosition;
	
	private static float MOVEMENT_RADIUS = 0.25f;
	private static float MOVEMENT_SPEED = 0.01f;
	private static float EXPLOSION_POWER = 500.0f;
	private static float EXPLOSION_RADIUS = 2.0f * RoomMesh.MESH_SCALE;
	
	public void Initialize(Game g, Play p) {
		game = g;
		play = p;
	}
	
	void Start() {
		basePosition = transform.position;
		SetTargetPosition();
	}
	
	void Update() {
		if (targetPosition != transform.position) {
			transform.position = Vector3.MoveTowards (transform.position, targetPosition, MOVEMENT_SPEED);
		} else {
			SetTargetPosition();
		}
	}
	
	private void SetTargetPosition() {
		targetPosition = basePosition + EnemyDistributor.RandomVector() * MOVEMENT_RADIUS;
	}
	
	private void OnTriggerEnter(Collider c) {
		if (!play.isShipInvincible) {
			c.rigidbody.AddExplosionForce(EXPLOSION_POWER, transform.position, EXPLOSION_RADIUS);
//			Debug.Log("Mine Touch EXPLOSION");
		}
	}
	
}

