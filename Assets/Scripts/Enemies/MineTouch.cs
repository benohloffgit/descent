using UnityEngine;
using System.Collections;

public class MineTouch : MonoBehaviour {
	private Play play;
	private Game game;
	
	private Vector3 basePosition;
	private Vector3 targetPosition;
	
	private static float MOVEMENT_RADIUS = 0.25f;
	private static float MOVEMENT_SPEED = 0.01f;
	
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
	
}

