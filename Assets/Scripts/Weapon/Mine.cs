using UnityEngine;
using System.Collections;
/*
public class Mine : MonoBehaviour {
	public static int TOUCH = 0;

	public int type;	
	private Play play;
	
	private int damage;
	private int source;
	
	private Vector3 basePosition;
	private Vector3 targetPosition;
	
	private static float MOVEMENT_RADIUS = 0.25f;
	private static float MOVEMENT_SPEED = 0.01f;
	private static float EXPLOSION_POWER = 500.0f;
	private static float EXPLOSION_RADIUS = 2.0f * RoomMesh.MESH_SCALE;
	
	public void Initialize(Play p, int damage_, int source_, int type_) {
		play = p;
		damage = damage_;
		source = source_;
		type = type_;
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

*/