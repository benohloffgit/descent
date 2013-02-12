using UnityEngine;
using System.Collections;

public class WallGun : MonoBehaviour {
	private Play play;
	private Game game;

	private Transform barrelAnchor;
	private Vector3 baseForward;
	private float lastShotTime;
	
	private static float SHOOTING_FREQUENCY = 1.0f;

	void Awake () {
		barrelAnchor = transform.Find("Barrel Anchor");
	}
	
	public void Initialize(Game g, Play p) {
		game = g;
		play = p;
	}
	
	void Start() {
		baseForward = barrelAnchor.forward;
		lastShotTime = Time.time;
	}
	
	void Update() {
		Vector3 isVisible = play.ship.IsVisibleFrom(barrelAnchor.position);
		if (isVisible != Vector3.zero) {
			Vector3 rotateTo = Vector3.RotateTowards(barrelAnchor.forward, isVisible.normalized, 0.01f, 0.01f);
			if (Vector3.Angle(baseForward, rotateTo) < 45.0f) {
	//			Debug.Log (barrelAnchor.forward + " " + toShip + " " +rotateTo);
	//			barrelAnchor.LookAt(barrelAnchor.TransformPoint(rotateTo));
				barrelAnchor.forward = rotateTo;
				
				if (Time.time > lastShotTime + SHOOTING_FREQUENCY) {
					Shoot();
					lastShotTime = Time.time;
				}
			}
		}
	}
	
	private void Shoot() {
		GameObject newBullet = game.CreateFromPrefab().CreateGunBullet(barrelAnchor.position, barrelAnchor.rotation);
		Vector3 bulletDirection = barrelAnchor.forward * Game.GUN_BULLET_SPEED;
		newBullet.rigidbody.AddForce(bulletDirection);
	}
	
}
